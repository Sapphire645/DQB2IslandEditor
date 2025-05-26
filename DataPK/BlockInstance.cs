using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DQB2IslandEditor.DataPK
{
    public class BlockInstance
    {
        private bool builderPlaced;
        private uint blockID;
        private Chisel chiselID;

        //ADD BLOCK INFO

        public uint publicBlockID => blockID;
        public Chisel publicChiselID => chiselID;
        public bool publicBuilderPlaced => builderPlaced;
        public BlockInstance(byte[] blockBytes)
        {
            blockID = (uint)(blockBytes[0] + ((blockBytes[1] & 0x07) << 8));
            builderPlaced = (blockBytes[1] & 0x08) == 0x08;
            chiselID = (Chisel)(blockBytes[1] >> 4);
        }

        public BlockInstance(byte blockByteOne, byte blockByteTwo)
        {
            blockID = (uint)(blockByteOne + ((blockByteTwo & 0x07) << 8));
            builderPlaced = (blockByteTwo & 0x08) == 0x08;
            chiselID = (Chisel)(blockByteTwo >> 4);
        }

        public BlockInstance(uint blockID, Chisel chiselID, bool overflow)
        {
            this.blockID = blockID;
            this.builderPlaced = overflow;
            this.chiselID = chiselID;
        }

        public byte[] GetBytes()
        {
            var bytes = new byte[2];
            bytes[0] = (byte)(blockID & 0xFF);
            bytes[1] = (byte)(blockID & 0xFF00 + (builderPlaced ? 0x0800 : 0) + (byte)chiselID << 12);
            return bytes;
        }

        public void UpdateBlock(bool builderPlaced, uint blockID, Chisel chiselID)
        {
            this.builderPlaced = builderPlaced;
            this.blockID = blockID;
            this.chiselID = chiselID;
        }
        public void UpdateBlock(Chisel chiselID)
        {
            this.chiselID = chiselID;
        }
        public void UpdateBlock(ushort id)
        {
            blockID = id;
        }
        public void UpdateBlock(bool overflow)
        {
            builderPlaced = overflow;
        }
    }
}
