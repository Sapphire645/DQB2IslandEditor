using DQB2IslandEditor.DataPK;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Xml.Linq;

namespace DQB2IslandEditor.ObjectPK
{
    public static class DataBaseReading
    {
        private const string ISLAND_PATH = "Info/Islands.txt";
        private const string HARDNESS_PATH = "Info/Hardness.txt";

        private const string BLOCK_PATH = "Info/Blocks.txt";
        private const string BLOCK_EXTRA_PATH = "Info/BlockInfo.txt";

        private const string BLOCK_MENU_PATH = "Info/MenuData/TabBlock.txt";
        private const string ITEM_NATURE_MENU_PATH = "Info/MenuData/TabItemN.txt";
        private const string ITEM_ARTIFICIAL_MENU_PATH = "Info/MenuData/TabItemA.txt";

        private const string ITEM_PATH = "Info/Items.txt";

        private const string SHEET_BLOCK_ONE_PATH = "Images/Inventory/BlockSheet.png";
        private const string SHEET_TILE_ONE_PATH = "Images/Inventory/TileSheet.png";

        private const string SHEET_ITEM_ONE_PATH = "Images/Inventory/ItemSheet.png";

        private const string BLOCK_ERR_PATH = "Images/Inventory/B-0001.png";
        private const string TILE_ERR_PATH = "Images/Inventory/T-0001.png";

        private const string SHEET_TOOLS = "Images/UI/toolsheet.png";

        private const string SHEET_MINIMAP_RETRO = "Images/Inventory/SheetRetro.png";
        private const string SHEET_MINIMAP_CHUNKY = "Images/Inventory/SheetChunky.png";

        private const byte BLOCK_SIZE = 112;
        private const byte TILE_SIZE = 64;
        private const byte SHEET_DIMENSION = 32;

        private const byte MINIMAP_RETRO_TILE_SIZE = 16;
        private const byte MINIMAP_CHUNKY_TILE_SIZE = 1;
        private const byte MINIMAP_SHEET_DIMENSION = 32;

        public static Dictionary<uint, BlockInfo> BLOCK_INFO_DICTIONARY;

        public static Dictionary<uint, ItemInfo> ITEM_INFO_DICTIONARY;

        private static readonly Dictionary<byte,byte> _mountain = new Dictionary<byte, byte>()
        {
            { 8, 8 },
            { 9, 10},
            { 10, 9 },
            { 11, 7 },
            { 18, 8 }
        };
        private static string[] _islands = new string[32];

        private static BitmapImage _sheetOneBlock;
        private static BitmapImage _sheetOneTile;

        private static BitmapImage _sheetOneItem;

        private static BitmapImage _toolSheet;
        private static BitmapImage _minimapSheetChunky;
        private static BitmapImage _minimapSheetRetro;

        //its taking forever...
        private static CroppedBitmap[] _minimapDecoratorsRetro = new CroppedBitmap[12];
        private static CroppedBitmap[] _minimapDecoratorsChunky = new CroppedBitmap[12];

        //Store new cropped bitmaps here.
        private static Dictionary<ushort,CroppedBitmap> _minimapStorageChunky = new Dictionary<ushort, CroppedBitmap>();
        //Store new cropped bitmaps here.
        private static Dictionary<ushort, CroppedBitmap> _minimapStorageRetro = new Dictionary<ushort, CroppedBitmap>();

        private const int TOOL_SIZE = 80;

        private static DataBaseImageHandler[] ImageDatabase = new DataBaseImageHandler[4];

        //Hardness
        private static Dictionary<byte, string> Hardness = new Dictionary<byte, string>();

        public static string getHardness(byte key)
        {
            return Hardness.ContainsKey(key) ? Hardness[key] : key.ToString();
        }
        
