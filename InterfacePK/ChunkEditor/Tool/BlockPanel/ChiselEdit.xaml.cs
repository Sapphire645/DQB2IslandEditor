using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for ChiselEdit.xaml
    /// </summary>
    public partial class ChiselEdit : UserControl
    {
        private ChunkEditorViewModel viewModel;
        private Dictionary<Chisel, BitmapSource> chiselImages = new Dictionary<Chisel, BitmapSource>();
        //what a mess
        public ChiselEdit(ChunkEditorViewModel viewModel)
        {
            this.viewModel = viewModel;
            InitializeComponent();

            if (viewModel.SelectedBlock == null)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
                ChiselImage.Source = chiselImages[viewModel.Chisel];
            }

            viewModel.PropertyChanged += PropertyChanged;
            //store store store
            foreach (UIElement item in ButtonGrid.Children)
            {
                Grid content = (item as Button).Content as Grid;
                foreach (UIElement itm in content.Children)
                {
                    if(itm is Image img)
                    {
                        chiselImages.Add((Chisel)byte.Parse(((Button)item).Tag.ToString()), img.Source as BitmapSource);
                    }
                }
            }
        }
        private void ChangeChisel(object sender, RoutedEventArgs e)
        {
            viewModel.Chisel = (Chisel)byte.Parse(((Button)sender).Tag.ToString());
        }
        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.SelectedBlock))  // BLOCKINFO CHANGE
            {
                if (viewModel.SelectedBlock == null)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    ChiselImage.Source = chiselImages[viewModel.Chisel];
                    TBlock.Text = viewModel.Chisel.ToString().Replace('_',' ');
                }
            }
            if (e.PropertyName == nameof(viewModel.Chisel))  // BLOCKINFO CHANGE
            {
                ChiselImage.Source = chiselImages[viewModel.Chisel];
                TBlock.Text = viewModel.Chisel.ToString().Replace('_', ' ');
            }
        }
    }
}
