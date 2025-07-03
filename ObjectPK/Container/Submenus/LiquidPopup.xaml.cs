using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DQB2IslandEditor.ObjectPK.Container.Submenus
{
    /// <summary>
    /// Interaction logic for LiquidPopup.xaml
    /// </summary>
    public partial class LiquidPopup : UserControl
    {
        private InventoryContainer dad;
        public ObservableCollection<ObjectInfo> objects { get; set; }

        private Action<ObjectInfo>? _selectedColourFunct;
        private Action<ObjectInfo>? _rightClickColourFunct;

        private byte shellIndex = 20;
        private byte liquidIndex = 0;
        static public readonly ushort LIQUID_START = 1158;
        static public readonly byte LIQUID_SIZE = 11;
        static public readonly byte LIQUID_COUNT = 8;
        static public readonly byte[] LIQUID_ORDER = [2,8,7,6,5,4,3,0,10,9,1];

        //Hmmm, gonna do some code sinning here. Dont mind the spaguetti.
        public LiquidPopup(Action<ObjectInfo>? selectedColourFunct, Action<ObjectInfo>? rightClickColourFunct,
            List<ObjectInfo> objectInfos, byte liquidIndex)
        {
            this.liquidIndex = liquidIndex;

            this.dad = (InventoryContainer)DataContext;
            objects = new ObservableCollection<ObjectInfo>();
            foreach (var ob in objectInfos)
                objects.Add(ob);

            _selectedColourFunct = selectedColourFunct;
            _rightClickColourFunct = rightClickColourFunct;

            InitializeComponent();
        }
        private void SelectedRightItem(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ObjectInfo item = null;
            if (shellIndex == 20)
                item = objects[byte.Parse((sender as Button).Tag.ToString())];
            else
                item = DataBaseReading.BLOCK_INFO_DICTIONARY[(uint)(LIQUID_START + (shellIndex * LIQUID_COUNT * LIQUID_SIZE) + (liquidIndex * LIQUID_SIZE) + LIQUID_ORDER[byte.Parse((sender as Button).Tag.ToString())] + shellIndex)];
            _rightClickColourFunct.Invoke((ObjectInfo)item);
        }

        private void SelectedItem(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ObjectInfo item = null;
            if (shellIndex == 20)
                item = objects[byte.Parse((sender as Button).Tag.ToString())];
            else
                item = DataBaseReading.BLOCK_INFO_DICTIONARY[(uint)(LIQUID_START + (shellIndex * LIQUID_COUNT * LIQUID_SIZE) + (liquidIndex * LIQUID_SIZE) + LIQUID_ORDER[byte.Parse((sender as Button).Tag.ToString())] + shellIndex)];
            _selectedColourFunct.Invoke((ObjectInfo)item);
        }

        private void ShellCheck(object sender, RoutedEventArgs e)
        {
            shellIndex = byte.Parse((sender as RadioButton).Tag.ToString());
        }
        private void ShellUncheck(object sender, RoutedEventArgs e)
        {
            shellIndex = 20;
        }

        private void FeckYou(object sender, RoutedEventArgs e)
        {
            CheckMe.IsChecked = true;
        }
    }
}
