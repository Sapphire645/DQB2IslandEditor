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
    /// Interaction logic for CropPopup.xaml
    /// </summary>
    public partial class CropPopup : UserControl
    {
        private InventoryContainer dad;
        public ObservableCollection<ObjectInfo> objects { get; set; }

        private Action<ObjectInfo>? _selectedColourFunct;
        private Action<ObjectInfo>? _rightClickColourFunct;

        public CropPopup(Action<ObjectInfo>? selectedColourFunct, Action<ObjectInfo>? rightClickColourFunct,
            List<ObjectInfo> objectInfos)
        {
            
            this.dad = (InventoryContainer)DataContext;
            objects = new ObservableCollection<ObjectInfo>();

            _selectedColourFunct = selectedColourFunct;
            _rightClickColourFunct = rightClickColourFunct;

            DataContext = this;

            InitializeComponent();

            var buttons = grid.Children.OfType<Button>().ToList();
            for (int i = 0; i < objectInfos.Count; i++)
            {
                objects.Add(objectInfos[i]);
                if (objectInfos[i] == null)
                {
                    buttons[i].IsEnabled = false;
                }
            }
        }

        private void SelectedRightItem(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ObjectInfo item = null;
            item = objects[byte.Parse((sender as Button).Tag.ToString())];
            _rightClickColourFunct.Invoke((ObjectInfo)item);
        }

        private void SelectedItem(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ObjectInfo item = null;
            item = objects[byte.Parse((sender as Button).Tag.ToString())];
            _selectedColourFunct.Invoke((ObjectInfo)item);
        }
    }
}
