using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DQB2IslandEditor.ObjectPK
{
    public class ItemInfo : ObjectInfo
    {
        public ItemInfo(ushort objectId, short imageId, byte tab, Colour colour, string name)
            : base(objectId, imageId, tab, colour, name)
        {
        }
    }
}
