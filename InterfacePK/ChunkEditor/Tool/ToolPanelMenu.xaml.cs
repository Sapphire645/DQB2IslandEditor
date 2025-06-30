using DQB2IslandEditor.InterfacePK.ChunkEditor.Tool.BlockPanel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;


namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Tool
{

    /// <summary>
    /// Interaction logic for ToolMenu.xaml
    /// </summary>
    public partial class ToolPanelMenu : UserControl
    {
        private ChunkEditorViewModel viewModel;
        public ObservableCollection<UserControl> PanelsToDisplay { get; set; } = new ObservableCollection<UserControl>();
        private byte _currentTab = 0;
        public byte CurrentTab => _currentTab;


        public List<UserControl> CreateBlockBasics = new List<UserControl>();
        public List<UserControl> CreateItemBasics = new List<UserControl>();

        public ToolPanelMenu()
        {
            InitializeComponent();
            
        }
        public void UpdateContext(ChunkEditorViewModel viewModelGive)
        {
            viewModel = viewModelGive;
            viewModel.PropertyChanged += PropertyChanged;
            PanelsToDisplay.Clear();

            CreateBlockBasics.Add(new BlockInformation(viewModelGive));
            CreateBlockBasics.Add(new ChiselEdit(viewModelGive));
            //CreateBlockBasics.Add(new ChiselEdit(nameof(viewModel.ValueChisel), viewModelGive));
            BlockPanel(null, null);
        }

        private void BlockPanel(object sender, RoutedEventArgs e)
        {
            if(viewModel == null)return;
            _currentTab = 0;
            PanelsToDisplay.Clear();

            foreach (var panel in CreateBlockBasics)
                PanelsToDisplay.Add(panel);
        }

        private void ItemPanel(object sender, RoutedEventArgs e)
        {
            _currentTab = 1;
            PanelsToDisplay.Clear();
        }

        private void AreaPanel(object sender, RoutedEventArgs e)
        {
            _currentTab = 2;
            PanelsToDisplay.Clear();
        }

        private void OtherPanel(object sender, RoutedEventArgs e)
        {
            _currentTab = 3;
            PanelsToDisplay.Clear();
        }
        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.SelectedBlock))  // BLOCKINFO CHANGE
            {
                if(viewModel.SelectedBlock == null)
                {
                }
            }
        }
    }
}
