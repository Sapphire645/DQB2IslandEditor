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

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.BlockPanel
{
    /// <summary>
    /// Interaction logic for BlockInformation.xaml
    /// </summary>
    public partial class BlockInformation : UserControl
    {

        ChunkEditorViewModel ViewModel;
        public BlockInformation(ChunkEditorViewModel viewModelGive)
        {
            InitializeComponent();
            ViewModel = viewModelGive;
            ViewModel.PropertyChanged += PropertyChanged;

            if (ViewModel.SelectedBlock == null)
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
            if (e.PropertyName == nameof(ViewModel.SelectedBlock))  // BLOCKINFO CHANGE
            {
                if (ViewModel.SelectedBlock == null)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
