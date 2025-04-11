using DQB2IslandEditor.ObjectPK;
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

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.EditValues
{
    /// <summary>
    /// Interaction logic for ChiselEdit.xaml
    /// </summary>
    public partial class ChiselEdit : UserControl
    {
        public ChiselEdit()
        {
            InitializeComponent();
        }

        private void ChangeChisel(object sender, RoutedEventArgs e)
        {
            ((ChunkEditorViewModel)DataContext).ValueChisel = (Chisel)byte.Parse(((Button)sender).Tag.ToString());
        }
    }
}
