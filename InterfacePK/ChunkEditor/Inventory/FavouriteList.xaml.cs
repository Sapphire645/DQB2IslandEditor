using DQB2IslandEditor.ObjectPK;
using DQB2IslandEditor.ObjectPK.Container;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
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

namespace DQB2IslandEditor.InterfacePK.ChunkEditor.Inventory
{
    /// <summary>
    /// Interaction logic for FavouriteList.xaml
    /// </summary>
    public partial class FavouriteList : UserControl
    {
        ChunkEditorViewModel viewModel;
        public ObservableCollection<ListContainer> favouriteList { get; private set; } = new ObservableCollection<ListContainer>();
        public FavouriteList()
        {
            DataContext = this;
            InitializeComponent();
        }

        public void shareViewModel(ChunkEditorViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public void addObject(ObjectInfo objectInfo)
        {
            var container = new ListContainer(objectInfo);
            container.SlotButton.Click += (_, _) => { UpdateSelectedObject(objectInfo); };
            container.SlotButton.MouseRightButtonDown += (_, _) => { UpdateFavourite(objectInfo); };
            favouriteList.Add(container);
        }

        //rerouting this
        public void UpdateSelectedObject(ObjectInfo objectInfo)
        {
            viewModel.UpdateSelectedObject(objectInfo);
        }
        public void UpdateFavourite(ObjectInfo objectInfo)
        {
            ListContainer container = FindListContainer(objectInfo);
            if (container == null) //Not found
                addObject(objectInfo);//Add
            else //Found
                favouriteList.Remove(container);
        }
        //Search for the object. Returns NULL if not found
        private ListContainer FindListContainer(ObjectInfo objectInfo)
        {
            foreach (var container in favouriteList)
            {
                if (container.objectInfo == objectInfo) return container;
            }
            return null;
        }
    }
}
