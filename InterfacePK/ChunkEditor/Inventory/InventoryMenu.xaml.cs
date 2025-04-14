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

        public async void CreateInventory(IDictionary<uint, ObjectInfo> fullBlockList, ChunkEditorWindow chunkEditorWindow)
        {
            Task<List<(uint, uint)>> parityBlocks = Task.Run(() => DataBaseReading.BlockParity());

            inventoryGridBlock.CreateFilterButtons(new List<string> { "Used", "Unused", "Indestructible", "Item shells", "NULL" },new byte[3] { 0, 1, 2 });
            inventoryGridLiquid.CreateFilterButtons(new List<string> { "Scoopable", "Unscoopable", "Item shells", "Unused" }, new byte[2] { 0, 1, });
            inventoryGridItem.CreateFilterButtons(new List<string> { "Used", "Unused", "NULL" }, new byte[1] { 0 });

            var blockParity = parityBlocks.Result;
            //try to thread
            Task inv1 = Task.Run(() => inventoryGridBlock.CreateInventory(fullBlockList, false, false, blockParity));
            Task inv2 = Task.Run(() => inventoryGridLiquid.CreateInventory(fullBlockList, false, true, blockParity));

            await inv1;
            await inv2;

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
            height = height - 54;
            if (height < 0) height = 0;
            inventoryGridBlock.ScrollPanel.Height = height;
            inventoryGridBlock.Resize(inventoryGridBlock.ActualWidth);
            inventoryGridLiquid.ScrollPanel.Height = height;
            inventoryGridLiquid.Resize(inventoryGridBlock.ActualWidth);
            inventoryGridItem.ScrollPanel.Height = height;
            inventoryGridItem.Resize(inventoryGridBlock.ActualWidth);
        }
    }
}
