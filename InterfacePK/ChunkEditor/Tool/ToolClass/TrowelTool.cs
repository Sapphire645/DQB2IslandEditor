using DQB2IslandEditor.InterfacePK.ChunkEditor.Map.ChunkView;
using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
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
            if(viewModel.SelectedObject is ItemInfo && viewModel.SelectedItem != null)
            {
                chunkDisplays.TileAura_SetItem(Aura(offset), viewModel.SelectedItem, chunk);
            }
        }

        public override void BlockInstance_MouseEnter(ushort offset, ushort chunk)
        {
            base.BlockInstance_MouseEnter(offset, chunk);
            if (isPainting)
            { //If I am painting then I paint.
                chunkDisplays.TileAura_SetBlock(Aura(offset), viewModel.currentBlockInstance, chunk);
                base.BlockInstance_MouseLeftClick(offset, chunk);
                if (viewModel.SelectedObject is ItemInfo && viewModel.SelectedItem != null)
                {
                    chunkDisplays.TileAura_SetItem(Aura(offset), viewModel.SelectedItem, chunk);
                }
            }
        }
        public override void BlockInstance_MouseRelease(ushort offset, ushort chunk)
        {
            base.BlockInstance_MouseRelease(offset, chunk);
            isPainting = false;
        }


        public override void ItemInstance_MouseLeftClick(ItemContainer item, ushort chunk)
        {
            ushort offset = item.itemInstance.worldOffset;
            isPainting = true; //I paint now.
            base.ItemInstance_MouseLeftClick(item, chunk);

            chunkDisplays.TileAura_SetBlock(Aura(offset), viewModel.currentBlockInstance, chunk);
            if (viewModel.SelectedObject is ItemInfo && viewModel.SelectedItem != null)
            {
                chunkDisplays.TileAura_SetItem(Aura(offset), viewModel.SelectedItem, chunk);
            }
        }

        public override void ItemInstance_MouseEnter(ItemContainer item, ushort chunk)
        {
            ushort offset = item.itemInstance.worldOffset;
            base.ItemInstance_MouseEnter(item, chunk);
            if (isPainting)
            { //If I am painting then I paint.
                chunkDisplays.TileAura_SetBlock(Aura(offset), viewModel.currentBlockInstance, chunk);
                base.BlockInstance_MouseLeftClick(offset, chunk);
                if (viewModel.SelectedObject is ItemInfo && viewModel.SelectedItem != null)
                {
                    chunkDisplays.TileAura_SetItem(Aura(offset), viewModel.SelectedItem, chunk);
                }
            }
        }
        public override void ItemInstance_MouseRelease(ItemContainer item, ushort chunk)
        {
            ushort offset = item.itemInstance.worldOffset;
            base.ItemInstance_MouseRelease(item, chunk);
            isPainting = false;
        }
        public override void ItemInstance_MouseLeave(ItemContainer item, ushort chunk)
        {
            base.ItemInstance_MouseLeave(item, chunk);
           

        }
        public override void ChunkViewLostFocus()
        {
            isPainting = false;
        }
    }
}
