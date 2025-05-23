using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Inventory
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    public partial class InventoryGrid : UserControl
    {
        private ChunkEditorViewModel viewModel;
        private bool _loaded = false;
        public ObservableProperty<int> Columns { get; private set; } = new ObservableProperty<int>() { Value = 3 };
        private Dictionary<ushort, InventoryContainer> inventoryItemsAll = new Dictionary<ushort, InventoryContainer>();
        public ObservableCollection<InventoryContainer> inventoryFilteredItems { get; private set; } = new ObservableCollection<InventoryContainer>();

        private List<ToggleButton> filterButtons = new List<ToggleButton>();
        private bool[] toggleFilter;
        private string _filterText = "" ;
        public InventoryGrid()
        {
            DataContext = this;
            InitializeComponent();
        }
        public void shareViewModel(ChunkEditorViewModel viewModel)
        {
            this.viewModel = viewModel;
        }
        public void CreateFilterButtons(List<String> Filters, byte[] active)
        {
            if (!_loaded)
            {
                TextFilter.TextChanged += (_, _) => { TextChange(TextFilter.Text); };
                toggleFilter = new bool[Filters.Count];
                for (byte i = 0; i < Filters.Count; i++)
                {
                    ToggleButton button = new ToggleButton()
                    {
                        Content = Filters[i],
                        Tag = i,
                        Background = Brushes.Black
                    };
                    button.Click += (_, _) => { FilterClicked(byte.Parse(button.Tag.ToString())); };
                    filterButtons.Add(button);
                    FilterToggleStack.Children.Add(button);

                    //Filtering
                    if (active.Contains(i))
                    {
                        toggleFilter[i] = true;
                        button.IsChecked = true;
                    }
                    else
                    {
                        toggleFilter[i] = false;
                    }
                }

            }
        }
        public async void CreateInventory(IDictionary<uint, ObjectInfo> fullBlockList, bool isItem, bool liquidCompare,
            IDictionary<uint, List<uint>> parity)
        {
            //Gotta change this later.
            if (!_loaded)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    DataContext = null; //try to make faster
                }));
                var sendingBlocks = new List<DispatcherOperation>();

                foreach (var itemFull in fullBlockList)
                {
                    var item = itemFull.Value;
                    if (!isItem && ((BlockInfo)item).liquid == liquidCompare)
                    {
                        if (item.colour == 0) //For all plain.
                        {
                            var send = Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                InventoryContainer inventoryItem = new InventoryContainer(item, ObjectClicked, ObjectRightClicked);
                                inventoryItemsAll.Add(item.objectId, inventoryItem);
                            }));
                            sendingBlocks.Add(send);
                        }
                    }
                }
                //Wait for the async
                foreach (var send in sendingBlocks)
                {
                    send.Wait();
                }

                //Nos create the colour submenus
                foreach (var parityMerge in parity.Keys)
                {
                    if (!inventoryItemsAll.ContainsKey((ushort)parityMerge)) continue;
                    List<ObjectInfo> values = new List<ObjectInfo>();
                    foreach (var index in parity[parityMerge])
                        values.Add(fullBlockList[index]);

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        inventoryItemsAll[(ushort)parityMerge].createSubMenu(true, 0, values);
                    }));
                }
                _loaded = true;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    FilterItems();
                }));
            }
        }

        private void FilterItems()
        {
            DataContext = null; //I was having a bug where inputting a 1 len text would somehow cause the filteredItems to corrupt and give an error on .Add()
            //No fkin idea so I just remover the access privileges
            //Its 2 AM fuck you InventoryGrid.xaml no datacontext for you stop complaining about indices
            inventoryFilteredItems.Clear();
            //Bool must be correct.
            foreach (var itemInventoryPair in inventoryItemsAll)
            {
                var itemInventory = itemInventoryPair.Value;
                var obinfo = itemInventory.CurrentObject; //Me save 5 clock cycles. me smart me optimize (joke).
                if (toggleFilter[obinfo.tab])
                {

                    //Change when colour menu is implemented
                    if (obinfo.name.ToLower().Contains(_filterText) || obinfo.objectId.ToString().Contains(_filterText))
                    {
                        inventoryFilteredItems.Add(itemInventory);
                    }
                    else
                    {
                        if (itemInventory.hasSubmenu)
                        {
                            //itemInventory.filter;
                            //foreach (var obvarinfo in itemInventory.alternateColours)
                            //{
                            //    if (obvarinfo.Value.fullName.ToLower().Contains(_filterText) || obvarinfo.Value.objectId.ToString().Contains(_filterText))
                            //    {
                            //            inventoryFilteredItems.Add(itemInventory);
                            //    }
                            //}
                        }
                    }

                }
            }
            DataContext = this;
        }

        private void ObjectClicked(ObjectInfo selectedObject)
        {
            viewModel.UpdateSelectedObject(selectedObject);
        }
        private void ObjectRightClicked(ObjectInfo selectedObject)
        {
            viewModel.UpdateFavouriteObject(selectedObject);
        }
        private void FilterClicked(byte number)
        {
            toggleFilter[number] = !toggleFilter[number];
            FilterItems();
        }
        public void Resize(double width)
        {
            width = width - 20;
            Columns.Value = (int)width / 56;
            InventoryBlockHolder.Width = ((int)width / 56) * 56;
            //ScrollPanel.Width = (((int)width / 56) * 56)+20;
        }

        private void TextChange(string filterText)
        {
            _filterText = filterText.ToLower();
            FilterItems();
        }
    }
}
