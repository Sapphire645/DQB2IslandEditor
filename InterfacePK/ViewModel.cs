using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.InterfacePK.ChunkEditor;
using DQB2IslandEditor.ObjectPK;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;

namespace DQB2IslandEditor.InterfacePK
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private SaveData saveData;
        private MWindow mWindow;
        public Visibility SaveDataVisibility
        {
            get
            {
                if (saveData == null) return Visibility.Hidden;
                else return Visibility.Visible;
            }
        }
        public Brush ValidCMNDAT { get; set; } = Brushes.Black;
        public string PublicCMNDATPath
        {
            get
            {
                return _cmndatPath;
            }
            set {
                _cmndatPath = value;
                if (!SaveData.ValidCMNDAT(_cmndatPath))
                    ValidCMNDAT = Brushes.Red;
                else
                    ValidCMNDAT = Brushes.Black;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValidCMNDAT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublicCMNDATPath)));
            }
        }
        private string _cmndatPath = "";
        private Dictionary<byte, BitmapSource> minimapGeneratedImages = new Dictionary<byte, BitmapSource>();

        private bool button = false;
        public bool ButtonV { get { return button; } }

        private byte _selectedIsland = 0;
        public byte SelectedIsland
        {
            get { return _selectedIsland; }
            set
            {
                _selectedIsland = value;
                //Change to a thread thing (if I can...)
                CreateMinimap();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIsland)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIslandName)));
            }
        }

        private BitmapSource _currentMinimapBackground; //change
        public BitmapSource CurrentMinimapBackground
        {
            get
            { return _currentMinimapBackground; }
            set
            {
                _currentMinimapBackground = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMinimapBackground)));
            }
        }

        public ImageSource CurrentIslandName
        {
            get
            {
                return DataBaseReading.GetIslandNameImage(_selectedIsland);
            }
        }

        public ViewModel(MWindow mWindow)
        {
            this.mWindow = mWindow;
            DataBaseReading.ReadIslands();
            Console.WriteLine("NORMAL START\n");
            DataBaseReading.InitStaticElements();
        }

        public void SelectedIslandRotate(int movement)
        {
            int newIsland = SelectedIsland;
            do
            {
                newIsland = newIsland + movement;
                //From 01 to 32
                if (newIsland == 33) newIsland = 0;
                else if (newIsland < 0) newIsland = 32;
                //if (newIsland == 0) saveData.ValidSTGDAT(); //Needs more work.
                //else 
            } while (!saveData.ValidSTGDAT((byte)newIsland));
            SelectedIsland = (byte)newIsland;
        }

        private async void CreateMinimap()
        {
            Task<RenderTargetBitmap> TaskCreate = null;
            if (!minimapGeneratedImages.ContainsKey(_selectedIsland))
            {
                TaskCreate = Task.Run(() =>
                {
                    var a = saveData.islandCMNDATdata[_selectedIsland].pMinimap.MinimapImage(2, false);
                    a.Freeze();
                    return a;
                }); 
            }
            if (!minimapGeneratedImages.ContainsKey(_selectedIsland))
            {
                minimapGeneratedImages.Add(_selectedIsland, TaskCreate.Result.Clone());
            }
            CurrentMinimapBackground = minimapGeneratedImages[_selectedIsland];
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            fadeIn.Completed += (s, e) =>
            {
                mWindow.MinimapBackgroundOld.Source = _currentMinimapBackground.Clone();
            };
            mWindow.MinimapBackground.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }
        public void OpenSTGDAT()
        {
            ChunkEditorWindow window = new ChunkEditorWindow(saveData, SelectedIsland);
            window.Show();
        }
        public void OpenCMNDATpath()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "CMNDAT.BIN|CMNDAT.BIN"
                };

                if (openFileDialog.ShowDialog() == false) return;

                if (SaveData.ValidCMNDAT(openFileDialog.FileName))
                {
                    saveData = new SaveData(openFileDialog.FileName);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveDataVisibility)));
                    SelectedIsland = 1;
                    button = true;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonV)));
                }
                PublicCMNDATPath = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message, "Failed to open file", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
