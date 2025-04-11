using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace DQB2IslandEditor.DataPK
{
    public class ItemInstance
    {

        private byte[] bytes;

        private ushort itemId;
        private ushort chunk;

        private byte x;
        private byte y;
        private byte z;

        private byte rotation;

        public ItemInfo itemInfo { get; set; } //Shit

        public short chunkX => (short)(chunk / Island.GRID_DIMENSION);
        public short chunkY => (short)(chunk % Island.GRID_DIMENSION);
        public ItemInstance(byte[] bytes) {
            this.bytes = bytes;

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

        public bool IsInChunkLayer(ushort chunk, byte layer) {
            if (y == y && this.chunk == chunk) return true; //Change
            return false;
        }
        public bool IsInLayer(byte y) { 
            if (this.y == y) return true; //Change
            return false;
        }
    }
}
