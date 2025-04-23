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
        private const string BLOCK_PATH = "Info/Blocks.txt";
        private const string BLOCK_EXTRA_PATH = "Info/BlockExtra.txt";

        private const string BLOCK_PARITY_PATH = "Info/BlockParity.txt";

        private const string SHEET_BLOCK_ONE_PATH = "Images/Inventory/BlockSheetOne.png";
        private const string SHEET_BLOCK_TWO_PATH = "Images/Inventory/BlockSheetTwo.png";
        private const string SHEET_TILE_ONE_PATH = "Images/Inventory/TileSheetOne.png";
        private const string SHEET_TILE_TWO_PATH = "Images/Inventory/TileSheetTwo.png";

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
        private static BitmapImage _sheetTwoBlock;
        private static BitmapImage _sheetOneTile;
        private static BitmapImage _sheetTwoTile;

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
        public static string GetIslandName(byte island)
        {
            return _islands[island];
        }

        public static ImageSource ValueChiselImage(Chisel chisel)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Images/Chisel/{(byte)chisel:00}.png"));
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


        public static void ReadBlockFile()
        {
            //Loads sheets
            _sheetOneBlock = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_BLOCK_ONE_PATH));
            _sheetTwoBlock = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_BLOCK_TWO_PATH));
            _sheetOneTile = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_TILE_ONE_PATH));
            _sheetTwoTile = new BitmapImage(new Uri("pack://application:,,,/" + SHEET_TILE_TWO_PATH));

            //Helps performance
            _sheetOneBlock.Freeze();
            _sheetTwoBlock.Freeze();
            _sheetOneTile.Freeze();
            _sheetTwoTile.Freeze();

            var blockList = new Dictionary<uint, BlockInfo>();
            if (!System.IO.File.Exists(BLOCK_EXTRA_PATH)) return; //Crash lmao

            String[] blockLines = ReadEmbeddedResource(BLOCK_PATH).Split("\n");
            CroppedBitmap blockIcon;
            CroppedBitmap tileIcon;

            foreach (String line in blockLines)
            {
                if (line[0] == '#') continue; //This is comment (python comment go brrr)
                                              //FORMAT:

                //ID      Image Id       LiquidTab?     Tab      Color     Name

                String[] values = line.Split('\t');
                if (values.Length < 6) continue;

                var ImageID = short.Parse(values[1]);
                try
                {
                    if (ImageID < SHEET_DIMENSION * SHEET_DIMENSION)  //Do I create all images here or do I get them dinamically? Ponder
                    {
                        blockIcon = new CroppedBitmap(_sheetOneBlock, new Int32Rect((ImageID % SHEET_DIMENSION) * BLOCK_SIZE, (ImageID / SHEET_DIMENSION) * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE));
                        tileIcon = new CroppedBitmap(_sheetOneTile, new Int32Rect((ImageID % SHEET_DIMENSION) * TILE_SIZE, (ImageID / SHEET_DIMENSION) * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                    }
                    else
                    {
                        var ImageIDTemp = ImageID % (SHEET_DIMENSION * SHEET_DIMENSION);
                        blockIcon = new CroppedBitmap(_sheetTwoBlock, new Int32Rect((ImageIDTemp % SHEET_DIMENSION) * BLOCK_SIZE, (ImageIDTemp / SHEET_DIMENSION) * BLOCK_SIZE, BLOCK_SIZE, BLOCK_SIZE));
                        tileIcon = new CroppedBitmap(_sheetTwoTile, new Int32Rect((ImageIDTemp % SHEET_DIMENSION) * TILE_SIZE, (ImageIDTemp / SHEET_DIMENSION) * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(ImageID);
                    blockIcon = new CroppedBitmap(new BitmapImage(new Uri("pack://application:,,,/" + BLOCK_ERR_PATH)), new Int32Rect(0, 0, BLOCK_SIZE, BLOCK_SIZE));
                    tileIcon = new CroppedBitmap(new BitmapImage(new Uri("pack://application:,,,/" + TILE_ERR_PATH)), new Int32Rect(0, 0, TILE_SIZE, TILE_SIZE));
                }
                blockIcon.Freeze();
                tileIcon.Freeze();

                blockList.Add(ushort.Parse(values[0]), new BlockInfo(
                    ushort.Parse(values[0]), ImageID,
                    values[2] == "1", byte.Parse(values[3]),
                    (Colour)byte.Parse(values[4]), values[5].Trim(),
                    blockIcon, tileIcon));

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
                    if (values.Length < 5) continue;

                    blockList[uint.Parse(values[0])].UpdateExtraData(values[1], values[2], values[3], "This is a block, holy shit.", values[4]);
                }
            }
            BLOCK_INFO_DICTIONARY =  blockList;
        }
        public static void ReadItemFile()
        {
            var itemList = new Dictionary<uint, ItemInfo>();
            //I'll do a placeholder item for now.
            itemList.Add(0, new ItemInfo(0,0,0,Colour.Plain,"Placeholder", 
                new BitmapImage(new Uri("pack://application:,,,/Images/Inventory/placeholder.png")),
                new BitmapImage(new Uri("pack://application:,,,/Images/Inventory/placeholder.png"))
                ));
            ITEM_INFO_DICTIONARY = itemList;
        }

        public static List<(uint, uint)> BlockParity()
        {
            var blockList = new List<(uint, uint)>();
            String[] blockLines = ReadEmbeddedResource(BLOCK_PARITY_PATH).Split("\n");
            foreach (String line in blockLines)
            {
                if (line.Length < 1 || line[0] == '#') continue;
                String[] values = line.Split('\t');
                if (values.Length < 2) continue;
                uint ID = uint.Parse(values[0]);
                foreach (String id in values[1].Split(','))
                {
                    blockList.Add((uint.Parse(id), ID));
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
