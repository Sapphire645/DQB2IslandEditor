using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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

        public int itemDelta = 0;

        private Dictionary<ushort, List<ItemInstance>> listCache;
        public PropHandlerClass(Island island, uint PropCountSave, List<ItemInstance> items, List<ushort> chunk)
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
            if(index != -1)
            {
                _edited[index] = true;
                _items[index] = null;

                itemDelta--;
            }
            
        }

        public ItemInstance CreateItem(ItemInfo item, Chunk chunk, byte x, byte layer, byte z, byte rotation)
        {
            //I think its adding a ton of the same item in the same place?
            for(int i = 0; i < _items.Count; i++)
            {
                if (_chunk[i].Item1 == chunk.chunkXPosition && _chunk[i].Item2 == chunk.chunkYPosition)
                {
                    if (_items[i].equalsItem((int)item.objectId, x, layer, z, rotation))
                    {
                        return null;
                    }
                }
            }
            ItemInstance itemInstance = new ItemInstance(x, layer, z, rotation, item.objectId);
            int index = _items.FindIndex(1,i => i == null); //0 seems to not work.
            if (index >= 0)
            {
                _items[index] = itemInstance;
                _edited[index] = true;
                _chunk[index] = (chunk.chunkXPosition, chunk.chunkYPosition);
            }
            else
            {
                _items.Add(itemInstance);
                _edited.Add(true);
                _chunk.Add((chunk.chunkXPosition, chunk.chunkYPosition));
            }
            if (listCache.ContainsKey(chunk.chunkPosition))
            {
                listCache[chunk.chunkPosition].Add(itemInstance);
            }
            itemDelta++;
            return itemInstance;
            }

        //This is the code that will manage checking what entries to edit in the island
        //Once it gets more complete it will become more complicated, with defragmentation and things.

        //Remember to edit the entry pointer here
        public List<(uint,ItemInstance, ushort)> GetAllEditedItems()
        {
            List<(uint, ItemInstance, ushort)> editedItems = new List<(uint, ItemInstance, ushort)>();
            for(int i = 0; i < _items.Count; i++)
            {
                if (_edited[i])
                {
                    editedItems.Add(((uint)i, _items[i], (ushort)(_chunk[i].Item1 + (_chunk[i].Item2 * Island.GRID_DIMENSION))));
                    _edited[i] = false;
                }
            }
            return editedItems;
        }
    }
}
