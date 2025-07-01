using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for SelectedObject.xaml
    /// </summary>
    public partial class SelectedObject : UserControl
    {
        private ChunkEditorViewModel viewModel => (ChunkEditorViewModel)DataContext;
        private bool load = false;
        public SelectedObject()
        {
            InitializeComponent();
        }
        public void isLoaded(object sender, RoutedEventArgs e)
        {
            if(load) return; //Prevent double loading
            viewModel.PropertyChanged += PropertyChanged;
            load = true;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.SelectedObject))  // BLOCKINFO CHANGE
            {
                if (viewModel.SelectedObject != null)
                    Im.Source = viewModel.SelectedObject.objectInventoryImage;
                    viewModel.SelectedObject.InventoryImageChanged += updateImage;
            }
        }
        private void updateImage(object sender, PropertyChangedEventArgs e)
        {
            if (IsLoaded)
                Im.Source = viewModel.SelectedObject.objectInventoryImage;
            viewModel.SelectedObject.InventoryImageChanged -= updateImage;
        }
    }
}
