using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.ToolClass
{
    interface ITool
    {
        //A tool is the little thing on the left of the downright panel.
        //Depending on what tool you have, the program must react in a different way to these inputs
        //You can either click/hover over a block, or an item.
        //In certain tools the item hovering will work the exact same as the block hover
        //Since this is an interface, for any reaction we'll have to use the offset and chunk to fetch the attributes we'll need.
        public void BlockInstance_MouseEnter(ushort offset, ushort chunk);
        public void BlockInstance_MouseLeave(ushort offset, ushort chunk);
        public void BlockInstance_MouseLeftClick(ushort offset, ushort chunk);
        public void BlockInstance_MouseRelease(ushort offset, ushort chunk);
        public void ItemInstance_MouseEnter(ushort offset, ushort chunk, byte itemLocationInArray);
        public void ItemInstance_MouseLeave(ushort offset, ushort chunk, byte itemLocationInArray);
        public void ItemInstance_MouseLeftClick(ushort offset, ushort chunk, byte itemLocationInArray);
        public void ItemInstance_MouseRelease(ushort offset, ushort chunk, byte itemLocationInArray);
    }
}
