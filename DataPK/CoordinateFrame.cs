using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.DataPK
{
    public class CoordinateFrame //Used for the minimap and the chunk grid.
    {
        public uint X0 { get; set; }
        public uint X1 { get; set; }
        public uint Y0 { get; set; }
        public uint Y1 { get; set; }
        public CoordinateFrame(uint x0, uint x1, uint y0, uint y1)
        {
            X0 = x0;
            X1 = x1;
            Y0 = y0;
            Y1 = y1;
        }
    }

    public class AreaFrame //Used for the minimap and the chunk grid.
    {
        private byte X0;
        private byte Y0;
        private byte Z0;
        private ushort Chunk0;
        private byte X1;
        private byte Y1;
        private byte Z1;
        private ushort Chunk1;
        public AreaFrame(byte x0, byte y0, byte z0, ushort chunk0, byte x1, byte y1, byte z1, ushort chunk1)
        {
            X0 = x0;
            Y0 = y0;
            Z0 = z0;
            Chunk0 = chunk0;
            X1 = x1;
            Y1 = y1;
            Z1 = z1;
            Chunk1 = chunk1;
        }
        public void SetZero(byte x0, byte y0, byte z0, ushort chunk0)
        {
            X0 = x0;
            Y0 = y0;
            Z0 = z0;
            Chunk0 = chunk0;
        }
        public void SetOne(byte x1, byte y1, byte z1, ushort chunk1)
        {
            X1 = x1;
            Y1 = y1;
            Z1 = z1;
            Chunk1 = chunk1;
        }
    }
}
