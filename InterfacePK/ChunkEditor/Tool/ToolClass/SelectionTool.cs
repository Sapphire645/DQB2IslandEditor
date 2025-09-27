using DQB2IslandEditor.DataPK;
using DQB2IslandEditor.InterfacePK.ChunkEditor.Map.ChunkView;
using DQB2IslandEditor.ObjectPK.Container;
using System.Transactions;

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.ToolClass
{
    public class SelectionTool : AHoverTool
    {
        public SelectionTool(ChunkEditorViewModel viewModel, ChunkBlockGrid chunkDisplays) : base(viewModel, chunkDisplays)
        {
            usesAura = false; //Selection tool does not use aura.
        }
        public override void BlockInstance_MouseLeftClick(ushort offset, ushort chunk)
        {
            base.BlockInstance_MouseLeftClick(offset, chunk);
            viewModel.UpdateSelectedObject(chunkDisplays.GetBlockInfo(offset, chunk));
            viewModel.UpdateSelectedBlock(chunkDisplays.GetBlockInstance(offset, chunk));
            viewModel.UpdateSelectedItemInfo(null);
        }


        public override void ItemInstance_MouseLeftClick(ItemContainer itemInstance, ushort chunk)
        {
            base.ItemInstance_MouseLeftClick(itemInstance, chunk);
            viewModel.UpdateSelectedObject(chunkDisplays.GetBlockInfo(itemInstance.itemInstance.worldOffset, chunk));
            viewModel.UpdateSelectedObject(itemInstance.itemInfo.Value);
            viewModel.UpdateSelectedBlock(chunkDisplays.GetBlockInstance(itemInstance.itemInstance.worldOffset, chunk));
            //viewModel.UpdateSelectedItem(itemInstance);
        }

    }
}
