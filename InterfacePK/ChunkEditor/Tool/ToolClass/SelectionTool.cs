using DQB2IslandEditor.InterfacePK.ChunkEditor.Map.ChunkView;
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
            viewModel.UpdateSelectedTile(chunk,offset);
        }

    }
}
