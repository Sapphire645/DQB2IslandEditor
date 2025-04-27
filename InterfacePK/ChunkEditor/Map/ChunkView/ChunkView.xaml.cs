using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Map.ChunkView
{
    public partial class ChunkView : UserControl
    {
        private ChunkEditorViewModel viewModel;

        private ushort chunkIndex;
        public ushort ChunkIndex
        {
            get
            {
                return this.chunkIndex;
            }
            set
            {
                if (value >= Island.GRID_DIMENSION * Island.GRID_DIMENSION) return;
                this.chunkIndex = value;
                UpdateView(null,null);
            }
        }
        private byte layer => viewModel.CurrentLayer;
        private Chunk displayedChunk => viewModel.CurrentIsland.GetChunk(chunkIndex);

        public Dictionary<ushort, List<ItemContainer>> itemContainerGrid = new Dictionary<ushort, List<ItemContainer>>();
        public TileContainer[] tiles = new TileContainer[Chunk.X_DIMENSION*Chunk.Z_DIMENSION];
        public ChunkView(ChunkEditorViewModel viewModel)
        {
            InitializeComponent();
            viewModel.LayerUpdateNotification += UpdateView;
            this.viewModel = viewModel;
            try
            {
                for (ushort i = 0; i < 1024; i++)
                {
                    tiles[i] = new TileContainer(i, Enter,
                        Leave, LeftClick, Release);

                    TileGrid.Children.Add(tiles[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Enter(ushort offset) { viewModel.BlockTile_TileEnter(offset, chunkIndex); }
        private void Leave(ushort offset) { viewModel.BlockTile_TileLeave(offset, chunkIndex); }
        private void LeftClick(ushort offset) { viewModel.BlockTile_LeftClick(offset, chunkIndex); }
        private void Release(ushort offset) { viewModel.BlockTile_Release(offset, chunkIndex); }

        //Update the display.
        private void UpdateView(object sender, PropertyChangedEventArgs e)
        {
            var blockInstances = displayedChunk.GetBlocksFromLayer(layer);
            for (int i = 0; i < 1024; i++)
            {
                tiles[i].blockInstance = blockInstances[i];
            }
            var itemInstances = displayedChunk.GetItemsFromLayer(layer);
            //should have delegated the block one to the chunk block grid as well but ah.
            UpdateItemsOnGrid(itemInstances);
        }


        private void UpdateItemsOnGrid(List<ItemInstance> items)
        {
            ItemGrid.Children.Clear();
            itemContainerGrid.Clear();
            foreach (var item in items)
            {
                Console.WriteLine(item.ToString());
                ItemContainer itemContainer = new ItemContainer(item, 32); //  1024/32

                itemContainer.ItemBorder.PreviewMouseDown += (_, _) => { viewModel.ItemTile_LeftClick(itemContainer); };
                itemContainer.ItemBorder.MouseEnter += (_, _) => { viewModel.ItemTile_TileEnter(itemContainer); };
                itemContainer.ItemBorder.MouseLeave += (_, _) => { viewModel.ItemTile_TileLeave(itemContainer); };
                itemContainer.ItemBorder.MouseLeftButtonUp += (_, _) => { viewModel.ItemTile_Release(itemContainer); };

                ItemGrid.Children.Add(itemContainer);
                //Alright, here comes the funky stuff
                //You can have multiple items in the same tile, meaning, chaos
                //This will create the need to have multiple item panels for each item in that tile
                //I will also have to keep track of every item in the tile
                //So, the dictionary holds the offset (x + z*32), with a list of all the items on the offset
                //That is all.
                if (!itemContainerGrid.ContainsKey(item.worldOffset))
                    itemContainerGrid[item.worldOffset] = new List<ItemContainer>();
                itemContainerGrid[item.worldOffset].Append(itemContainer);
                //Hope this doesnt tank the performance tbh
            }
        }

        //------------------------------------------------------------------
        //GETTING BLOCKS / ITEMS
        public BlockInstance GetBlockInstanceInOffset(ushort offset)
        {
            return tiles[offset].blockInstance;
        }
        public BlockInfo GetBlockInfoInOffset(ushort offset)
        {
            return tiles[offset].blockInfo.Value;
        }

        //this gets the items that are in a specific coordinate.
        public ItemInstance[] GetItemsInOffset(ushort offset)
        {
            if (!itemContainerGrid.ContainsKey(offset)) return null;
            var items = itemContainerGrid[offset];
            ItemInstance[] list = new ItemInstance[items.Count];
            for (int i = 0; i < items.Count; i++) list[i] = items.ElementAt(i).itemInstance;

            return list;
        }
        //------------------------------------------------------------------
        //Setting blocks and items
        public void AddItemToOffset(ushort offset, ItemInfo Item)
        {
            //Not implemented yet.
        }


        //--------------------------------------------------------------------
        //Asthetic:
        private void TileAura_Create(ushort[] offsets)
        {
            foreach (var offset in offsets)
            {
                tiles[offset].IsHovered();
            }
        }
        private void TileAura_SetBlock(ushort[] offsets,BlockInstance bi)
        {
            foreach (var offset in offsets)
            {
                Offset_SetBlock(offset, bi);
            }
        }

        private void TileAura_Destroy(ushort[] offsets)
        {
            foreach (var offset in offsets)
            {
                tiles[offset].IsUnHovered();
            }
        }

        private void Offset_SetBlock(ushort offset, BlockInstance bi)
        {
            displayedChunk.SetBlockFromCoords(bi, (byte)(offset % Chunk.X_DIMENSION), (byte)(offset % Chunk.Z_DIMENSION), viewModel.CurrentLayer);
            tiles[offset].blockInstance = bi;
        }

    }
}
