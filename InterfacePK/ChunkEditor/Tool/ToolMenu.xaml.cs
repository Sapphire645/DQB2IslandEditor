using DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.EditValues;
using DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.Information;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool
{

    /// <summary>
    /// Interaction logic for ToolMenu.xaml
    /// </summary>
    public partial class ToolMenu : UserControl
    {
        private ChunkEditorViewModel viewModel;
        public ObservableCollection<UserControl> PanelsToDisplay { get; set; } = new ObservableCollection<UserControl> ();
        private byte _currentTab = 0;

        //Editing values panel


        //Information panel
        private UserControl _blockChiselPanel = new ChiselEdit();

        private UserControl _blockInformationPanel = new BlockInformation();
        private UserControl _blockValuesPanel = new BlockValues();
        //Area panel


        //Replace panel


        public ToolMenu()
        {
            InitializeComponent();
        }
        public void UpdateContext(ChunkEditorViewModel viewModelGive)
        {
            viewModel = viewModelGive;
            viewModel.PropertyChanged += PropertyChanged;
            PanelsToDisplay.Clear();
        }

        private void EditPanel(object sender, RoutedEventArgs e)
        {
            if(viewModel == null)return;
            _currentTab = 0;
            PanelsToDisplay.Clear();
            if (viewModel.SelectedBlock != null) PanelsToDisplay.Add(_blockChiselPanel);
        }

        private void InfoPanel(object sender, RoutedEventArgs e)
        {
            _currentTab = 1;
            PanelsToDisplay.Clear();
            if(viewModel.SelectedBlock != null) PanelsToDisplay.Add(_blockInformationPanel);
            if(viewModel.SelectedBlock != null) PanelsToDisplay.Add(_blockValuesPanel);
        }

        private void AreaPanel(object sender, RoutedEventArgs e)
        {
            _currentTab = 2;
            PanelsToDisplay.Clear();
        }

        private void ReplacePanel(object sender, RoutedEventArgs e)
        {
            _currentTab = 3;
            PanelsToDisplay.Clear();
        }
        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.SelectedBlock))  // BLOCKINFO CHANGE
            {
                switch (_currentTab)
                {
                    case 0:
                        if (viewModel.SelectedBlock == null && PanelsToDisplay.Contains(_blockChiselPanel)) PanelsToDisplay.Remove(_blockChiselPanel);
                        if (viewModel.SelectedBlock != null && !PanelsToDisplay.Contains(_blockChiselPanel)) PanelsToDisplay.Add(_blockChiselPanel);
                        break;
                    case 1:
                        if (viewModel.SelectedBlock == null && PanelsToDisplay.Contains(_blockInformationPanel)) PanelsToDisplay.Remove(_blockInformationPanel);
                        if (viewModel.SelectedBlock != null && !PanelsToDisplay.Contains(_blockInformationPanel)) PanelsToDisplay.Add(_blockInformationPanel);

                        if (viewModel.SelectedBlock == null && PanelsToDisplay.Contains(_blockValuesPanel)) PanelsToDisplay.Remove(_blockValuesPanel);
                        if (viewModel.SelectedBlock != null && !PanelsToDisplay.Contains(_blockValuesPanel)) PanelsToDisplay.Add(_blockValuesPanel);
                        break;
                }
            }
        }
    }
}
