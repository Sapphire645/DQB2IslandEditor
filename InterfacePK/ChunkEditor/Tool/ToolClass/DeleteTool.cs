using DQB2IslandEditor.InterfacePK.ChunkEditor.Map.ChunkView;
using DQB2IslandEditor.ObjectPK.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.ToolClass
{
    internal class DeleteTool : AHoverTool
    {
        private bool isPainting = false;
        public DeleteTool(ChunkEditorViewModel viewModel, ChunkBlockGrid chunkDisplays) : base(viewModel, chunkDisplays)
        {
            usesAura = false;
        }

        public override void BlockInstance_MouseLeftClick(ushort offset, ushort chunk)
        {
            isPainting = true; //I paint now.
            base.BlockInstance_MouseLeftClick(offset, chunk);

            //chunkDisplays.TileAura_SetBlock(Aura(offset), viewModel.currentBlockInstance, chunk);
        }
        public override void BlockInstance_MouseRelease(ushort offset, ushort chunk)
        {
            base.BlockInstance_MouseRelease(offset, chunk);
            base.BlockInstance_MouseLeave(offset, chunk);
            isPainting = false;
        }
        public override void BlockInstance_MouseEnter(ushort offset, ushort chunk)
        {

        }
        public override void BlockInstance_MouseLeave(ushort offset, ushort chunk)
        {
            base.BlockInstance_MouseLeave(offset, chunk);
        }

        public override void ItemInstance_MouseLeftClick(ItemContainer itemInstance, ushort chunk)
        {
            isPainting = true; //I paint now.

            base.ItemInstance_MouseLeftClick(itemInstance, chunk);
            chunkDisplays.DeleteItem(itemInstance.itemInstance,chunk);
        }

        public override void ItemInstance_MouseEnter(ItemContainer itemInstance, ushort chunk)
        {
            if(isPainting)
                chunkDisplays.DeleteItem(itemInstance.itemInstance, chunk);
            base.ItemInstance_MouseLeftClick(itemInstance, chunk);

        }
        public override void ChunkViewLostFocus()
        {
            isPainting = false;
        }
    }
}