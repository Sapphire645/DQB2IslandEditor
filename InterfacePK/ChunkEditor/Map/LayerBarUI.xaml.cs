using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Shapes;
using System;
using System.Security.Cryptography;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using DQB2IslandEditor.DataPK;

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Map
{
    //Copied from old proyect (Me lazy)
    public partial class LayerBarUI : UserControl
    {
        private List<RadioButton> RectLines = new();
        private RadioButton SelectedRadioButton;
        public LayerBarUI()
        {
            InitializeComponent();
            CreateRadioButtons();
            var RadioButton = RectLines.ElementAt(0);
            RadioButton.IsChecked = true;
            SelectedRadioButton = RadioButton;
        }
        

        private void CreateRadioButtons()
        {
            var Y_DIMENSION = Chunk.Y_DIMENSION;
            if (FindResource("RadioButtonStyle") is Style RadioButtonStyle)
            {
                for (uint i = Y_DIMENSION; i > 0; i--)
                {
                    var RadioButton = new RadioButton
                    {
                        Style = RadioButtonStyle,
                        Tag = i - 1,
                        Foreground = Brushes.White,
                        Height= i == 1 || i == Y_DIMENSION ? 8 : 7,
                        Width = i == 1 || i == Y_DIMENSION ? 19 :
                        i % 10 == 1 ? 16 :
                        i % 5 == 1 ? 14 : 10,
                        Margin = i == 1 || i == Y_DIMENSION ? new Thickness(0, 0, 0, 0) : new Thickness(0, 0.5, 0, 0.5)
                    };
                    RectLines.Insert(0, RadioButton);
                    RadioButton.Click += (_, _) => { RadioButton_Click(RadioButton); };
                    GridLine.Children.Add(RadioButton);

                    if (i % 10 == 1)
                    {
                        Numbers.Children.Add(new TextBlock()
                        {
                            Text = (i - 1).ToString(),
                            FontSize = 12,
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Right
                        });
                    }
                    else if (i % 5 == 1)
                    {
                        Numbers.Children.Add(new TextBlock()
                        {
                            Text = (i - 1).ToString(),
                            FontSize = 10,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Foreground = Brushes.White
                        });
                    }
                }
            }
        }
        public void UpdateSeaLevel(byte seaLevel)
        {
            ushort i = 0;
            foreach(var Button in RectLines)
            {
                if (i <= seaLevel)
                {
                    Button.Foreground = Brushes.LightSkyBlue;
                }
                else
                {
                    Button.Foreground = Brushes.White;
                }
                i++;
            }
        }
        private void RadioButton_Click(RadioButton sender)
        {
            ((ChunkEditorViewModel)DataContext).CurrentLayer = byte.Parse(sender.Tag.ToString());
        }

        public void LayerChange(byte Layer)
        {
            var RadioButton = RectLines.ElementAt(Layer);
            if (SelectedRadioButton != null)
            SelectedRadioButton.IsChecked = false;
            RadioButton.IsChecked = true;
            SelectedRadioButton = RadioButton;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ChunkEditorViewModel)DataContext).CurrentLayer += 1;
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            ((ChunkEditorViewModel)DataContext).CurrentLayer -= 1;
        }

        public void Resize() {
            System.Windows.Point RadioButtonPosition = SelectedRadioButton.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));
            //ArrowLayer.Margin = new Thickness(YLayerLine.ActualWidth / 2, RadioButtonPosition.Y - (7 * YLayerLine.ActualHeight / 960), 0, 0);
            Numbers.Margin = new Thickness(0, 0, 0, -(Numbers.ActualHeight/25)+7);
        //    UpButton.Margin = new Thickness(YLayerLine.ActualWidth / 2, RadioButtonPosition.Y - (7 * YLayerLine.ActualHeight / 960) - 20, 0, 0);
        //    DownButton.Margin = new Thickness(YLayerLine.ActualWidth / 2, RadioButtonPosition.Y - (7 * YLayerLine.ActualHeight / 960) + 20, 0, 0);
        }

        private void ScrollUpdate(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta > 0)
                {
                    ((ChunkEditorViewModel)DataContext).CurrentLayer += 1;
                }
                else
                {
                    ((ChunkEditorViewModel)DataContext).CurrentLayer -= 1;
                }
            }
        }
    }
}
