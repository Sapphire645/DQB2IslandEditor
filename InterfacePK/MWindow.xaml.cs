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
using System.Windows.Shapes;

namespace DQB2IslandEditor.InterfacePK
{
    /// <summary>
    /// Interaction logic for MWindow.xaml
    /// </summary>
    public partial class MWindow : Window
    {
        private ViewModel viewModel;
        public MWindow()
        {
            viewModel = new ViewModel(this);
            DataContext = viewModel;

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenCMNDATpath();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            viewModel.OpenSTGDAT();
        }

        private void SelectedIslandLeft(object sender, RoutedEventArgs e)
        {
            viewModel.SelectedIslandRotate(-1);
        }
        private void SelectedIslandRight(object sender, RoutedEventArgs e)
        {
            viewModel.SelectedIslandRotate(1);
        }
    }
}
