using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.InterfacePK;
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

namespace DQB2IslandEditor.ObjectPK.Container
{
    /// <summary>
    /// Interaction logic for TileContainer.xaml
    /// </summary>
    public partial class TileContainer : UserControl
    {
        public BlockInstance blockInstance { get; set; }

        public ObservableProperty<BlockInfo> blockInfo { get; set; } = new ObservableProperty<BlockInfo> { };

        public ushort offset;
        public TileContainer()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void IsHovered() {
            Tile.Background = Brushes.DarkOrange;
            Image.Opacity = 0.5;
        }
        public void IsUnHovered()
        {
            Tile.Background = Brushes.Transparent;
            Image.Opacity = 1;
        }
        public void IsClicked()
        {
            Tile.Background = Brushes.Yellow;
            Image.Opacity = 0.5;
        }
    }
}
