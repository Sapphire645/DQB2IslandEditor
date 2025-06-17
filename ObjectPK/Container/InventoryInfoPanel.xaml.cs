using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for InventoryInfoPanel.xaml
    /// </summary>
    public partial class InventoryInfoPanel : UserControl
    {
        public static readonly DependencyProperty ObjectInfoProperty =
        DependencyProperty.Register(
            "objectInfo",
            typeof(ObjectInfo),
            typeof(InventoryInfo),
            new PropertyMetadata(null));

        public ObjectInfo objectInfo
        {
            get { return (ObjectInfo)GetValue(ObjectInfoProperty); }
            set
            {
                if (objectInfo != null)
                    objectInfo.InventoryImageChanged -= updateImage;
                SetValue(ObjectInfoProperty, value); DataContext = value;
                value.InventoryImageChanged += updateImage;
            }
        }
        public InventoryInfoPanel()
        {
            InitializeComponent();
        }
        private void updateImage(object sender, PropertyChangedEventArgs e)
        {
            if (IsLoaded)
                Darn.Source = objectInfo.objectInventoryImage;
        }

        private void LoaddImage(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                Darn.Source = objectInfo.objectInventoryImage;
        }
    }
}
