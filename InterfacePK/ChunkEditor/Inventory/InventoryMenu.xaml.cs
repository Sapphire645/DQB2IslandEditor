using DQB2IslandEditor.ObjectPK.Container;
using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Inventory
{
    /// <summary>
    /// Interaction logic for InventoryMenu.xaml
    /// </summary>
    public partial class InventoryMenu : UserControl
    {
        public InventoryMenu()
        {
            InitializeComponent();
        }

        public void shareViewModel(ChunkEditorViewModel viewModel) {
            inventoryGridBlock.shareViewModel(viewModel);
            inventoryGridLiquid.shareViewModel(viewModel);
            inventoryGridItem.shareViewModel(viewModel);
        }

        public async void CreateInventory(IDictionary<uint, ObjectInfo> fullBlockList, IDictionary<uint, ObjectInfo> fullItemList, ChunkEditorWindow chunkEditorWindow)
        {
            Task<IDictionary<byte, object>> parityBlocks = Task.Run(() => DataBaseReading.BlockMenuData());
            Task<IDictionary<byte, object>> parityItemsA = Task.Run(() => DataBaseReading.ItemAMenuData());
            //Task<IDictionary<uint, List<uint>>> parityItem = Task.Run(() => DataBaseReading.ItemParity());

            inventoryGridBlock.CreateFilterButtons(new List<string> { "Used", "Unused", "Indestructible", "Liquid", "NULL" },new byte[3] { 0, 1, 3 });
            inventoryGridLiquid.CreateFilterButtons(new List<string> { "Scoopable", "Unscoopable" }, new byte[2] { 0, 1 });
            inventoryGridItem.CreateFilterButtons(new List<string> { "Used", "Unused", "NULL" }, new byte[1] { 0 });

            var blockParity = parityBlocks.Result;
            var itemsAParity = parityItemsA.Result;
            //var itemParity = parityItem.Result;
            //try to thread
            Task inv1 = Task.Run(() => inventoryGridBlock.CreateInventory(fullBlockList, blockParity));
            //Task inv2 = Task.Run(() => inventoryGridLiquid.CreateInventory(fullBlockList, false, true, liquidParity));
            Task inv3 = Task.Run(() => inventoryGridItem.CreateInventory(fullItemList, itemsAParity));

            await inv1;
            //await inv2;
            await inv3;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                chunkEditorWindow.SavedText.Opacity = 0;
            }));
        }

        public void Resize(double height, double width)
        {
            gridInventory.Height = height;
            foreach(var item in BlocksTabs.Items)
            {
               ((TabItem)item).Height = height/3;
            }
            inventoryGridBlock.Resize(inventoryGridBlock.ActualWidth);
            inventoryGridLiquid.Resize(inventoryGridBlock.ActualWidth);
            inventoryGridItem.Resize(inventoryGridBlock.ActualWidth);
        }
    }
}
