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

        private byte witdh;
        private byte height;
        private byte depth;
        public byte Witdh=>witdh;
        public byte Height => height;
        public byte Depth => depth;

        //Idk how you write dimension in english whateveeerrrrrrrrr
        public ItemInfo(ushort objectId, short imageId,string dimension, byte tab, Colour colour, string name)
            : base(objectId, imageId, tab, colour, name)
        {
            var parts = dimension.Split('x');
            witdh = byte.Parse(parts[0]);
            height = byte.Parse(parts[1]);
            depth = byte.Parse(parts[2]);
        }
    }
}
