using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace DQB2IslandEditor.ObjectPK.Container
{
    /// <summary>
    /// Interaction logic for ColourPopup.xaml
    /// </summary>
    public partial class ColourPopup : UserControl
    {
        private InventoryContainer dad;
        public ObservableCollection<ObjectInfo> objects { get; set; }

        private Action<ObjectInfo>? _selectedColourFunct;
        private Action<ObjectInfo>? _rightClickColourFunct;

        public ColourPopup(Action<ObjectInfo>? selectedColourFunct, Action<ObjectInfo>? rightClickColourFunct, 
            ObjectInfo plain, List<ObjectInfo> objectInfos)
        {
            
            this.dad = (InventoryContainer)DataContext;
            objects = new ObservableCollection<ObjectInfo>();
            objects.Add(plain);
            foreach(var ob in objectInfos)
                objects.Add(ob);

            _selectedColourFunct = selectedColourFunct;
            _rightClickColourFunct = rightClickColourFunct;

            InitializeComponent();
        }
        private void SelectedRightItem(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement((ListBox)sender, e.OriginalSource as DependencyObject) as ListBoxItem;
            _rightClickColourFunct.Invoke((ObjectInfo)item.DataContext);
        }

        private void SelectedItem(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement((ListBox)sender, e.OriginalSource as DependencyObject) as ListBoxItem;
            _selectedColourFunct.Invoke((ObjectInfo)item.DataContext);
        }
    }
}
