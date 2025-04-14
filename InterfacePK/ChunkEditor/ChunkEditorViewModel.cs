using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.InterfacePK.ChunkEditor;
using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DQB2IslandEditor.InterfacePK
{
    public class ChunkEditorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private SaveData saveData;

        private ChunkEditorWindow chunkEditorWindow;
        private TileContainer[] chunkLayer;

        private ObjectInfo _selectedObject = null;

        private BlockInfo _selectedBlock = null;
        private ItemInfo _selectedItem = null;

        //Extender values
        private Chisel _editValuesChisel;
        private bool _editValuesBuilderPlaced;
        public Chisel ValueChisel
        {
            get
            { return _editValuesChisel;  }
            set
            { _editValuesChisel = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueChisel)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueChiselImage)));
            }
        }
        public string ValueChiselImage
        {
            get { return $"/Images/Chisel/{(byte)_editValuesChisel:00}.png"; }
        }
            
        public bool ValueBuilderPlaced
        {
            get
            { return _editValuesBuilderPlaced; }
            set
            { _editValuesBuilderPlaced = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueBuilderPlaced))); }
        }

        private bool _showMinimapGrid = true;
        private bool _showFullGrid = false;

        //Chunk displayData
        private ushort _currentChunk = 0;
        private byte _currentLayer = 0;

        public ushort CurrentChunk { get {
                return _currentChunk;
            }
            set {
                _currentChunk = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentChunk)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentChunkX)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentChunkY)));
                LoadLayer(_currentLayer, _currentChunk);
                chunkEditorWindow.virtualGridView.UpdateChunk(_currentChunk);
            } 
        }
        public byte CurrentLayer
        {
            get
            { return _currentLayer;}
            set
            {
                if (value < 0 || value >= Chunk.Y_DIMENSION) return;
                _currentLayer = value;
                chunkEditorWindow.LayerBar.LayerChange(_currentLayer);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLayer)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SeaBrush)));
                //chunkEditorWindow.chunkBlockGrid.LayerBar.LayerChange(_currentLayer);
                LoadLayer(_currentLayer, _currentChunk);
            }
        }
        public byte CurrentChunkX => (byte)(_currentChunk % Island.GRID_DIMENSION);
        public byte CurrentChunkY => (byte)(_currentChunk / Island.GRID_DIMENSION);
        public Dictionary<uint, BlockInfo> fullBlockList { get; set; }
        public ObjectInfo SelectedObject { get {
                return _selectedObject;
            } set {
                _selectedObject = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedObject)));
            } 
        }
        public BlockInfo SelectedBlock
        {
            get
            {
                return _selectedBlock;
            }
            set
            {
                _selectedBlock = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedBlock)));
            }
        }
        public ItemInfo SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }
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
        public string SelectedToolName => _selectedTool == 0 ? "Select" : _selectedTool == 1 ? "Area" : _selectedTool == 2 ? "Paint" : "Chisel";
        public ImageSource SelectedToolImage => DataBaseReading.toolImage(SelectedTool, true);
        public ImageSource ToolImageZero => DataBaseReading.toolImage(0, false);
        public ImageSource ToolImageOne => DataBaseReading.toolImage(1, false);
        public ImageSource ToolImageTwo => DataBaseReading.toolImage(2, false);
        public ImageSource ToolImageThree => DataBaseReading.toolImage(3, false);


        public ChunkEditorViewModel(ChunkEditorWindow mainWindow, SaveData saveData) {
            this.chunkEditorWindow = mainWindow;
            this.saveData = saveData;

            fullBlockList = DataBaseReading.ReadBlockFile("a"); //Temporal

            Console.WriteLine("0");
        }
        public void CreateInventory()
        {
            IDictionary<uint, ObjectInfo> objectDic = fullBlockList.ToDictionary(kvp => kvp.Key, kvp => (ObjectInfo)kvp.Value);
            chunkEditorWindow.inventoryMenu.CreateInventory(objectDic);

            chunkEditorWindow.selectedObject.SelButton.Click += (_, _) => { UpdateSelectedObject(_selectedObject); };
            chunkEditorWindow.selectedObject.SelButton.MouseRightButtonDown += (_, _) => { UpdateFavouriteObject(_selectedObject); };
        }

        //Get the tile containers from the grid
        public void SetChunkTileContainers(TileContainer[] chunkLayer)
        {
            this.chunkLayer = chunkLayer;
            foreach(var tile in chunkLayer)
            {
                tile.Tile.PreviewMouseDown += (_, _) => { BlockTile_LeftClick(tile.offset, tile.blockInstance.publicBlockID); };
                tile.Tile.MouseEnter += (_, _) => { BlockTile_TileEnter(tile.offset); };
                tile.Tile.MouseLeave += (_, _) => { BlockTile_TileLeave(tile.offset); };
                tile.Tile.MouseLeftButtonUp += (_, _) => { BlockTile_Release(tile.offset); };
            }
           
        }

        //Change selected object -> Selecting anything on the inventory, dropicking a chunk tile
        public void UpdateSelectedObject(ObjectInfo selectedObject)
        {
            SelectedObject = selectedObject;
            if(selectedObject is BlockInfo)
                SelectedBlock = selectedObject as BlockInfo;
            else
                SelectedItem = selectedObject as ItemInfo;
        }
        public void BlockTile_LeftClick(ushort offset, uint blockID)
        {
            chunkLayer[offset].IsClicked();
            switch (SelectedTool)
            {
                case 0: //Select
                    SelectedObject = fullBlockList[blockID];
                    SelectedBlock = fullBlockList[blockID];
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
           
        }
        public void BlockTile_TileEnter(ushort offset)
        {
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
        }

        public void BlockTile_TileLeave(ushort offset)
        {
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
        }

        public void BlockTile_Release(ushort offset)
        {
             chunkLayer[offset].IsHovered();
        }

        private void TileAura_Create(ushort offset)
        {
            if(_paintToolSize > 0)
            {
                if (offset % 32 != 0) //Left
                    chunkLayer[offset - 1].IsHovered();
                if (offset % 32 != 31) //Right
                    chunkLayer[offset + 1].IsHovered();
                if (offset / 32 != 0) //Up
                    chunkLayer[offset - 32].IsHovered();
                if (offset / 32 != 31) //Down
                    chunkLayer[offset + 32].IsHovered();
            }
            if(_paintToolSize > 1)
            {
                if (offset % 32 != 0) //Left
                {
                    if (offset / 32 != 0) //Up
                        chunkLayer[offset - 33].IsHovered();
                    if (offset / 32 != 31) //Down
                        chunkLayer[offset + 31].IsHovered();
                }
                if (offset % 32 != 31) //right
                {
                    if (offset / 32 != 0) //Up
                        chunkLayer[offset - 31].IsHovered();
                    if (offset / 32 != 31) //Down
                        chunkLayer[offset + 33].IsHovered();
                }
            }

        }
        private void TileAura_SetBlock(ushort offset)
        {
            Offset_SetBlock(offset);
            if (_paintToolSize > 0)
            {
                if (offset % 32 != 0)
                    Offset_SetBlock((ushort)(offset - 1));
                if (offset % 32 != 31)
                    Offset_SetBlock((ushort)(offset + 1));
                if (offset / 32 != 0)
                    Offset_SetBlock((ushort)(offset - 32));
                if (offset / 32 != 31)
                    Offset_SetBlock((ushort)(offset + 32));
            }
            if (_paintToolSize > 1)
            {
                if (offset % 32 != 0) //Left
                {
                    if (offset / 32 != 0) //Up
                        Offset_SetBlock((ushort)(offset - 33));
                    if (offset / 32 != 31) //Down
                        Offset_SetBlock((ushort)(offset +31));
                }
                if (offset % 32 != 31) //right
                {
                    if (offset / 32 != 0) //Up
                        Offset_SetBlock((ushort)(offset - 31));
                    if (offset / 32 != 31) //Down
                        Offset_SetBlock((ushort)(offset + 33));
                }
            }
           
           
        }
        private void Offset_SetBlock(ushort offset)
        {
            if (SelectedBlock == null) return;
            chunkLayer[offset].blockInfo.Value = SelectedBlock;
            chunkLayer[offset].blockInstance.UpdateBlock(ValueBuilderPlaced, SelectedBlock.objectId, ValueChisel);
            saveData.Island.SetBlock(_currentChunk, _currentLayer, (byte)(offset % Chunk.X_DIMENSION), (byte)(offset / Chunk.Z_DIMENSION), chunkLayer[offset].blockInstance);
        }

        private void TileAura_Destroy(ushort offset)
        {
            if (_paintToolSize > 0)
            {
                if (offset % 32 != 0) //Left
                    chunkLayer[offset - 1].IsUnHovered();
                if (offset % 32 != 31) //Right
                    chunkLayer[offset + 1].IsUnHovered();
                if (offset / 32 != 0) //Up
                    chunkLayer[offset - 32].IsUnHovered();
                if (offset / 32 != 31) //Down
                    chunkLayer[offset + 32].IsUnHovered();
            }
            if (_paintToolSize > 1)
            {
                if (offset % 32 != 0) //Left
                {
                    if (offset / 32 != 0) //Up
                        chunkLayer[offset - 33].IsUnHovered();
                    if (offset / 32 != 31) //Down
                        chunkLayer[offset + 31].IsUnHovered();
                }
                if (offset % 32 != 31) //right
                {
                    if (offset / 32 != 0) //Up
                        chunkLayer[offset - 31].IsUnHovered();
                    if (offset / 32 != 31) //Down
                        chunkLayer[offset + 33].IsUnHovered();
                }
            }
        }
        //Update favourite -> Right clicking anything on the inventory, right click dropicking a chunk tile 
        public void UpdateFavouriteObject(ObjectInfo selectedObject)
        {
            chunkEditorWindow.favouriteList.UpdateFavourite(selectedObject); //Update favourite
        }
        //Refresh Chunk
        public void LoadLayer(byte layer, ushort vChunk)
        {
            var blockInstances = saveData.Island.GetBlocksFromLayer(vChunk, layer);
            for (int i = 0; i < 1024; i++)
            {
                chunkLayer[i].blockInstance = blockInstances[i];
                chunkLayer[i].blockInfo.Value = fullBlockList[blockInstances[i].publicBlockID];
            }
        }

        public void ChangeChunk(string command)
        {
            ushort newChunk = _currentChunk;
            if (command.Contains("l")) //Left
                newChunk -= 1;
            if (command.Contains("r")) //Right
                newChunk += 1;
            if (command.Contains("u")) //Up
                newChunk -= Island.GRID_DIMENSION;
            if (command.Contains("d")) //Down
                newChunk += Island.GRID_DIMENSION;

            if (newChunk < 0 || newChunk >= Island.GRID_DIMENSION * Island.GRID_DIMENSION) return; //Invalid

            CurrentChunk = newChunk; //Update
        }
        
    }
}
