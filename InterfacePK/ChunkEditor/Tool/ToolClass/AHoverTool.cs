using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.InterfacePK.ChunkEditor.Map.ChunkView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.ToolClass
{
    //Implements the logic fpr the hover and tile aura.
    public abstract class AHoverTool : ITool
    {
        protected ChunkEditorViewModel viewModel;
        protected byte auraSize => viewModel.auraSize;
        protected bool auraSquare => viewModel.auraSquare;
        protected bool usesAura = true; //Disable by kids.
        protected ChunkBlockGrid chunkDisplays;
        public AHoverTool(ChunkEditorViewModel viewModel, ChunkBlockGrid chunkDisplays) { 
            this.viewModel = viewModel;
            this.chunkDisplays = chunkDisplays;
        }
        protected ushort[] Aura(ushort offset)
        {
            List<ushort> offsets = new List<ushort>();
            offsets.Add(offset);

            // I must check that the offset is inside
            // Ill keep it as 1 chunk for now
            if (usesAura) //I use the aura
            {
                byte x = (byte)(offset % Chunk.X_DIMENSION);
                byte z = (byte)(offset / Chunk.Z_DIMENSION);
                byte minX = (byte)(x - auraSize < 0 ? 0 : x - auraSize);
                byte maxX = (byte)(x + auraSize >= Chunk.X_DIMENSION ? Chunk.X_DIMENSION - 1 : x + auraSize);
                byte minZ = (byte)(z - auraSize < 0 ? 0 : z - auraSize);
                byte maxZ = (byte)(z + auraSize >= Chunk.Z_DIMENSION ? Chunk.Z_DIMENSION - 1 : z + auraSize);
                if (auraSquare)
                    for (byte i = minX; i <= maxX; i++)
                    {
                        for (byte j = minZ; j <= maxZ; j++)
                        {
                            offsets.Add((ushort)(i + (j * Chunk.X_DIMENSION)));
                        }
                    }
                else
                {
                    for (byte i = minX; i <= maxX; i++)
                    {
                        for (byte j = minZ; j <= maxZ; j++)
                        {
                            if (Math.Sqrt(Math.Pow(i - x, 2) + Math.Pow(j - z, 2)) <= auraSize) //idk it autocompleted, yeet
                            {
                                offsets.Add((ushort)(i + (j * Chunk.X_DIMENSION)));
                            }
                        }
                    }
                }
            }
            return offsets.ToArray();
        }
        public virtual void BlockInstance_MouseEnter(ushort offset, ushort chunk)
        {
            chunkDisplays.TileAura_Create(Aura(offset), chunk);
        }
        public void BlockInstance_MouseLeave(ushort offset, ushort chunk)
        {
            chunkDisplays.TileAura_Destroy(Aura(offset), chunk);
        }
        public virtual void BlockInstance_MouseLeftClick(ushort offset, ushort chunk)
        {
            chunkDisplays.Tile_Click(offset, chunk);
        }
        public virtual void BlockInstance_MouseRelease(ushort offset, ushort chunk)
        {
            chunkDisplays.Tile_Unclick(offset, chunk);
        }
        public void ItemInstance_MouseEnter(ushort offset, ushort chunk, byte itemLocationInArray)
        {
            // Default hover behavior for item instance
        }
        public void ItemInstance_MouseLeave(ushort offset, ushort chunk, byte itemLocationInArray)
        {
            // Default leave behavior for item instance
        }
        public void ItemInstance_MouseLeftClick(ushort offset, ushort chunk, byte itemLocationInArray)
        {
            // Default left click behavior for item instance
        }
        public void ItemInstance_MouseRelease(ushort offset, ushort chunk, byte itemLocationInArray)
        {
            // Default release behavior for item instance

        }
    }
}
