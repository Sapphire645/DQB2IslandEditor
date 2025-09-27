using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.DataPK
{
    public class PropHandlerClass
    {

        //This class is for basically doing all the hard item work. 
        //I just need it to be able to store its entry offset and if it is edited and things like that.
        private Island island;

        private List<ItemInstance> _items;
        private List<bool> _edited;
        private List<(byte,byte)> _chunk;

        private Dictionary<ushort, List<ItemInstance>> listCache;
        public PropHandlerClass(Island island,uint PropCountSave, List<ItemInstance> items, List<ushort> chunk)
        {
            listCache = new Dictionary<ushort, List<ItemInstance>>();
            this.island = island;
            _items = items;
            _chunk = new List<(byte, byte)>();
            foreach (var chun in chunk)
            {
                _chunk.Add(((byte)(chun % Island.GRID_DIMENSION), (byte)(chun / Island.GRID_DIMENSION)));
            }
            _edited = new List<bool>();
            for (int i = 0; i < items.Count; i++)
            {
                _edited.Add(false);
            }
        }

        public List<ItemInstance> getItemsChunk(ushort chunkPosition)
        {
            if (listCache.ContainsKey(chunkPosition))
                return listCache[chunkPosition];
            byte x = (byte)(chunkPosition % Island.GRID_DIMENSION);
            byte z = (byte)(chunkPosition / Island.GRID_DIMENSION);
            List<ItemInstance> items = new List<ItemInstance>();
            for(int i = 0; i < _chunk.Count; i++)
            {
                if (_items[i] == null) continue;
                if(x == _chunk[i].Item1 && z == _chunk[i].Item2)
                {
                    items.Add(_items[i]);
                }
                else
                {
                    //Valid range.
                    if (_chunk[i].Item1 + 1 >= x  && _chunk[i].Item1 - 1 <= x
                        && _chunk[i].Item2 + 1 >= z && _chunk[i].Item2 - 1 <= z)
                    {
                        if(_items[i].DoIOverflow(_chunk[i].Item2 < z, _chunk[i].Item2 > z, _chunk[i].Item1 < x, _chunk[i].Item1 > x))
                            items.Add(_items[i]);
                    }
                }
            }
            listCache[chunkPosition] = items;
            return items;
        }

        public void RemoveItem(ItemInstance item)
        {
            listCache.Clear();
            var index = _items.FindIndex(x => x == item);
            _edited[index] = true;
            _items[index] = null;
        }

        //This is the code that will manage checking what entries to edit in the island
        //Once it gets more complete it will become more complicated, with defragmentation and things.

        //Remember to edit the entry pointer here
        public List<(uint,ItemInstance, ushort)> GetAllEditedItems()
        {
            List<(uint, ItemInstance, ushort)> editedItems = new List<(uint, ItemInstance, ushort)>();
            for(int i = 0; i < _items.Count; i++)
            {
                if (_edited[i] && _items[i] != null)
                    editedItems.Add(((uint)i, _items[i], (ushort)(_chunk[i].Item1 + (_chunk[i].Item2* Island.GRID_DIMENSION))));
            }
            return editedItems;
        }
    }
}
