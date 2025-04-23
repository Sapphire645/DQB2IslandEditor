using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ChunkBlockGrid : UserControl
    {
        private ChunkEditorViewModel viewModel;

        //Get delegated idiot
        private Action<ItemContainer>? enter;
        private Action<ItemContainer>? leave;
        private Action<ItemContainer>? click;
        private Action<ItemContainer>? release;

        public Dictionary<ushort, List<ItemContainer>> itemContainerGrid = new Dictionary<ushort, List<ItemContainer>>();
        public ChunkBlockGrid()
        {
            InitializeComponent();
        }
        public void SetActions(Action<ItemContainer> enter, Action<ItemContainer> leave, Action<ItemContainer> click, Action<ItemContainer> release)
        {
            this.enter = enter;
            this.leave = leave;
            this.click = click;
            this.release = release;
        }

        public TileContainer[] CreateTiles(ChunkEditorViewModel viewModel)
        {
            this.viewModel = viewModel;
            try
            {
                TileContainer[] tileContainers = new TileContainer[1024]; // layers are 32x32

                for (ushort i = 0; i < 1024; i++) 
                {
                    tileContainers[i] = new TileContainer
                    {
                        offset = i
                    };

                    TileGrid.Children.Add(tileContainers[i]);
                }
                return tileContainers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private void ChangeChunk(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            viewModel.ChangeChunk(tag);
        }
        private void ChangeLayer(object sender, RoutedEventArgs e)
        {
            viewModel.CurrentLayer = (byte)(viewModel.CurrentLayer + short.Parse((sender as Button).Tag.ToString()));
        }

        //For more intuitive movement.
        private void ScrollUpdate(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
            {
                Point localPosition = e.GetPosition(this);
                Point relativePosition = new Point((localPosition.X / ActualWidth) - 0.5, (localPosition.Y / ActualHeight) - 0.5);
                //If the difference is too low then dont move.
                if (Math.Abs(Math.Abs(relativePosition.X) - Math.Abs(relativePosition.Y)) < 0.08) return;

                if (Math.Abs(relativePosition.X) > Math.Abs(relativePosition.Y))
                    if (e.Delta > 0)
                        viewModel.ChangeChunk("r");
                    else
                        viewModel.ChangeChunk("l");
                else
                    if (e.Delta > 0)
                        viewModel.ChangeChunk("u");
                    else
                        viewModel.ChangeChunk("d");
            }
        }

        public void UpdateItemsOnGrid(List<ItemInstance> items)
        {
            ItemGrid.Children.Clear();
            itemContainerGrid.Clear();
            foreach (var item in items)
            {
                Console.WriteLine(item.ToString());
                ItemContainer itemContainer = new ItemContainer(item, 32); //  1024/32

                itemContainer.ItemBorder.PreviewMouseDown += (_, _) => { click(itemContainer); };
                itemContainer.ItemBorder.MouseEnter += (_, _) => { enter(itemContainer); };
                itemContainer.ItemBorder.MouseLeave += (_, _) => { leave(itemContainer); };
                itemContainer.ItemBorder.MouseLeftButtonUp += (_, _) => { release(itemContainer); };

                ItemGrid.Children.Add(itemContainer);
                //Alright, here comes the funky stuff
                //You can have multiple items in the same tile, meaning, chaos
                //This will create the need to have multiple item panels for each item in that tile
                //I will also have to keep track of every item in the tile
                //So, the dictionary holds the offset (x + z*32), with a list of all the items on the offset
                //That is all.
                if(!itemContainerGrid.ContainsKey(item.worldOffset))
                    itemContainerGrid[item.worldOffset] = new List<ItemContainer>();
                itemContainerGrid[item.worldOffset].Append(itemContainer);
                //Hope this doesnt tank the performance tbh
            }
        }

        public ItemInstance[] GetItemsInOffset(ushort offset)
        {
            if (!itemContainerGrid.ContainsKey(offset)) return null;
            var items = itemContainerGrid[offset];
            ItemInstance[] list = new ItemInstance[items.Count];
            for(int i = 0; i < items.Count; i++) list[i] = items.ElementAt(i).itemInstance;

            return list;
        }
        public void AddItemToOffset(ushort offset, ItemInfo Item)
        {
            //Not implemented yet.
        }
    }
}
