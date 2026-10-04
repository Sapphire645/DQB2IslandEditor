using DQB2IslandEditor.DataPK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using static System.Net.WebRequestMethods;

namespace DQB2IslandEditor.ObjectPK
{
    public class FullMapCreator
    {
        public static FullMapCreator fullMapCreator;
        private BitmapSource tileSheet;
        private const int tileSize = 64;
        public FullMapCreator(BitmapSource tileSheet)
        {

            fullMapCreator = this;
            this.tileSheet = tileSheet;
        }

        private BitmapImage loadUri(string path)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path);
            // Or: new Uri("C:/Images/MyImage.png");
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;  // Important if file is disposable
            bitmapImage.EndInit();
            bitmapImage.Freeze();  // Optional performance optimization
            return bitmapImage;
        }

        public WriteableBitmap CreateIdBitmap(string idText)
        {
            int width = 64;
            int height = 64;

            // Create a visual to draw on
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                // Optional: background color
                context.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

                // Draw text
                var formattedText = new FormattedText(
                    idText,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    20,                          // Font size
                    Brushes.Black,              // Text color
                    VisualTreeHelper.GetDpi(visual).PixelsPerDip
                );

                // Center text in 64×64
                double x = (width - formattedText.Width) / 2;
                double y = (height - formattedText.Height) / 2;

                context.DrawText(formattedText, new Point(x, y));
            }

            // Render to bitmap
            var renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(visual);

            // Return as WriteableBitmap (if you want to edit later)
            return new WriteableBitmap(renderBitmap);
        }
        private unsafe void PasteObject(ItemInstance item, WriteableBitmap result)
        {
            BitmapSource overlay;
            try
            {
                overlay = loadUri($"pack://application:,,,/Images/Map/Object/o{item.itemInfo.imageId:D4}.png");
            }
            catch
            {
                overlay = CreateIdBitmap(item.itemInfo.imageId.ToString());
            }
            
            overlay.Freeze();
            var brightness = 0.5f + ((float)item.Y / (float)96) * 0.5f; // 0.5 → 1.0

            // Pointer to background
            IntPtr bgPtr = result.BackBuffer;

            int offsetX = item.X * tileSize;
            int offsetY = item.Z * tileSize;
            //rotate tile
            
                var rotatedOverlay = new TransformedBitmap(overlay, new RotateTransform(90*item.Rotation));
                rotatedOverlay.Freeze();
                overlay = new FormatConvertedBitmap(rotatedOverlay, PixelFormats.Bgra32, null, 0);
                overlay.Freeze();
            //Offset the point if big

            //This does not account for overflowing.....
            if(item.itemInfo.Witdh >1 || item.itemInfo.Depth > 1)
            {
                if (item.Rotation == 0 || item.Rotation ==1)
                    offsetY -= (item.itemInfo.Depth - 1) * tileSize;
                if (item.Rotation == 2 || item.Rotation == 1)
                    offsetX -= (item.itemInfo.Witdh - 1) * tileSize;
            }
            
            // Get buffers
            var overlayPixels = new byte[overlay.PixelWidth * overlay.PixelHeight * 4];
            overlay.CopyPixels(overlayPixels, overlay.PixelWidth * 4, 0);
            try
            {
                for (int y = 0; y < overlay.PixelHeight; y++)
                {
                    for (int x = 0; x < overlay.PixelWidth; x++)
                    {
                        int overlayIndex = (y * overlay.PixelWidth + x) * 4;
                        byte b = overlayPixels[overlayIndex + 0];
                        byte g = overlayPixels[overlayIndex + 1];
                        byte r = overlayPixels[overlayIndex + 2];
                        byte a = overlayPixels[overlayIndex + 3]; // IMPORTANT: alpha channel

                        if (a == 0) continue; // Skip fully transparent pixels

                        int targetX = x + offsetX;
                        int targetY = y + offsetY;

                        if (targetX < 0 || targetX >= result.PixelWidth || targetY < 0 || targetY >= result.PixelHeight)
                            continue;  // Skip out-of-range pixels

                        // Pixel in background
                        IntPtr pixelPtr = bgPtr + targetY * result.BackBufferStride + targetX * 4;
                        int bgPixel = *((int*)pixelPtr);

                        byte bgB = (byte)(bgPixel & 0xFF);
                        byte bgG = (byte)((bgPixel >> 8) & 0xFF);
                        byte bgR = (byte)((bgPixel >> 16) & 0xFF);

                        // Alpha blending
                        double alpha = a / 255.0;
                        byte outB = (byte)(bgB * (1 - alpha) + b * alpha);
                        byte outG = (byte)(bgG * (1 - alpha) + g * alpha);
                        byte outR = (byte)(bgR * (1 - alpha) + r * alpha);

                        // Write back
                        *((int*)pixelPtr) = (a << 24) | (outR << 16) | (outG << 8) | outB;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error pasting object: Overflow?" + ex.Message);
            }

        }
        public static void AlphaBlendBitmap(
                WriteableBitmap background,
                BitmapSource overlay,
                int offsetX,
                int offsetY, float brightness)
        {
            
            
        }
        private unsafe void PasteTile(
            int x, int y,
            byte* srcBase, byte* dstBase,
            int srcStride, int dstStride,
            int baseTileId, byte height,
            bool transparent)
        {
            int srcX = (baseTileId % Chunk.X_DIMENSION) * tileSize;
            int srcY = (baseTileId / Chunk.X_DIMENSION) * tileSize;
            if (baseTileId != 0)
            {
                var brightness = 0.5f + ((float)height / (float)96) * 0.5f; // 0.5 → 1.0

                for (int ty = 0; ty < tileSize; ty++)
                {
                    byte* srcRow = srcBase + (srcY + ty) * srcStride + srcX * 4;
                    byte* dstRow = dstBase + (y * tileSize + ty) * dstStride + x * tileSize * 4;

                    for (int tx = 0; tx < tileSize; tx++)
                    {
                        byte* srcPixel = srcRow + tx * 4;
                        byte* dstPixel = dstRow + tx * 4;

                        if (transparent)
                        {
                            // BGRA
                            dstPixel[0] = (byte)((Math.Clamp(srcPixel[0] * brightness, 0, 255) + dstPixel[0]) / 2); // Blue
                            dstPixel[1] = (byte)((Math.Clamp(srcPixel[1] * brightness, 0, 255) + dstPixel[1]) / 2); // Green
                            dstPixel[2] = (byte)((Math.Clamp(srcPixel[2] * brightness, 0, 255) + dstPixel[2]) / 2); // Red
                            dstPixel[3] = srcPixel[3]; // Alpha unchanged
                        }
                        else
                        {
                            // BGRA
                            dstPixel[0] = (byte)Math.Clamp(srcPixel[0] * brightness, 0, 255); // Blue
                            dstPixel[1] = (byte)Math.Clamp(srcPixel[1] * brightness, 0, 255); // Green
                            dstPixel[2] = (byte)Math.Clamp(srcPixel[2] * brightness, 0, 255); // Red
                            dstPixel[3] = srcPixel[3]; // Alpha unchanged
                        }
                    }
                }
            }
        }
        private unsafe void DrawBorderLeft(
                int x, int y,
                byte* dstBase, int dstStride,
                byte heightMe, byte heightLeft,
                byte borderThickness)
        {
            if (heightMe != heightLeft)
            {
                int sign = 1;
                int off = 0;
                if (heightMe > heightLeft)
                {
                    sign = -1;
                    off = 1;
                }
                for (int ty = 0; ty < tileSize; ty++)
                {
                    for (int bt = 0; bt < borderThickness; bt++)
                    {
                        byte* dstPixel = dstBase + (y * tileSize + ty) * dstStride // Y pos
                                                   + ((x) * tileSize) * 4       // start of my tile
                                                   + ((bt + off) * 4 * sign);                        // border pixels
                        float factor = ((float)bt / (float)borderThickness);
                        dstPixel[0] = (byte)(((dstPixel[0] * factor) + dstPixel[0]) / 2);
                        dstPixel[1] = (byte)(((dstPixel[1] * factor) + dstPixel[1]) / 2);
                        dstPixel[2] = (byte)(((dstPixel[2] * factor) + dstPixel[2]) / 2);
                        dstPixel[3] = 255;                         // full alpha
                    }
                }
            }
        }
        private unsafe void DrawBorderTop(
                int x, int y,
                byte* dstBase, int dstStride,
                byte heightMe, byte heightTop,
                byte borderThickness)
        {
            if (heightMe != heightTop)
            {
                int sign = 1;
                int off = 0;
                if (heightMe > heightTop)
                {
                    sign = -1;
                    off = 1;
                }
                for (int tx = 0; tx < tileSize; tx++)
                {
                    for (int bt = 0; bt < borderThickness; bt++)
                    {
                        byte* dstPixel = dstBase + ((y) * tileSize) * dstStride  // start of my tile
                                                   + ((bt + off) * sign) * dstStride                 // vertical thickness
                                                   + (x * tileSize + tx) * 4;

                        float factor = ((float)bt / (float)borderThickness);
                        dstPixel[0] = (byte)(((dstPixel[0] * factor) + dstPixel[0]) / 2);
                        dstPixel[1] = (byte)(((dstPixel[1] * factor) + dstPixel[1]) / 2);
                        dstPixel[2] = (byte)(((dstPixel[2] * factor) + dstPixel[2]) / 2);
                        dstPixel[3] = 255;                         // full alpha                         // alpha
                    }
                }
            }
        }

        public unsafe void PasteBorders
            (byte[] heightsUnderWater,
            byte[] heightsUnderwaterLeftNeighbour,
            byte[] heightsUnderwaterRightNeighbour,
            byte[] heightsUnderwaterTopNeighbour,
            byte[] heightsUnderwaterBottomNeighbour,
            int x, int y,byte* dstBase, int dstStride,byte heightValue,
            byte borderThickness)
        {
            //Now do the borders.
            // === Draw vertical border (to the left) ===
            if (x != 0)
                DrawBorderLeft(x, y, dstBase, dstStride, heightValue, heightsUnderWater[y * Chunk.X_DIMENSION + x - 1], borderThickness);
            else //Check the other chunk
                if (heightsUnderWater[y * Chunk.X_DIMENSION] < heightsUnderwaterLeftNeighbour[y])
                DrawBorderLeft(x, y, dstBase, dstStride, heightValue, heightsUnderwaterLeftNeighbour[y], borderThickness);

            // === Draw horizontal border (Upwards) ===
            if (y != 0)
                DrawBorderTop(x, y, dstBase, dstStride, heightValue, heightsUnderWater[y * Chunk.X_DIMENSION + x - Chunk.X_DIMENSION], borderThickness);
            else //Check the other chunk
                if (heightsUnderWater[x] < heightsUnderwaterTopNeighbour[x])
                DrawBorderTop(x, y, dstBase, dstStride, heightValue, heightsUnderwaterTopNeighbour[x], borderThickness);

            //Now check if bottom

            // === Draw vertical border (to the right) ===
            if (x + 1 == Chunk.X_DIMENSION && heightsUnderWater[y * Chunk.X_DIMENSION + (Chunk.X_DIMENSION - 1)] < heightsUnderwaterRightNeighbour[y])
                DrawBorderLeft(x + 1, y, dstBase, dstStride, heightsUnderwaterRightNeighbour[y], heightsUnderWater[y * Chunk.X_DIMENSION + (Chunk.X_DIMENSION - 1)], borderThickness);

            // === Draw horizontal border (downwards) ===
            if (y + 1 == Chunk.Z_DIMENSION && heightsUnderWater[(Chunk.X_DIMENSION - 1) * Chunk.X_DIMENSION + x] < heightsUnderwaterBottomNeighbour[x])
                DrawBorderTop(x, y + 1, dstBase, dstStride, heightsUnderwaterBottomNeighbour[x], heightsUnderWater[(Chunk.X_DIMENSION - 1) * Chunk.X_DIMENSION + x], borderThickness);
        }
        // I'll need the neighboring tiles to draw borders
        public WriteableBitmap CreateChunkMapLines(Chunk chunk, 
            byte[] heightsLeftNeighbour, 
            byte[] heightsRightNeighbour,
            byte[] heightsTopNeighbour,
            byte[] heightsBottomNeighbour,
            byte[] heightsUnderwaterLeftNeighbour,
            byte[] heightsUnderwaterRightNeighbour,
            byte[] heightsUnderwaterTopNeighbour,
            byte[] heightsUnderwaterBottomNeighbour,
            byte water)
        {
            if(System.IO.File.Exists(chunk.chunkPosition + ".png"))
            {
                return null;
            }
            // Resulting image dimensions
            WriteableBitmap result = new WriteableBitmap(
                Chunk.X_DIMENSION * tileSize, Chunk.Z_DIMENSION * tileSize, 96, 96,
                System.Windows.Media.PixelFormats.Bgra32, null);

            int srcStride = (tileSheet.PixelWidth * 4);
            int dstStride = result.BackBufferStride;

            byte[] srcPixels = new byte[tileSheet.PixelHeight * srcStride];
            tileSheet.CopyPixels(srcPixels, srcStride, 0);
            byte borderThickness = 8;

            //Get blocks and heights
            var blocks = chunk.GetBlocksFromTopView();
            bool[] isItem = new bool[blocks.Length];
            var heights = chunk.GetHeightsFromTopView();
            int[] tileID =  new int[blocks.Length];

            var blocksUnderWater = chunk.GetBlocksFromTopView(true);
            bool[] isItemUnderwater = new bool[blocks.Length];
            var heightsUnderWater = chunk.GetHeightsFromTopView(true);
            int[] tileIDUnderwater = new int[blocks.Length];

            //Set water in empty areas
            for (int i = 0; i < heights.Length; i++)
            {
                if (heights[i] == 0)
                    heights[i] = water;
                isItem[i] = false;
                isItemUnderwater[i] = false;
                //Get the image tile ID for the base block.
                int imageId = DataBaseReading.BLOCK_INFO_DICTIONARY[blocksUnderWater[i].publicBlockID].imageId;
                //Variance of base block.
                tileIDUnderwater[i] = imageId * 4 + Random.Shared.Next(0, 4);
                //Get the image tile ID for the base block.
                imageId = DataBaseReading.BLOCK_INFO_DICTIONARY[blocks[i].publicBlockID].imageId;
                //Variance of base block.
                tileID[i] = imageId * 4 + Random.Shared.Next(0, 4);


            }
            for (int i = 0; i < heightsLeftNeighbour.Length; i++)
            {
                if (heightsLeftNeighbour[i] == 0)
                    heightsLeftNeighbour[i] = water;
                if (heightsRightNeighbour[i] == 0)
                    heightsRightNeighbour[i] = water;
                if (heightsBottomNeighbour[i] == 0)
                    heightsBottomNeighbour[i] = water;
                if (heightsTopNeighbour[i] == 0)
                    heightsTopNeighbour[i] = water;
            }
            result.Lock();

            var ItemListUnsorted = chunk.GetAllItems();
            //Sort from bottom to top
            //Later on I'll have to trim out the objetcs that are below blocks and other full objects.

            //Transparent items.
            List<ItemInstance> ItemList = new List<ItemInstance>();

            foreach(ItemInstance item in ItemListUnsorted)
            {
                //this is where things like tile per tile would come into play. Not sure yet how to account for that.
                if (item.itemInfo.FullBlock)
                {
                    if(item.Y > heights[item.worldOffset])
                    {
                        heights[item.worldOffset] = (byte)(item.Y);
                        heightsUnderWater[item.worldOffset] = (byte)(item.Y);
                        tileID[item.worldOffset] = ItemListUnsorted.IndexOf(item);
                        tileIDUnderwater[item.worldOffset] = ItemListUnsorted.IndexOf(item);
                        isItem[item.worldOffset] = true;
                        isItemUnderwater[item.worldOffset] = true;
                    }
                    if(item.Y > heightsUnderWater[item.worldOffset])
                    {
                        heightsUnderWater[item.worldOffset] = (byte)(item.Y);
                        tileIDUnderwater[item.worldOffset] = ItemListUnsorted.IndexOf(item);
                        isItemUnderwater[item.worldOffset] = true;
                    }
                    
                }
                else
                {
                    if (item.Y > heightsUnderWater[item.worldOffset])
                    {
                        ItemList.Add(item);
                    }
                }
            }



            unsafe
            {
                byte* dstBase = (byte*)result.BackBuffer;
                fixed (byte* srcBase = srcPixels)
                {
                    //Do entire block layer first.
                    for (int y = 0; y < Chunk.X_DIMENSION; y++)
                    {
                        for (int x = 0; x < Chunk.Z_DIMENSION; x++)
                        {
                            //height
                            int tileIndex = tileIDUnderwater[y * Chunk.X_DIMENSION + x];
                            byte heightValue = heightsUnderWater[y * Chunk.X_DIMENSION + x]; // 0–255
                            //Paste the underwater block.
                            if (isItemUnderwater[y * Chunk.X_DIMENSION + x])
                            {
                                PasteObject(ItemListUnsorted[tileIndex], result);
                            }
                            else
                            {
                                PasteTile(x, y, srcBase, dstBase, srcStride, dstStride, tileIndex, heightValue, false);
                            }
                                

                            PasteBorders(heightsUnderWater, heightsUnderwaterLeftNeighbour, heightsUnderwaterRightNeighbour,
                                heightsUnderwaterTopNeighbour, heightsUnderwaterBottomNeighbour,
                            x, y, dstBase, dstStride, heightValue, borderThickness);

                            
                        }
                    }
                }
                //Do underwater items.
                foreach(var item in ItemList)
                {
                    if(heights[item.worldOffset] > item.Y && heightsUnderWater[item.worldOffset] < item.Y) //Below water
                    {
                        PasteObject(item, result);
                    }
                }

                //Redo to not overwrite water transparency.
                dstBase = (byte*)result.BackBuffer;
                fixed (byte* srcBase = srcPixels)
                {
                    for (int y = 0; y < Chunk.X_DIMENSION; y++)
                    {
                        for (int x = 0; x < Chunk.Z_DIMENSION; x++)
                        {
                            //Check if this is underwater.
                            if (heightsUnderWater[y * Chunk.X_DIMENSION + x] != heights[y * Chunk.X_DIMENSION + x])
                            {  // redo, but water logic.

                                //height
                                int tileIndex = tileID[y * Chunk.X_DIMENSION + x];
                                byte heightValue = heights[y * Chunk.X_DIMENSION + x]; // 0–255
                                                                                                 //Paste the underwater block.
                                if (isItem[y * Chunk.X_DIMENSION + x])
                                {
                                    PasteObject(ItemListUnsorted[tileIndex], result);
                                }
                                else
                                {
                                    PasteTile(x, y, srcBase, dstBase, srcStride, dstStride, tileIndex, heightValue, true);
                                }


                                PasteBorders(heights, heightsLeftNeighbour, heightsRightNeighbour,
                                    heightsTopNeighbour, heightsBottomNeighbour,
                                x, y, dstBase, dstStride, heightValue, borderThickness);

                                
                            }
                        }
                    }
                }
                //Do overwater items.
                foreach (var item in ItemList)
                {
                    if (heights[item.worldOffset] <= item.Y) //Above water
                    {
                        PasteObject(item, result);
                        heights[item.worldOffset] = item.Y;
                    }
                }
            }
            result.AddDirtyRect(new Int32Rect(0, 0, result.PixelWidth, result.PixelHeight));
            result.Unlock();

            SaveWriteableBitmapAsPng(result, chunk.chunkPosition+ ".png");
            return result;
        }

        public void SaveWriteableBitmapAsPng(WriteableBitmap bitmap, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
    }
}
