using DQB2IslandEditor.DataPK;
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

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Map
{
    /// <summary>
    /// Interaction logic for VirtualGridView.xaml
    /// </summary>
    public partial class VirtualGridView : UserControl
    {
        private ChunkEditorViewModel viewModel;
        private Border selectedchunk;
        private Dictionary<ushort, Border> allChunks;
        public VirtualGridView()
        {
            InitializeComponent();
        }

        public void Init(Island island, ChunkEditorViewModel ViewModel)
        {
            allChunks = new Dictionary<ushort, Border>();
            this.viewModel = ViewModel;
            var dimensions = island.ChunkGridFrame();

            if(dimensions.X0 > 1) dimensions.X0 -= 1;
            if(dimensions.Y0 > 1) dimensions.Y0 -= 1;
            if(dimensions.X1 < Island.GRID_DIMENSION-1) dimensions.X1 += 1;
            if(dimensions.Y1 < Island.GRID_DIMENSION-1) dimensions.Y1 += 1;
            //Set the grid to the size of the island
            ChunkGridButtons.Columns = (int)(dimensions.X1 - dimensions.X0 + 1);
            ChunkGridButtons.Rows = (int)(dimensions.Y1 - dimensions.Y0 + 1);

            for (uint y = dimensions.Y0; y <= dimensions.Y1; y++)
                {
                for (uint x = dimensions.X0; x <= dimensions.X1; x++)
                {
                    if (island.IsChunkEmpty((ushort)(x + (64 * y))))
                    {
                        Border empty = new Border()
                        {
                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0555555")),
                            BorderBrush = new SolidColorBrush(Colors.Gray),
                            BorderThickness = new Thickness(1),
                            Tag = (ushort)1000
                        };
                        ChunkGridButtons.Children.Add(empty);
                        allChunks[(ushort)(x + (64 * y))] = empty;
                    }
                    else
                    {
                        Border chunk = new Border()
                        {
                            Background = new SolidColorBrush(Colors.Transparent),
                            BorderBrush = new SolidColorBrush(Colors.Gray),
                            BorderThickness = new Thickness(1),
                            Tag = (ushort)(x + (64 * y))
                        };
                        chunk.MouseEnter += (s, e) => MouseEnterChunk(chunk);
                        chunk.MouseLeave += (s, e) => MouseLeaveChunk(chunk);
                        chunk.MouseLeftButtonUp += (s, e) => MouseClickChunk(chunk);
                        ChunkGridButtons.Children.Add(chunk);
                        allChunks[(ushort)(x + (64 * y))] = chunk;
                    }


                }
            }
            //Scale for minimap
            //I think i need the sum
            if (dimensions.X1 < Island.GRID_DIMENSION - 1) dimensions.X1 += 1;
            if (dimensions.Y1 < Island.GRID_DIMENSION - 1) dimensions.Y1 += 1;

            dimensions.X0 = dimensions.X0 * Minimap.MINIMAP_DIMENSION_IN_CHUNK;
            dimensions.X1 = dimensions.X1 * Minimap.MINIMAP_DIMENSION_IN_CHUNK;
            dimensions.Y0 = dimensions.Y0 * Minimap.MINIMAP_DIMENSION_IN_CHUNK;
            dimensions.Y1 = dimensions.Y1 * Minimap.MINIMAP_DIMENSION_IN_CHUNK;
            MinimapImage.Source = island.shell.pMinimap.MinimapImage(2, false, dimensions);
        }

        private void MouseEnterChunk(Border chunk)
        {
            //Set the color of the border
            chunk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55FF9000"));
            chunk.BorderBrush = new SolidColorBrush(Colors.Gold);
        }
        private void MouseLeaveChunk(Border chunk)
        {
            //Set the color of the border
            if(chunk == selectedchunk)
            {
                chunk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55FF4000"));
                chunk.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
            }
            else
            {
                chunk.Background = new SolidColorBrush(Colors.Transparent);
                chunk.BorderBrush = new SolidColorBrush(Colors.Gray);
            }
                
        }
        private void MouseClickChunk(Border chunk)
        {
            //Get the chunk number
            ushort chunkNumber = (ushort)chunk.Tag;
            //Set the chunk data to the selected one
            viewModel.CurrentChunk = chunkNumber;
            //Set the color of the border
            chunk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55FF9000"));
            chunk.BorderBrush = new SolidColorBrush(Colors.Yellow);
        }

        public void UpdateChunk(ushort vChunk)
        {
            if (selectedchunk != null)
            {
                if ((ushort)selectedchunk.Tag != 1000)
                {
                    selectedchunk.Background = new SolidColorBrush(Colors.Transparent);
                    selectedchunk.BorderBrush = new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    selectedchunk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0555555"));
                    selectedchunk.BorderBrush = new SolidColorBrush(Colors.Gray);
                }
            }
            if(allChunks != null)
            {
                if (allChunks.ContainsKey(vChunk))
                {
                    selectedchunk = allChunks[vChunk];
                    selectedchunk.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55FF4000"));
                    selectedchunk.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
                }
                else
                {
                    selectedchunk = null;
                }
               
            }
        }
    }
}
