using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        private List<ChunkView> chunkViews = new List<ChunkView>();

        public ushort[] ChunkList
        {
            get
            {
                var li = new ushort[chunkViews.Count];
                for(byte i = 0; i < chunkViews.Count; i++) {
                    li[i] = chunkViews[i].ChunkIndex;
                }
                return li;
            }
            private set{
                //I wont check here, I'll check down.
                for (byte i = 0; i < chunkViews.Count; i++)
                {
                    if(chunkViews[i].ChunkIndex != value[i])
                        chunkViews[i].ChunkIndex = value[i];
                }
                viewModel.UpdatedChunks();
            }
        }

        public ushort FirstChunk => chunkViews[0].ChunkIndex;
        //Get delegated idiot
        private Action<ItemContainer>? enter;
        private Action<ItemContainer>? leave;
        private Action<ItemContainer>? click;
        private Action<ItemContainer>? release;
        public ChunkBlockGrid()
        {
            InitializeComponent();
        }

        public void ShareViewModel(ChunkEditorViewModel viewModel)
        {
            this.viewModel = viewModel;
            var cv = new ChunkView(viewModel);
            chunkViews.Add(cv);
            ChunkDisplay.Children.Add(cv);
        }

        public void SetChunk(ushort vChunk)
        {
            ChunkList = ChunkChangeProcess((short)(vChunk - chunkViews[0].ChunkIndex), ChunkList);
        }
        private void ChangeChunk(object sender, RoutedEventArgs e)
        {
            string command = (sender as Button).Tag.ToString();

            ChangeChunk(command);
        }

        private void ChangeChunk(string command)
        {
            var chunkIdList = ChunkList;

            short Movement = 0;

            if (command.Contains("l")) //Left
                chunkIdList = ChunkChangeProcess(-1, chunkIdList);
            if (command.Contains("r")) //Right
                chunkIdList = ChunkChangeProcess(1, chunkIdList);
            if (command.Contains("u")) //Up
                chunkIdList = ChunkChangeProcess((short)-Island.GRID_DIMENSION, chunkIdList);
            if (command.Contains("d")) //Down
                chunkIdList = ChunkChangeProcess(Island.GRID_DIMENSION, chunkIdList);

            ChunkList = chunkIdList;
        }

        //This is to check, if one is invalid then the next one gets porcessed.
        private ushort[] ChunkChangeProcess(short movement, ushort[] list)
        {
            ushort[] newList = (ushort[])list.Clone();
            for(byte i = 0; i < newList.Length; i++)
            {
                if ((short)newList[i] + movement < 0) return list;
                newList[i] = (ushort)(newList[i] + movement);
                if (newList[i] >= Island.GRID_DIMENSION * Island.GRID_DIMENSION) return list; //Invalid
            }
            return newList;
        }

        private void ChangeLayer(object sender, RoutedEventArgs e)
        {
            viewModel.CurrentLayer = (byte)(viewModel.CurrentLayer + short.Parse((sender as Button).Tag.ToString()));
        }

        //For more intuitive movement.
        private void ScrollUpdate(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
            {
                Point localPosition = e.GetPosition(this);
                Point relativePosition = new Point((localPosition.X / ActualWidth) - 0.5, (localPosition.Y / ActualHeight) - 0.5);
                //If the difference is too low then dont move.
                if (Math.Abs(Math.Abs(relativePosition.X) - Math.Abs(relativePosition.Y)) < 0.08) return;

                //Change for later since the scrolling will be smoother.
                if (Math.Abs(relativePosition.X) > Math.Abs(relativePosition.Y))
                    if (e.Delta > 0)
                        ChangeChunk("r");
                    else
                        ChangeChunk("l");
                else
                    if (e.Delta > 0)
                        ChangeChunk("u");
                    else
                        ChangeChunk("d");
            }
        }




        public void TileAura_Create(ushort[] offsets, ushort chunk)
        {
            for (byte i = 0; i < chunkViews.Count; i++)
            {
                if(chunkViews[i].ChunkIndex == chunk) chunkViews[i].TileAura_Create(offsets);
            }
        }
        public void TileAura_SetBlock(ushort[] offsets, BlockInstance bi, ushort chunk)
        {
            for (byte i = 0; i < chunkViews.Count; i++)
            {
                if (chunkViews[i].ChunkIndex == chunk) chunkViews[i].TileAura_SetBlock(offsets, bi);
            }
        }

        public void TileAura_Destroy(ushort[] offsets, ushort chunk)
        {
            for (byte i = 0; i < chunkViews.Count; i++)
            {
                if (chunkViews[i].ChunkIndex == chunk) chunkViews[i].TileAura_Destroy(offsets);
            }
        }

        public void Tile_Click(ushort offset, ushort chunk)
        {
            for (byte i = 0; i < chunkViews.Count; i++)
            {
                if (chunkViews[i].ChunkIndex == chunk) chunkViews[i].Tile_Click(offset);
            }
        }
        public void Tile_Unclick(ushort offset, ushort chunk)
        {
            for (byte i = 0; i < chunkViews.Count; i++)
            {
                if (chunkViews[i].ChunkIndex == chunk) chunkViews[i].Tile_Unclick(offset);
            }
        }

        public BlockInfo GetBlockInfo(ushort offset, ushort chunk)
        {
            for (byte i = 0; i < chunkViews.Count; i++)
            {
                if (chunkViews[i].ChunkIndex == chunk) return chunkViews[i].GetBlockInfoInOffset(offset);
            }
            return null;
        }
    }
}
