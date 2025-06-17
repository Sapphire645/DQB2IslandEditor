using DQB2IslandEditor.InterfacePK.ChunkEditor.Map.ChunkView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.ToolClass
{
    public class TrowelTool : AHoverTool
    {
        private bool isPainting = false;
        public TrowelTool(ChunkEditorViewModel viewModel, ChunkBlockGrid chunkDisplays) : base(viewModel, chunkDisplays)
        {
            usesAura = true; 
        }

        public override void BlockInstance_MouseLeftClick(ushort offset, ushort chunk)
        {
            isPainting = true; //I paint now.
            base.BlockInstance_MouseLeftClick(offset, chunk);

            chunkDisplays.TileAura_SetBlock(Aura(offset),viewModel.currentBlockInstance,chunk);
        }

        public override void BlockInstance_MouseEnter(ushort offset, ushort chunk)
        {
            base.BlockInstance_MouseEnter(offset, chunk);
            if (isPainting)
            { //If I am painting then I paint.
                chunkDisplays.TileAura_SetBlock(Aura(offset), viewModel.currentBlockInstance, chunk);
                base.BlockInstance_MouseLeftClick(offset, chunk);
            }

        }
        public override void BlockInstance_MouseRelease(ushort offset, ushort chunk)
        {
            base.BlockInstance_MouseRelease(offset, chunk);
            isPainting = false;
        }
    }
}
