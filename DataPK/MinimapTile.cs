using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.DataPK
{
    public class MinimapTile
    {
        public bool Explored
        {
            get => _explored;
            set => _explored = value;
        }
        public bool Height
        {
            get => _height;
            set => _height = value;
        }
        public byte Decorator
        {
            get => _decorator;
            set
            {
                if (value < 11) _decorator = value;
            }
        }
        public ushort Type
        {
            get => _type;
            set
            {
                if (value < 0x3FFF) _type = value;
            }
        }

        public byte ByteType => Type < 255 ? (byte)Type : (byte)0;

        private bool _explored;
        private bool _height;
        private byte _decorator;
        private ushort _type;

        public bool IsEmpty()
        {
            if(_type == 0 && _decorator == 0 && !_height) return true;
            if (_type == 4 && _decorator == 0 && !_height) return true; //Malhalla
            return false;
        }
        public MinimapTile(byte[] bytes)
        {
            _type = (ushort)(bytes[0] + ((bytes[1] & 0x3F) << 8)-1); //Temporal

            _decorator = (byte)(_type % 11);
            _type = (ushort)(_type / 11);

            _explored = ((bytes[1] & 0x80) == 0x80);
            _height = ((bytes[1] & 0x40) == 0x40);
        }

        public MinimapTile(byte byteOne, byte byteTwo)
        {
            _type = (ushort)(byteOne + ((byteTwo & 0x3F) << 8)-1); //Temporal

            _decorator = (byte)(_type % 11);
            _type = (ushort)(_type / 11);

            _explored = ((byteTwo & 0x80) == 0x80);
            _height = ((byteTwo & 0x40) == 0x40);
        }

    }
}