        public static void InitStaticElements()
        {
            _toolSheet = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_TOOLS));
            _minimapSheetChunky = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_MINIMAP_CHUNKY));
            _minimapSheetRetro = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_MINIMAP_RETRO));

            _minimapSheetChunky.CacheOption = BitmapCacheOption.OnLoad;
            _minimapSheetRetro.CacheOption = BitmapCacheOption.OnLoad;
            for (byte i = 0; i < 11; i++) {
                _minimapDecoratorsChunky[i] = new CroppedBitmap(_minimapSheetChunky, //decorator
                    new Int32Rect(MINIMAP_CHUNKY_TILE_SIZE * (i + 1), (MINIMAP_SHEET_DIMENSION - 1) * MINIMAP_CHUNKY_TILE_SIZE,
                    MINIMAP_CHUNKY_TILE_SIZE, MINIMAP_CHUNKY_TILE_SIZE));
                _minimapDecoratorsChunky[i].Freeze();
            }
            _minimapDecoratorsChunky[11] = new CroppedBitmap(_minimapSheetChunky, //decorator
                new Int32Rect(0, (MINIMAP_SHEET_DIMENSION - 1) * MINIMAP_CHUNKY_TILE_SIZE,
                MINIMAP_CHUNKY_TILE_SIZE, MINIMAP_CHUNKY_TILE_SIZE));
            _minimapDecoratorsChunky[11].Freeze();
            for (byte i = 0; i < 11; i++)
            {
                _minimapDecoratorsRetro[i] = new CroppedBitmap(_minimapSheetRetro, //decorator
                    new Int32Rect( MINIMAP_RETRO_TILE_SIZE * (i + 1), (MINIMAP_SHEET_DIMENSION - 1) * MINIMAP_RETRO_TILE_SIZE,
                    MINIMAP_RETRO_TILE_SIZE, MINIMAP_RETRO_TILE_SIZE));
                _minimapDecoratorsRetro[i].Freeze();
            }
            _minimapDecoratorsRetro[11] = new CroppedBitmap(_minimapSheetRetro, //decorator
                new Int32Rect(0 , (MINIMAP_SHEET_DIMENSION - 1) * MINIMAP_RETRO_TILE_SIZE,
                MINIMAP_RETRO_TILE_SIZE, MINIMAP_RETRO_TILE_SIZE));
            _minimapDecoratorsRetro[11].Freeze();
            _minimapSheetChunky.Freeze();
            _minimapSheetRetro.Freeze();

            //Loads sheets
            _sheetOneBlock = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_BLOCK_ONE_PATH));
            _sheetOneTile = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_TILE_ONE_PATH));

            _sheetOneItem = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_ITEM_ONE_PATH));

            //Helps performance
            _sheetOneBlock.Freeze();
            _sheetOneTile.Freeze();

            _sheetOneItem.Freeze();

            ImageDatabase[0] = new DataBaseImageHandler(_sheetOneBlock, BLOCK_SIZE, BLOCK_ERR_PATH, SHEET_DIMENSION, false);
            ImageDatabase[1] = new DataBaseImageHandler(_sheetOneTile, TILE_SIZE, TILE_ERR_PATH, SHEET_DIMENSION, true);
            ImageDatabase[2] = new DataBaseImageHandler(_sheetOneItem, BLOCK_SIZE, BLOCK_ERR_PATH, SHEET_DIMENSION, false);
            //Placeholder.
            ImageDatabase[3] = new DataBaseImageHandler(_sheetOneItem, BLOCK_SIZE, TILE_ERR_PATH, SHEET_DIMENSION, true);
            ReadHardness();
        }
        public static CroppedBitmap toolImage(byte tool, bool active)
        {
            return new CroppedBitmap(_toolSheet,
                new Int32Rect(TOOL_SIZE * tool, active ? TOOL_SIZE : 0, TOOL_SIZE, TOOL_SIZE));
        }
        public static void ReadIslands()
        {
            IEnumerable<string> islandFile = System.IO.File.ReadLines(ISLAND_PATH);
            foreach (var Line in islandFile)
            {
                if (Line[0] == '#') continue;
                String[] values = Line.Split('\t');
                if (values.Length < 2) continue;
                _islands[short.Parse(values[0])] = values[1];
            }
        }

        private static void ReadHardness()
        {
            if (!System.IO.File.Exists(HARDNESS_PATH)) return;

            var blockLines = System.IO.File.ReadAllLines(HARDNESS_PATH);
            foreach (String line in blockLines)
            {
                if (line[0] == '#') continue;
                //ID      hardness

                String[] values = line.Split('\t');
                if (values.Length < 2) continue;

                Hardness.Add(byte.Parse(values[0]), values[1]);
            }
        }
        public static string GetIslandName(byte island)
        {
            return _islands[island];
        }

        public static ImageSource ValueChiselImage(Chisel chisel)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Images/Chisel/{(byte)chisel:00}.png"));
        }
        public static ImageSource GetInventoryIcon(byte type)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Images/Inventory/icon{(byte)type:0}.png"));
        }

        public static ImageSource GetIslandNameImage(byte island)
        {
            try
            {
                return new BitmapImage(new Uri($"pack://application:,,,/Images/Islands/{island:00}.png"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static float GetTileSize(bool chunky)
        {
            if (chunky) return MINIMAP_CHUNKY_TILE_SIZE;
            else return MINIMAP_RETRO_TILE_SIZE;
            
        }
        public static CroppedBitmap[] GetTileImage(MinimapTile tile, byte explored, bool chunky)
        { //0 covered, 1 half seen, 2 invisible
            CroppedBitmap[] layers = new CroppedBitmap[3];
            byte size = chunky ? MINIMAP_CHUNKY_TILE_SIZE : MINIMAP_RETRO_TILE_SIZE;
            //BASE
            if (chunky)
            { //Try to cut on lines
                if (explored != 0 || tile.Explored)
                { //If either its explored or explored is set to true
                  //Return the normal tile, set by "type".
                    if (_minimapStorageChunky.ContainsKey(tile.Type))
                    {
                        layers[0] = _minimapStorageChunky[tile.Type];
                    }
                    else
                    {
                        layers[0] = new CroppedBitmap(_minimapSheetChunky,
                                  new Int32Rect((tile.Type % MINIMAP_SHEET_DIMENSION) * size, (tile.Type / MINIMAP_SHEET_DIMENSION) * size, size, size));
                        _minimapStorageChunky[tile.Type] = layers[0];
                        _minimapStorageChunky[tile.Type].Freeze();
                    }
                    if (tile.Decorator != 0)
                        layers[1] = _minimapDecoratorsChunky[tile.Decorator];
                    else if (tile.Height && _mountain.ContainsKey(tile.ByteType))
                        layers[1] = _minimapDecoratorsChunky[_mountain[tile.ByteType]];
                }
                if (explored != 2 && !tile.Explored) //If not explored and showing not exploder tiles
                    layers[2] = _minimapDecoratorsChunky[11]; //The not explored overlay
            }
            else
            {
                if (explored != 0 || tile.Explored)
                { //If either its explored or explored is set to true
                  //Return the normal tile, set by "type".
                    if (_minimapStorageRetro.ContainsKey(tile.Type))
                    {
                        layers[0] = _minimapStorageRetro[tile.Type];
                    }
                    else
                    {
                        try
                        {
                            layers[0] = new CroppedBitmap(_minimapSheetRetro,
                                  new Int32Rect((tile.Type % MINIMAP_SHEET_DIMENSION) * size, (tile.Type / MINIMAP_SHEET_DIMENSION) * size, size, size));
                        }
                        catch
                        {
                            Console.WriteLine("Error: " + tile.Type);
                            layers[0] = new CroppedBitmap(_minimapSheetRetro,
                                  new Int32Rect(0, 0, size, size));
                        }
                        _minimapStorageRetro[tile.Type] = layers[0];
                        _minimapStorageRetro[tile.Type].Freeze();
                    }
                    if (tile.Decorator != 0)
                        layers[1] = _minimapDecoratorsRetro[tile.Decorator];
                    else if (tile.Height && _mountain.ContainsKey(tile.ByteType))
                        layers[1] = _minimapDecoratorsRetro[_mountain[tile.ByteType]];
                }
                if (explored != 2 && !tile.Explored) //If not explored and showing not exploder tiles
                    layers[2] = _minimapDecoratorsRetro[11]; //The not explored overlay
            }
                return layers;
        }
        public static async void GetObjectInventoryImage(ObjectInfo parent)
        {
            CroppedBitmap blockIcon;
            if (parent is BlockInfo)
                ImageDatabase[0].GetObjectImage(parent);
            else
                ImageDatabase[2].GetObjectImage(parent);

        }
        public static async void GetObjectMapImage(ObjectInfo parent)
        {
            CroppedBitmap blockIcon;
            if (parent is BlockInfo)
                ImageDatabase[1].GetObjectImage(parent);
            else
                ImageDatabase[3].GetObjectImage(parent); 
        }

        public static void ReadBlockFile()
        {
            var blockList = new Dictionary<uint, BlockInfo>();
            if (!System.IO.File.Exists(BLOCK_EXTRA_PATH)) return; //Crash lmao

            String[] blockLines = ReadEmbeddedResource(BLOCK_PATH).Split("\n");

            foreach (String line in blockLines)
            {
                if (line[0] == '#') continue; //This is comment (python comment go brrr)
                                              //FORMAT:

                //ID      Image Id       LiquidTab?     Tab      Color     Name

                String[] values = line.Split('\t');
                if (values.Length < 6) continue;

                var ImageID = short.Parse(values[1]);

                blockList.Add(ushort.Parse(values[0]), new BlockInfo(
                    ushort.Parse(values[0]), ImageID, byte.Parse(values[2]),
                    (Colour)byte.Parse(values[3]), byte.Parse(values[4]), values[5].Trim()));

            }

            if (System.IO.File.Exists(BLOCK_EXTRA_PATH)) //This will be editable for translation purposes hopefully.
            {
                blockLines = System.IO.File.ReadAllLines(BLOCK_EXTRA_PATH);
                foreach (String line in blockLines)
                {
                    if (line[0] == '#') continue; //This is comment (python comment go brrr)
                                                  //FORMAT:

                    //ID      hardness      normaldrop    ultdrop      desc

                    String[] values = line.Split('\t');
                    if (values.Length < 6) continue;

                    blockList[uint.Parse(values[0])].UpdateExtraData(values[1], values[2], values[3], values[4], values[5]);
                }
            }
            BLOCK_INFO_DICTIONARY =  blockList;
        }
        public static void ReadItemFile()
        {
            var itemList = new Dictionary<uint, ItemInfo>();
            
            if (!System.IO.File.Exists(BLOCK_EXTRA_PATH)) return; //Change

            String[] itemLines = ReadEmbeddedResource(ITEM_PATH).Split("\n");

            foreach (String line in itemLines)
            {
                if (line[0] == '#') continue; //This is comment (python comment go brrr)
                                              //FORMAT:

                //ID      Image Id       Dimension     Tab      Color     Name

                String[] values = line.Split('\t');
                if (values.Length < 6) continue;

                var ImageID = short.Parse(values[1]);

                itemList.Add(ushort.Parse(values[0]), new ItemInfo(
                    ushort.Parse(values[0]), ImageID, values[2], byte.Parse(values[3]),
                    (Colour)byte.Parse(values[4]), values[5].Trim()));
            }
            ITEM_INFO_DICTIONARY = itemList;
        }

        public static IDictionary<byte, object> BlockMenuData()
        {
            var blockList = new Dictionary<byte, object>();

            var NoSubmenuList = new List<uint>();
            var ColourSubmenu = new Dictionary<uint, List<uint>>();
            var LiquidSubmenu = new Dictionary<uint, List<uint>>();
            var AirSubmenu = new List<uint>();
            var GeneralSubmenu = new Dictionary<uint, List<uint>>();

            blockList.Add(0, NoSubmenuList);
            blockList.Add(1, ColourSubmenu);
            blockList.Add(2, LiquidSubmenu);
            blockList.Add(3, AirSubmenu);
            blockList.Add(4, GeneralSubmenu);

            String[] blockLines = ReadEmbeddedResource(BLOCK_MENU_PATH).Split("\n");
            foreach (String line in blockLines)
            {
                if (line.Length < 1 || line[0] == '#') continue;
                String[] values = line.Split('\t');
                if (values.Length < 2) continue;
                byte typeOfSubmenu = byte.Parse(values[0]);
                switch (typeOfSubmenu)
                {
                    case 0:
                        NoSubmenuList.Add(uint.Parse(values[1]));
                        break;
                    case 1:
                        String[] colours = values[2].Split(',');
                        var lis = new List<uint>();
                        foreach (String colour in colours)
                        {
                            lis.Add(uint.Parse(colour));
                        }
                        ColourSubmenu.Add(uint.Parse(values[1]), lis);
                        break;
                    case 2:
                        String[] liquid = values[2].Split(',');
                        var lisT = new List<uint>();
                        foreach (String liqui in liquid)
                        {
                            lisT.Add(uint.Parse(liqui));
                        }
                        LiquidSubmenu.Add(uint.Parse(values[1]), lisT);
                        break;
                    case 3:
                        foreach (String air in values[1].Split(','))
                        {
                            AirSubmenu.Add(uint.Parse(air));
                        }
                        break;
                    case 4:
                        String[] general = values[2].Split(',');
                        var lisGeneral = new List<uint>();
                        foreach (String gen in general)
                        {
                            lisGeneral.Add(uint.Parse(gen));
                        }
                        GeneralSubmenu.Add(uint.Parse(values[1]), lisGeneral);
                        break;
                }
            }
            return blockList;
        }

        public static IDictionary<byte, object> ItemAMenuData()
        {
            return ItemMenuData(ITEM_ARTIFICIAL_MENU_PATH);
        }
        public static IDictionary<byte, object> ItemNMenuData()
        {
            return ItemMenuData(ITEM_NATURE_MENU_PATH);
        }
        public static IDictionary<byte, object> ItemMenuData(string path)
        {
            var blockList = new Dictionary<byte, object>();

            var NoSubmenuList = new List<uint>();
            var ColourSubmenu = new Dictionary<uint, List<uint>>();
            var GeneralSubmenu = new Dictionary<uint, List<uint>>();
            var CropSubmenu = new Dictionary<uint, List<uint>>();

            blockList.Add(0, NoSubmenuList);
            blockList.Add(1, ColourSubmenu);
            blockList.Add(4, GeneralSubmenu);
            blockList.Add(5, CropSubmenu);

            String[] blockLines = ReadEmbeddedResource(path).Split("\n");
            foreach (String line in blockLines)
            {
                if (line.Length < 1 || line[0] == '#') continue;
                String[] values = line.Split('\t');
                if (values.Length < 2) continue;
                byte typeOfSubmenu = byte.Parse(values[0]);
                switch (typeOfSubmenu)
                {
                    case 0:
                        NoSubmenuList.Add(uint.Parse(values[1]));
                        break;
                    case 1:
                        String[] colours = values[2].Split(',');
                        var lis = new List<uint>();
                        foreach (String colour in colours)
                        {
                            lis.Add(uint.Parse(colour));
                        }
                        ColourSubmenu.Add(uint.Parse(values[1]), lis);
                        break;
                    case 4:
                        String[] general = values[2].Split(',');
                        var lisGeneral = new List<uint>();
                        foreach (String gen in general)
                        {
                            lisGeneral.Add(uint.Parse(gen));
                        }
                        GeneralSubmenu.Add(uint.Parse(values[1]), lisGeneral);
                        break;
                    case 5:
                        String[] crop = values[2].Split(',');
                        var lisCrop = new List<uint>();
                        foreach (String gen in crop)
                        {
                            lisCrop.Add(uint.Parse(gen));
                        }
                        CropSubmenu.Add(uint.Parse(values[1]), lisCrop);
                        break;
                }
            }
            return blockList;
        }

        //Pdfadrf whatever
        private static string ReadEmbeddedResource(string path)
        {
            Uri resourceUri = new Uri(path, UriKind.Relative);
            StreamResourceInfo resourceInfo = Application.GetResourceStream(resourceUri);

            if (resourceInfo != null)
            {
                using (StreamReader reader = new StreamReader(resourceInfo.Stream))
                {
                    return reader.ReadToEnd();
                }
            }
            return string.Empty;
        }
    }
}
