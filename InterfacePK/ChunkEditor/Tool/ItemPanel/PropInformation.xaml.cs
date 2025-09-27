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

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.ItemPanel
{
    /// <summary>
    /// Interaction logic for InstanceInformation.xaml
    /// </summary>
    public partial class PropInformation : UserControl
    {
        ChunkEditorViewModel ViewModel;
        public PropInformation(ChunkEditorViewModel viewModelGive)
        {
            InitializeComponent();
            ViewModel = viewModelGive;
            ViewModel.PropertyChanged += PropertyChanged;

            if (ViewModel.SelectedItem == null)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.SelectedItem))  // BLOCKINFO CHANGE
            {
                if (ViewModel.SelectedItem == null)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    ImageBlock.Source = ViewModel.SelectedItem.objectInventoryImage;
                    ViewModel.SelectedItem.InventoryImageChanged += updateImage;
                }
            }
        }
        private void updateImage(object sender, PropertyChangedEventArgs e)
        {
            if (IsLoaded)
                ImageBlock.Source = ViewModel.SelectedItem.objectInventoryImage;
            ViewModel.SelectedItem.InventoryImageChanged -= updateImage;
        }
    }
}
