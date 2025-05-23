using DQB2IslandEditor.InterfacePK.ChunkEditor;
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
    /// Interaction logic for InventoryInfo.xaml
    /// </summary>
    public partial class InventoryInfo : UserControl
    {
        public static readonly DependencyProperty ObjectInfoProperty =
        DependencyProperty.Register(
            "objectInfoPanel",
            typeof(ObjectInfo),
            typeof(InventoryInfo),
            new PropertyMetadata(null));

        public ObjectInfo objectInfoPanel
        {
            get { return (ObjectInfo)GetValue(ObjectInfoProperty); }
            set {SetValue(ObjectInfoProperty, value); }
        }
        public InventoryInfo()
        {
            InitializeComponent();
        }

        private void LoadInformation(object sender, RoutedEventArgs e)
        {
            if(objectInfoPanel != null)
                ChunkEditorWindow.openInfoPanel(InformationGrid, objectInfoPanel);
        }
    }
}
