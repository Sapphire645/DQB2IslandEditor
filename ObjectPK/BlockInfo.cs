using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DQB2IslandEditor.ObjectPK
{
    public class BlockInfo : ObjectInfo
    {
        public readonly bool liquid;
        public BlockInfo(ushort objectId, short imageId,bool liquid, byte tab, Colour colour, string name, ImageSource objectInventoryImage, ImageSource objectMapImage)
            : base(objectId, imageId,tab, colour, name, objectInventoryImage, objectMapImage)
        {
            this.liquid = liquid;
        }
    }
}
