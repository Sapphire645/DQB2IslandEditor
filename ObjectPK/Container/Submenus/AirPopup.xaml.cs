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
    /// Interaction logic for Air.xaml
    /// </summary>
    public partial class AirPopup : UserControl
    {
        private InventoryContainer dad;

        private Action<ObjectInfo>? _selectedColourFunct;
        private Action<ObjectInfo>? _rightClickColourFunct;

        public AirPopup(Action<ObjectInfo>? selectedColourFunct, Action<ObjectInfo>? rightClickColourFunct)
        {

            this.dad = (InventoryContainer)DataContext;

            _selectedColourFunct = selectedColourFunct;
            _rightClickColourFunct = rightClickColourFunct;

            InitializeComponent();
        }
        private void SelectedRightItem(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var id = byte.Parse((sender as Button).Tag.ToString());
            ObjectInfo temp;
            if (id == 20)
            {
                temp = DataBaseReading.BLOCK_INFO_DICTIONARY[0];
            }
            else
            {
                temp = DataBaseReading.BLOCK_INFO_DICTIONARY[(uint)(1246 + (id*89))];
                
            }
            _rightClickColourFunct.Invoke(temp);
        }

        private void SelectedItem(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var id = byte.Parse((sender as Button).Tag.ToString());
            ObjectInfo temp;
            if (id == 20)
            {
                temp = DataBaseReading.BLOCK_INFO_DICTIONARY[0];
            }
            else
            {
                temp = DataBaseReading.BLOCK_INFO_DICTIONARY[(uint)(1246 + (id * 89))];

            }
            _selectedColourFunct.Invoke(temp);
        }
    }
}
