using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.DataPK
{
    internal class Chunk
    {
        public static readonly byte X_DIMENSION = 32;
        public static readonly byte Y_DIMENSION = 96;
        public static readonly byte Z_DIMENSION = 32;

        private const uint SIZE_LAYER = 0x800;

        public ushort chunkPosition { private set; get; }
        private byte[] blockBytes;
        private List<ItemInstance> itemInstances;

        public byte chunkXPosition => (byte)(chunkPosition % Island.GRID_DIMENSION);
        public byte chunkYPosition => (byte)(chunkPosition / Island.GRID_DIMENSION);

        public Chunk(byte[] blockBytes, ushort chunkPosition) { 
            //This is for fixing Moonbrooke Corruption and similar.
            if(blockBytes != null && blockBytes.Length < (SIZE_LAYER* Y_DIMENSION))
            {
                this.blockBytes = new byte[SIZE_LAYER * Y_DIMENSION];
                Array.Copy(blockBytes, 0, this.blockBytes, 0, blockBytes.Length);
            }
            this.blockBytes = blockBytes;
            this.chunkPosition = chunkPosition;
            itemInstances = new List<ItemInstance>();
        }

        public bool IsEmpty() { return blockBytes == null; }
        public void SetItemArray(List<ItemInstance> itemInstances) {
            this.itemInstances = itemInstances;
        }

        public BlockInstance GetBlockFromCoords(byte x, byte z, byte layer)
        {
            var offset = layer * SIZE_LAYER + z * X_DIMENSION * 2 + x * 2;
            return new BlockInstance(blockBytes[offset], blockBytes[offset + 1]);
        }

        //Trying to make as fast as possible
        public BlockInstance[] GetBlocksFromLayer(byte layer)
        {
            if(blockBytes == null) return null;
            BlockInstance[] layerBlocks = new BlockInstance[X_DIMENSION * Z_DIMENSION];
            var offset =  layer * SIZE_LAYER;
            for (var i = 0; i < X_DIMENSION * Z_DIMENSION; i ++)
            {
                layerBlocks[i] = new BlockInstance(blockBytes[offset + i * 2], blockBytes[offset + i * 2 + 1]);
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
            itemInstances.Remove(item);
        }
        public void AddItem(ItemInstance item)
        {
            itemInstances.Add(item);
        }
        public void SetBlockFromCoords(BlockInstance blockInstance, byte x, byte z, byte layer)
        {
            if (blockBytes == null) return;
            var bytes = blockInstance.GetBytes();
            var offset = layer * SIZE_LAYER + z * X_DIMENSION * 2 + x * 2;
            blockBytes[offset] = bytes[0];
            blockBytes[offset+1] = bytes[1];
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
