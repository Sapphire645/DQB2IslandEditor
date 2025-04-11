using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.DataPK
{
    //This stores the data from the CMNDAT.
    public class IslandShell
    {
        public byte island { get; set; }
        private Minimap _minimap;
        public Minimap pMinimap => _minimap;
        //Not sure what values are what in the cmndat yet. So lets just do the minimap for now.
        public IslandShell(byte island, byte[] minimapBytes)
        {
            this.island = island;
            //Minimap shenanigans...
            _minimap = new Minimap(minimapBytes);
        }



    }
}
