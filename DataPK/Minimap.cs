using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DQB2IslandEditor.DataPK
{
    public class Minimap
    {
        public static readonly ushort MINIMAP_DIMENSION = 256;
        public static readonly byte MINIMAP_DIMENSION_IN_CHUNK = 8;
        public static readonly byte TILE_SIZE = 2;
        private MinimapTile[] tiles = new MinimapTile[MINIMAP_DIMENSION* MINIMAP_DIMENSION];
        public Minimap(byte[] minimapBytes) { 
            for(int i = 0; i < MINIMAP_DIMENSION*MINIMAP_DIMENSION; i++)
            {
                tiles[i] = new MinimapTile(minimapBytes[i*2], minimapBytes[i * 2 +1]);
            }
        }

        private (ushort, ushort, ushort, ushort) CalculateLimit()
        {
            ushort ChunkUpCoord = 256, chunkDownCoord = 0, chunkLeftCoord = 256, chunkRightCoord = 0;
            for (int i = MINIMAP_DIMENSION_IN_CHUNK; i < MINIMAP_DIMENSION; i+= MINIMAP_DIMENSION_IN_CHUNK) //Skip this because it has garbage data somatimes.
            {
                for (int j = 0; j < MINIMAP_DIMENSION; j+= MINIMAP_DIMENSION_IN_CHUNK)
                {
                    //Per chunk.
                    for(byte x = 0; x < MINIMAP_DIMENSION_IN_CHUNK; x++)
                    {
                        for (byte y = 0; y < MINIMAP_DIMENSION_IN_CHUNK; y++)
                        {
                            if (!tiles[(i + x) * MINIMAP_DIMENSION + j + y].IsEmpty())
                            {
                                if (i < ChunkUpCoord) ChunkUpCoord = (byte)i;
                                if (i > chunkDownCoord) chunkDownCoord = (byte)i;
                                if (j < chunkLeftCoord) chunkLeftCoord = (byte)j;
                                if (j > chunkRightCoord) chunkRightCoord = (byte)j;
                                x = MINIMAP_DIMENSION_IN_CHUNK; //for break
                                y = MINIMAP_DIMENSION_IN_CHUNK;
                            }
                        }
                    }

                }
            }
            return (ChunkUpCoord, (ushort)(chunkDownCoord+8), chunkLeftCoord, (ushort)(chunkRightCoord+8));
        }
        public RenderTargetBitmap MinimapImage(byte explored, bool chunky, bool limit = true) //0 covered, 1 half seen, 2 invisible
        {
            (ushort, ushort, ushort, ushort) tileLimits = (0,256,0,256);
            if (limit) tileLimits = CalculateLimit();
            float size = DataBaseReading.GetTileSize(chunky);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                for (ushort i = tileLimits.Item1; i < tileLimits.Item2; i++)
                {
                    for (int j = tileLimits.Item3; j < tileLimits.Item4; j++)
                    {
                        CroppedBitmap[] tile = DataBaseReading.GetTileImage(tiles[i * MINIMAP_DIMENSION + j], explored, chunky);
                        foreach(var t in tile)
                        {
                            if(t == null) continue;
                            context.DrawImage(t, new Rect((j - tileLimits.Item3) * size, (i- tileLimits.Item1) * size, size, size));
                        }
                    }
                }
            }
            RenderTargetBitmap image = new RenderTargetBitmap((int)((tileLimits.Item4-tileLimits.Item3) * size), (int)((tileLimits.Item2 - tileLimits.Item1) * size), 96, 96, PixelFormats.Pbgra32);
            image.Render(drawingVisual);
            return image;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < MINIMAP_DIMENSION; i++)
            {
                for (int j = 0; j < MINIMAP_DIMENSION; j++)
                {
                    str.Append(tiles[i* MINIMAP_DIMENSION + j ].Type + "|" + tiles[i * MINIMAP_DIMENSION + j].Decorator + (tiles[i * MINIMAP_DIMENSION + j].Height ? "^" : "_") + (tiles[i * MINIMAP_DIMENSION + j].Explored ? " " : "X" ) +" ");
                }
                str.Append("\n");
            }
            return str.ToString();
        }

    }
}
