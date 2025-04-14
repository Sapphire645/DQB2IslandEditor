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
using System.Windows.Media.Animation;
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

        private void CMNDAT_Open(object sender, RoutedEventArgs e)
        {
            viewModel.OpenCMNDATpath();
            Border.BorderBrush = Brushes.White;
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

        private void RecalculateSize(object sender, SizeChangedEventArgs e)
        {
            var transform = ButtonOfMenu.TransformToVisual(MenuViewbox);
            Rect bounds = transform.TransformBounds(new Rect(0, 0, ButtonOfMenu.ActualWidth, ButtonOfMenu.ActualHeight));

            ViewBoxCMNDAT.Height = bounds.Height;

            //Caluclate the size of the textbox (pain)

            CMNDATGrid.Width = this.ActualWidth * ButtonOfMenu.ActualHeight / bounds.Height;
        }

        private void ClickChange(object sender, MouseButtonEventArgs e)
        {
            Border.BorderBrush = Brushes.Gold;
        }

        private void HoverEnter(object sender, MouseEventArgs e)
        {
            Transp.Opacity = 0.9;
        }

        private void HoverLeave(object sender, MouseEventArgs e)
        {
            Transp.Opacity = 0.7;
        }

        private void MouseMove_Paralax(object sender, MouseEventArgs e)
        {
            Point localPosition = e.GetPosition(this);

            double GridWidth = this.ActualWidth;
            double GridHeight = this.ActualHeight;

            double parallaxFactor = -0.02; 

            double offsetX = (localPosition.X - GridWidth / 2) * parallaxFactor;
            double offsetY = (localPosition.Y - GridHeight / 2) * parallaxFactor;

            // Apply the calculated offset to the background image
            ParalaxGrid.RenderTransform = new TranslateTransform(offsetX, offsetY);
        }
    }
}
