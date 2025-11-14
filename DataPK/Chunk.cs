using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DQB2IslandEditor.DataPK
{
    public class Chunk
    {
        public static readonly byte X_DIMENSION = 32;
        public static readonly byte Y_DIMENSION = 96;
        public static readonly byte Z_DIMENSION = 32;

        private const uint SIZE_LAYER = 0x800;

        public ushort chunkPosition { private set; get; }
        private byte[] blockBytes;
        private PropHandlerClass propHandler;
        private List<ItemInstance> itemInstances => propHandler != null ? propHandler.getItemsChunk(chunkPosition) : null;



        public byte chunkXPosition => (byte)(chunkPosition % Island.GRID_DIMENSION);
        public byte chunkYPosition => (byte)(chunkPosition / Island.GRID_DIMENSION);

        public Chunk(byte[] blockBytes, ushort chunkPosition, PropHandlerClass propHandlerClass) { 
            //This is for fixing Moonbrooke Corruption and similar.
            if(blockBytes != null && blockBytes.Length < (SIZE_LAYER* Y_DIMENSION))
            {
                this.blockBytes = new byte[SIZE_LAYER * Y_DIMENSION];
                Array.Copy(blockBytes, 0, this.blockBytes, 0, blockBytes.Length);
            }
            this.blockBytes = blockBytes;
            this.chunkPosition = chunkPosition;
            this.propHandler = propHandlerClass;
        }

        public bool IsEmpty() { return blockBytes == null; }
        public BlockInstance GetBlockFromCoords(byte x, byte z, byte layer)
        {
            var offset = layer * SIZE_LAYER + z * X_DIMENSION * 2 + x * 2;
            return new BlockInstance(blockBytes[offset], blockBytes[offset + 1]);
        }

        //Trying to make as fast as possible
        public BlockInstance[] GetBlocksFromLayer(byte layer)
        {
            BlockInstance[] layerBlocks = new BlockInstance[X_DIMENSION * Z_DIMENSION];
            if (blockBytes == null || layer > 95) {
                //Set water when gottrn
                    for (int i = 0; i < Chunk.X_DIMENSION * Chunk.Z_DIMENSION; i++) layerBlocks[i] = new BlockInstance(0, 0);
                    return layerBlocks;
            }
            var offset = layer * SIZE_LAYER;
            for (var i = 0; i < X_DIMENSION * Z_DIMENSION; i ++) layerBlocks[i] = new BlockInstance(blockBytes[offset + i * 2], blockBytes[offset + i * 2 + 1]);

            return layerBlocks;
        }
        public BlockInstance[] GetBlocksFromTopView()
        {
            BlockInstance[] layerBlocks = new BlockInstance[X_DIMENSION * Z_DIMENSION];

            int[][] IDs = new int[8][];
            for (int i = 0; i < 8; i++) IDs[i] = new int[8];
            for (int x = 0; x < 8; x++)
                for (int z = 0; z < 8; z++)
                    IDs[x][z] = 0;

            byte size = (byte)(X_DIMENSION / 8);

            Dictionary<int, byte> Count = new Dictionary<int, byte>();

            for (byte X_DIM = 0; X_DIM < 8; X_DIM++)
                for (byte Z_DIM = 0; Z_DIM < 8; Z_DIM++)
                //Per 8 tiles.
                {
                    Count.Clear();
                    for (byte x = (byte)(X_DIM * size); x < (X_DIM + 1) * size; x++)
                        for (byte z = (byte)(Z_DIM * size); z < (Z_DIM + 1) * size; z++)
                            for (byte y = (byte)(Y_DIMENSION - 1); y < 255; y--)
                            {
                                BlockInstance curr = GetBlockFromCoords(x, z, y);

                                if (curr.publicBlockID != 0) //Not air.
                                {
                                    layerBlocks[x + (z * 32)] = curr;
                                    break;
                                }
                                else if (y == 0)
                                {
                                    //If reached bottom. if Air, set water.
                                    layerBlocks[x + (z* 32)] = new BlockInstance(0, Chisel.Full, false);
                                }
                            }
                }
            for (int i = 0; i < 1024; i++)
            {
                if (layerBlocks[i] == null)
                    layerBlocks[i] = new BlockInstance(0, Chisel.Full, false);
            }
            return layerBlocks;
        }

        public List<ItemInstance> GetItemsFromLayer(byte layer)
        {
            List<ItemInstance> layerItems = new List<ItemInstance>();

            foreach (var item in itemInstances)
            {
                if (item.IsInLayer(layer))
                {
                    layerItems.Add(item);
                }
            }
            return layerItems;
            
        }
        public List<ItemInstance> GetItemsForOverflowChunk(bool north, bool south, bool east, bool west, byte layer)
        {
            List<ItemInstance> layerItems = new List<ItemInstance>();

            foreach (var item in itemInstances)
            {
                if (item.DoIOverflow(north, south, east, west) && item.IsInLayer(layer)) layerItems.Append(item);
            }
            return layerItems;
        }
        public void RemoveItem(ItemInstance item)
        {
            propHandler.RemoveItem(item);
        }
        public void AddItem(ItemInstance item)
        {
            itemInstances.Add(item);
        }
        public List<ItemInstance> GetAllItems()
        {
            return itemInstances;
        }
        public void SetBlockFromCoords(BlockInstance blockInstance, byte x, byte z, byte layer)
        {
            if (blockBytes == null) return;
            var bytes = blockInstance.GetBytes();
            var offset = layer * SIZE_LAYER + z * X_DIMENSION * 2 + x * 2;
            blockBytes[offset] = bytes[0];
            blockBytes[offset+1] = bytes[1];
        }


        public void CoverGroundWith(BlockInstance source, BlockInstance newindex)
        {
            if (blockBytes == null) return;

            for (byte x = 0; x< X_DIMENSION; x++)
                for (byte z = 0; z < Z_DIMENSION; z++)
                    for (byte y = (byte)(Y_DIMENSION - 1); y < 255 ; y--)
                    {
                        BlockInstance curr = GetBlockFromCoords(x, z, y);

                        if (curr.publicBlockID == source.publicBlockID)
                            SetBlockFromCoords(newindex, x,z,y);
                        if (curr.publicBlockID != 0) break;
                    }
        }

        public (int[][],bool[][]) GetTileMinimapID()
        {
            int[][] IDs = new int[4][];
            bool[][] heights = new bool[4][];
            for (int i = 0; i < 4; i++)
            {
                IDs[i] = new int[4];
                heights[i] = new bool[4];
            }
            for (int x = 0; x < 4; x++)
                for (int z = 0; z < 4; z++)
                    IDs[x][z] = 0;
            //Empty chunk
            if (blockBytes == null)
            {
                return (IDs,heights);
            }
            byte size = (byte)(X_DIMENSION / 4);

            Dictionary<int, byte> Count = new Dictionary<int, byte>();
            for (byte X_DIM = 0; X_DIM < 4; X_DIM++)
                for (byte Z_DIM = 0; Z_DIM < 4; Z_DIM++)
                    //Per 8 tiles.
                {
                    byte heightMin = 255;
                    byte heightMax = 0;
                    Count.Clear();
                    for (byte x = (byte)(X_DIM * size); x < (X_DIM+1) * size; x++)
                        for (byte z = (byte)(Z_DIM * size); z < (Z_DIM + 1) * size; z++)
                            for (byte y = (byte)(Y_DIMENSION - 1); y < 255; y--)
                            {
                                BlockInstance curr = GetBlockFromCoords(x, z, y);

                                if (curr.publicBlockID != 0) //Not air.
                                {
                                    if (heightMax < y) heightMax = y;
                                    if (heightMin > y) heightMin = y;
                                    //get tile it belongs to.
                                    var t = DataBaseReading.BLOCK_MINIMAP_DICTIONARY[(ushort)curr.publicBlockID];
                                    //Count up
                                    if (Count.ContainsKey(t))
                                        Count[t]++;
                                    else
                                        Count[t] = 0;
                                    break;
                                }
                                else if(y == 0)
                                {
                                    //If reached bottom. if Air, set water.
                                    var t = DataBaseReading.BLOCK_MINIMAP_DICTIONARY[0];
                                    if (Count.ContainsKey(t))
                                        Count[t]++;
                                    else
                                        Count[t] = 0;
                                    break;
                                }
                            }
                    int TileID = 0;
                    int c = 0;
                    foreach(int key in Count.Keys)
                    {
                        if (Count[key] > c && key >= 0)
                        {
                            c = Count[key];
                            TileID = key;
                        }
                    }
                    IDs[X_DIM][Z_DIM] = TileID;
                    heights[X_DIM][Z_DIM] = heightMax > 49; //- heightMin > 8;
                }
            return (IDs, heights);

        }

        override public string ToString()
        {
            StringBuilder output = new StringBuilder("CHUNK = {\n\tR : Position");

            output.Append(chunkPosition);
            output.Append(",\n}\n");
            return output.ToString();
        }
        public byte[] GetBytes()
        {
            return blockBytes;
        }
    }
}
