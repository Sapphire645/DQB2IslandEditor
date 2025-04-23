using DQB2IslandEditor.DataPK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DQB2IslandEditor.ObjectPK.Container
{
    /// <summary>
    /// Interaction logic for ItemContainer.xaml
    /// </summary>
    public partial class ItemContainer : UserControl
    {
        public ItemInstance itemInstance
        {
            get { return _itemInstance; }
            set
            {
                if (_itemInstance != null && _itemInstance.publicItemId == value.publicItemId) return; //Try to reduce cycles?
                _itemInstance = value;
                var a = itemInfo; //Get some cycles
                //Placeholdering this rn
                itemInfo.Value = DataBaseReading.ITEM_INFO_DICTIONARY[0];
                    //DataBaseReading.ITEM_INFO_DICTIONARY[_itemInstance.publicItemId];
            }
        }

        private ItemInstance _itemInstance;

        public ObservableProperty<ItemInfo> itemInfo { get; set; } = new ObservableProperty<ItemInfo> { };

        public ushort offset => _itemInstance.worldOffset;
        public ItemContainer(ItemInstance itemI, byte TILE_SIZE)
        {
            //Set.
            itemInstance = itemI;
            //From corner.
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
            //Size of the item (use the dimensions from item info)
            this.Width = TILE_SIZE; //Placeholder
            this.Height = TILE_SIZE;
            //Get the position from the instance
            this.Margin = itemInstance.CoordinateMargin(TILE_SIZE);

            InitializeComponent();
            DataContext = this;
        }

        public void IsHovered()
        {
            ItemBorder.Background = Brushes.DarkOrange;
            Image.Opacity = 0.5;
        }
        public void IsUnHovered()
        {
            ItemBorder.Background = Brushes.Transparent;
            Image.Opacity = 1;
        }
        public void IsClicked()
        {
            ItemBorder.Background = Brushes.Yellow;
            Image.Opacity = 0.5;
        }
    }
}
