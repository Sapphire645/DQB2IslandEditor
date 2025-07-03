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
    /// Interaction logic for GeneralGroupPopup.xaml
    /// </summary>
    public partial class GeneralGroupPopup : UserControl
    {
        private InventoryContainer dad;
        public ObservableCollection<InventoryContainer> objects { get; set; }

        public GeneralGroupPopup(Action<ObjectInfo>? selectedColourFunct, Action<ObjectInfo>? rightClickColourFunct, List<ObjectInfo> objectInfos)
        {

            this.dad = (InventoryContainer)DataContext;
            objects = new ObservableCollection<InventoryContainer>();
            foreach (var ob in objectInfos)
                objects.Add(new InventoryContainer(ob, selectedColourFunct, rightClickColourFunct));
            InitializeComponent();
        }

    }
}
