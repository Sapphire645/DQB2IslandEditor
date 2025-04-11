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
        public ChunkBlockGrid()
        {
            InitializeComponent();
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
        
    }
}
