﻿using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.ObjectPK;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Net.WebSockets;
using Microsoft.Win32;
using System.ComponentModel;
using DQB2IslandEditor.InterfacePK.ChunkEditor.Map;
using System.Windows.Media.Animation;
using System.Windows.Media;
using DQB2IslandEditor.ObjectPK.Container;


namespace DQB2IslandEditor.InterfacePK.ChunkEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChunkEditorWindow : Window
    {
        private ChunkEditorViewModel viewModel;
        private SaveData saveData;
        public ChunkEditorWindow(SaveData saveData, byte island)
        {
            Task<ushort> readFileTask = Task.Run(() => ReadFile(saveData, island));
            Console.WriteLine("CREATING VIEW MODEL\n");
            viewModel = new ChunkEditorViewModel(this, saveData);
            viewModel.SelectedTool = 0;

            this.DataContext = viewModel;
            this.saveData = saveData;
            Console.WriteLine("CREATING WINDOW\n");

            InitializeComponent();
            inventoryMenu.shareViewModel(viewModel);
            favouriteList.shareViewModel(viewModel);
            toolMenu.UpdateContext(viewModel);

            Console.WriteLine("CREATING INVENTORY\n");
            viewModel.SetChunkTileContainers(chunkBlockGrid.CreateTiles(viewModel));
            viewModel.CreateInventory();
            Console.WriteLine("WAIT FOR ASYNC\n");
            viewModel.CurrentChunk = readFileTask.Result;
            virtualGridView.Init(saveData.Island, viewModel);

            LayerBar.UpdateSeaLevel(30); //Change
        }
        public async Task<ushort> ReadFile(SaveData saveData, byte island)
        {
            Console.WriteLine("ASYNC START\n");
            Task loadSaveFile = Task.Run(() => saveData.OpenSTGDATCompressedFile(island));
            await loadSaveFile;
            Task<ushort> loadFirstChunk = Task.Run(() => saveData.Island.GetFirstChunk());
            Console.WriteLine("ASYNC END\n");
            return await loadFirstChunk;
        }

        private void RecalculateSize(object sender, SizeChangedEventArgs e)
        {
            if(inventoryMenu != null)
            { 
                Console.WriteLine("RESIZE:"+ e.NewSize + "\n");
                inventoryMenu.Resize(inventoryMenu.ActualHeight, inventoryMenu.ActualWidth);
                LayerBar.Resize();
            }
        }

        private void OpenSTGDAT(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "STGDAT.BIN|STGDAT*.BIN|AUTOSTGDAT.BIN|AUTOSTGDAT.BIN|Backup File|*STGDAT*.BIN"
                };

                if (openFileDialog.ShowDialog() == false) return;

                saveData.OpenSTGDATCompressedFile(openFileDialog.FileName);
                viewModel.CurrentChunk = saveData.Island.GetFirstChunk();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message, "Failed to open file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveSTGDAT(object sender, RoutedEventArgs e)
        {
            try
            {
                SavedText.Opacity = 1;
                SavedText.Text = "Saving...";
                SavedText.Foreground = Brushes.Gold;
                Task save = Task.Run(() => SaveSTGDATAsync(saveData.Island.STGDATPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message, "Failed to save file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveSTGDATAs(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "STGDAT.BIN|STGDAT*.BIN|AUTOSTGDAT.BIN|AUTOSTGDAT.BIN",
                    FileName = "STGDAT" + saveData.Island.islandNumber.ToString("D2") + ".BIN",

                };

                if (saveFileDialog.ShowDialog() == false) return;
                SavedText.Opacity = 1;
                SavedText.Text = "Saving...";
                SavedText.Foreground = Brushes.Gold;
                Task save = Task.Run(() => SaveSTGDATAsync(saveFileDialog.FileName));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message, "Failed to save file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveSTGDATAsync(string path)
        {
            //I think this should wooork....
            saveData.SaveSTGDATCompressedFile(path);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SavedText.Text = "Island Saved!";
                SavedText.Foreground = Brushes.Lime;
                var fadeIn = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    BeginTime = TimeSpan.FromSeconds(2),
                    Duration = TimeSpan.FromSeconds(2),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                SavedText.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }));
        }

        private void ToolClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            viewModel.SelectedTool = byte.Parse((button.Content as Image).Name.Split("_").Last());
        }

        private void ToolChange1(object sender, RoutedEventArgs e)
        {
            viewModel.PaintToolSize = 0;
            PaintPopup.IsOpen = false;
        }
        private void ToolChange2(object sender, RoutedEventArgs e)
        {
            viewModel.PaintToolSize = 1;
            PaintPopup.IsOpen = false;
        }
        private void ToolChange3(object sender, RoutedEventArgs e)
        {
            viewModel.PaintToolSize = 2;
            PaintPopup.IsOpen = false;
        }
    }
}