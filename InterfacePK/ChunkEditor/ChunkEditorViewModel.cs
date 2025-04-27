using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.InterfacePK.ChunkEditor;
using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DQB2IslandEditor.InterfacePK
{
    public class ChunkEditorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangedEventHandler? LayerUpdateNotification;
        public event PropertyChangedEventHandler? ChunkUpdateNotification;

        private SaveData saveData;
        public Island CurrentIsland => saveData.Island;
        private ChunkEditorWindow chunkEditorWindow;

        //Colective Layer value.

        public void UpdatedChunks()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentChunks)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentChunk)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentChunkX)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentChunkY)));
            ChunkUpdateNotification?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentChunks)));
        }
        public void ChangeChunk(ushort vChunk)
        {
            chunkEditorWindow.chunkBlockGrid.SetChunk(vChunk);
        }
        private byte _currentLayer;
        public byte CurrentLayer
        {
            get
            {
                return _currentLayer;
            }
            set
            {
                if (value > 96) return;
                _currentLayer = value;
                LayerUpdateNotification?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLayer)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLayer)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SeaBrush)));
            }
        }
        public ushort[] CurrentChunks => chunkEditorWindow.chunkBlockGrid.ChunkList;
        public ushort CurrentChunk => chunkEditorWindow.chunkBlockGrid.FirstChunk;
        public int CurrentChunkX => CurrentChunk % Island.GRID_DIMENSION;
        public int CurrentChunkY => CurrentChunk / Island.GRID_DIMENSION;


        //Colective hud stuffs
        private bool _showMinimapGrid = true;
        private bool _showFullGrid = false;
      
        public bool ShowMinimapGrid { get { return _showMinimapGrid; } set {
                _showMinimapGrid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowMinimapGrid)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowMinimapGridVisibility)));
            }
        }
        public bool ShowFullGrid
        {
            get { return _showFullGrid; }
            set { 
                _showFullGrid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowFullGrid)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowFullGridVisibility)));
            }
        }
        public Brush SeaBrush
        {
            get { return _currentLayer > 30 ? Brushes.Black : Brushes.DarkBlue; }
        }
        public Visibility ShowMinimapGridVisibility { get { return ShowMinimapGrid ? Visibility.Visible : Visibility.Hidden; } }
        public Visibility ShowFullGridVisibility { get { return ShowFullGrid ? Visibility.Visible : Visibility.Hidden; } }

        //TOOL HANDELING:

        private byte _selectedTool;

        private byte _paintToolSize = 1;

        public byte SelectedTool { get { return _selectedTool; }
            set {
                _selectedTool = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTool)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedToolImage)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedToolName)));
            }
        }

        public byte PaintToolSize
        {
            get { return _paintToolSize; }
            set
            {
                _paintToolSize = value;
                SelectedTool = 2;
            }
        }

        //Tool display.
        public string SelectedToolName => _selectedTool == 0 ? "Select" : _selectedTool == 1 ? "Area" : _selectedTool == 2 ? "Paint" : "Chisel";
        public ImageSource SelectedToolImage => DataBaseReading.toolImage(SelectedTool, true);
        public ImageSource ToolImageZero => DataBaseReading.toolImage(0, false);
        public ImageSource ToolImageOne => DataBaseReading.toolImage(1, false);
        public ImageSource ToolImageTwo => DataBaseReading.toolImage(2, false);
        public ImageSource ToolImageThree => DataBaseReading.toolImage(3, false);


        //----------------------------------------------------------------------------------------------------------------

        //Selected Tile:

        private ushort _selectedTileChunk;
        private ushort _selectedTileLayer;
        private ushort _selectedTileOffset;


        private ObjectInfo _selectedObject;

        public ObjectInfo SelectedObject
        {
            set
            {
                _selectedObject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedObject)));
            }
            get { 
            
                return _selectedObject;
            }
        }
        public ChunkEditorViewModel(ChunkEditorWindow mainWindow, SaveData saveData) {
            this.chunkEditorWindow = mainWindow;
            this.saveData = saveData;
            Console.WriteLine("Created View Model");
        }
        public void CreateInventory()
        {
            IDictionary<uint, ObjectInfo> objectDic = DataBaseReading.BLOCK_INFO_DICTIONARY.ToDictionary(kvp => kvp.Key, kvp => (ObjectInfo)kvp.Value);
            chunkEditorWindow.inventoryMenu.CreateInventory(objectDic, chunkEditorWindow);

            chunkEditorWindow.selectedObject.SelButton.Click += (_, _) => { UpdateSelectedObject(_selectedObject); };
            chunkEditorWindow.selectedObject.SelButton.MouseRightButtonDown += (_, _) => { UpdateFavouriteObject(_selectedObject); };
        }

        //Change selected object -> Selecting anything on the inventory
        public void UpdateSelectedObject(ObjectInfo selectedObject)
        {
            SelectedObject = selectedObject;
        }
        public void BlockTile_LeftClick(ushort offset, ushort chunk)
        {
            /*
            chunkLayer[offset].IsClicked();
            switch (SelectedTool)
            {
                case 0: //Select
                    //This is what happens when you click a block.
                    SelectedObject = DataBaseReading.BLOCK_INFO_DICTIONARY[blockID];
                    SelectedTileOffset = offset;
                    //Block
                    ValueChisel = chunkLayer[offset].blockInstance.publicChiselID;
                    ValueBuilderPlaced = chunkLayer[offset].blockInstance.publicBuilderPlaced;
                    break;
                case 1: //Area
                    //Do area calculation (for future...)
                    break;
                case 2: //Paint (change for size stuff)
                    //Update tile
                    TileAura_SetBlock(offset);
                    break;
                case 3: //Chisel
                    chunkLayer[offset].blockInstance.UpdateBlock(ValueChisel);
                    saveData.Island.SetBlock(_currentChunk, _currentLayer, (byte)(offset % Chunk.X_DIMENSION), (byte)(offset / Chunk.Z_DIMENSION), chunkLayer[offset].blockInstance);
                    break;

            }
                        */
        }

        public void BlockTile_TileEnter(ushort offset, ushort chunk)
        {
            /*
            switch (SelectedTool)
            {
                case 2: //Paint (change for size stuff)
                        //Handle bigger pencil
                    TileAura_Create(offset);
                    if (Mouse.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
                    {
                        chunkLayer[offset].IsHovered();
                        return;
                    }
                    chunkLayer[offset].IsClicked();
                    TileAura_SetBlock(offset);
                    
                    break;
                default:
                    if (Mouse.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
                        chunkLayer[offset].IsHovered();
                    else
                        chunkLayer[offset].IsClicked();
                    break;
            }
            */
        }

        public void BlockTile_TileLeave(ushort offset, ushort chunk)
        {
            /*
            switch (SelectedTool)
            {
                case 2:
                    TileAura_Destroy(offset);
                    chunkLayer[offset].IsUnHovered();
                    break;
                default:
                    chunkLayer[offset].IsUnHovered();
                    break;
            }
            */
        }

        public void BlockTile_Release(ushort offset, ushort chunk)
        {
            /*
             chunkLayer[offset].IsHovered();
            */
        }
        public void ItemTile_TileEnter(ItemContainer target)
        {
            //This will actually be a problem for compatibility between paint block and paint item
            //Oh no, what do you mean I should have used the state pattern oh noo
            //If you dont say anything then I wont say anything either wink wink
            switch (SelectedTool)
            {
                default:
                    if (Mouse.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
                        target.IsHovered();
                    else
                        target.IsClicked();
                    break;
            }
        }
        public void ItemTile_TileLeave(ItemContainer target)
        {
            switch (SelectedTool)
            {
                default:
                    target.IsUnHovered();
                    break;
            }
        }

        public void ItemTile_LeftClick(ItemContainer target)
        {
            /*
            target.IsClicked();
            switch (SelectedTool)
            {
                default:
                case 0: //Select
                    //When the items are hoverable, this is when the block underneath will always be selected to the xyz.
                    //Or, at least the xz
                    SelectedObject = DataBaseReading.ITEM_INFO_DICTIONARY[0];
                    SelectedTileOffset = target.offset;
                    //Block (Hmmmmm)
                    ValueChisel = chunkLayer[target.offset].blockInstance.publicChiselID;
                    ValueBuilderPlaced = chunkLayer[target.offset].blockInstance.publicBuilderPlaced;
                    break;
            }
            */
        }
        public void ItemTile_Release(ItemContainer target)
        {
            target.IsHovered();
        }


        //Update favourite -> Right clicking anything on the inventory, right click dropicking a chunk tile 
        public void UpdateFavouriteObject(ObjectInfo selectedObject)
        {
            chunkEditorWindow.favouriteList.UpdateFavourite(selectedObject); //Update favourite
        }
        
    }
}
