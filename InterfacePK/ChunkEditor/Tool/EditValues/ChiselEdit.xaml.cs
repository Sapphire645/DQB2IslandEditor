using DQB2IslandEditor.ObjectPK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        private ChunkEditorViewModel viewModel;

        private string nameOfBind;
        private PropertyInfo propertyInfo; //What im binding to, sheesh...
        //what a mess
        public ChiselEdit(string Value, ChunkEditorViewModel viewModel)
        {
            nameOfBind = Value;

            this.viewModel = viewModel;

            Binding binding = new Binding(Value)
            {
                Source = viewModel,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            InitializeComponent();
            TBlock.SetBinding(TextBox.TextProperty, binding);

            propertyInfo = viewModel.GetType().GetProperty(nameOfBind);
            
        }

        private void ChangeChisel(object sender, RoutedEventArgs e)
        {
            propertyInfo.SetValue(viewModel, (Chisel)byte.Parse(((Button)sender).Tag.ToString()));
        }
        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameOfBind)  // BLOCKINFO CHANGE
            {
                ChiselImage.Source = DataBaseReading.ValueChiselImage((Chisel)propertyInfo.GetValue(viewModel));
            }
        }
    }
}
