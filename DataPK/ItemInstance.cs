using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace DQB2IslandEditor.DataPK
{
    public class ItemInstance
    {

        private byte[] bytes;

        public ushort publicItemId => itemId;
        private ushort itemId;
        private byte x;
        private byte y;
        private byte z;

        private byte rotation;

        public ItemInfo itemInfo => DataBaseReading.ITEM_INFO_DICTIONARY[itemId]; //I need this for the margins.
        public ushort worldOffset => (ushort)(x + z * 32);

        public static bool IsThisEntryEmpty(byte[] bytes)
        {
            //This is placeholder for now
            if (bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0 && bytes[3] == 0 && bytes[4] == 0 && bytes[5] == 0)
            {
                return true;
            }
            return false;
        }
        public ItemInstance(byte[] bytes) {
            this.bytes = new byte[24]; 
            Array.Copy(bytes, this.bytes, bytes.Length);

            itemId = bytes[8];
            var tmp = bytes[9] & 0x1F;
            itemId += (ushort)(tmp * 256);

            x = (byte)(bytes[9] >> 5);
            tmp = bytes[10] & 0x3;
            x += (byte)(tmp << 3);

            y = (byte)(bytes[10] >> 2);
            tmp = bytes[11] & 0x1;
            y += (byte)(tmp << 6);

            z = (byte)(bytes[11] & 0x3E);
            z >>= 1;
            rotation = (byte)(bytes[11] >> 6);
        }

        public byte[] GetByteFormat()
        {
            byte[] itemIdBytes = BitConverter.GetBytes(itemId);

            bytes[8] = itemIdBytes[0];
            bytes[9] = (byte)(itemIdBytes[1] + (byte)(x<<5));

            bytes[10] = (byte)((byte)(x>>3) + (byte)(y<<2));

            bytes[11] = (byte)((byte)(y >> 6) + (byte)(z << 1) + (byte)(rotation << 6));

            return bytes;
        }

        //Used to calculate if the item is in a contiguous chunk.
        public bool DoIOverflow(bool north, bool south, bool east, bool west)
        {
            return false; //Change
        }
        public bool IsInLayer(byte y) { 
            if (this.y == y) return true; //Change
            return false;
        }

        public Thickness CoordinateMargin(byte tileSize)
        {
            //This will tell the UI where to place the item.
            return new Thickness(x * tileSize, z * tileSize, 0, 0);
        }

        public override string ToString()
        {
            string a = "";
            foreach(byte b in bytes) a += b.ToString("X2") + " ";
            return $"ID: {itemId} - Y:{y},X:{x},Z:{z} - {rotation} |"+ a+"\n";
        }
    }
}
