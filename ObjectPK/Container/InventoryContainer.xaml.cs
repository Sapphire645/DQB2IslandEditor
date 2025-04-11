using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for InventoryContainer.xaml
    /// </summary>
    public partial class InventoryContainer : UserControl
    {
        public ObjectInfo objectInfo { get; private set; }

        public Dictionary<Colour, ObjectInfo> alternateColours { get; private set; } = null ;
        private Popup colourPopup = null;
        private Button colourButton = null;

        public Action<ObjectInfo>? selectedColourFunct;
        public Action<ObjectInfo>? rightClickColourFunct;
        public InventoryContainer(ObjectInfo objectInfo)
        {
            DataContext = this;
            this.objectInfo = objectInfo;
            InitializeComponent();
        }

        public void AddColour(ObjectInfo objectInfo)
        {
            //If no colours have been added
            if (alternateColours == null)
            {
                alternateColours = new();
                if (FindResource("ColourRect") is Style ColourRect)
                {
                    //Create the button for opening the colour menu
                    colourButton = new Button
                    {
                        Width = 48,
                        Height = 10,
                        Style = ColourRect
                    };
                    colourButton.Click += (_, _) => { ColourPopup(colourButton); };
                    g.Children.Add(colourButton);
                }
            }
            alternateColours.Add(objectInfo.colour, objectInfo);
        }

        private void ColourPopup(Button colourButton)
        {
            if(colourPopup == null)
            {
                colourPopup = new Popup
                {
                    PlacementTarget = colourButton,
                    Placement = PlacementMode.Bottom,
                    StaysOpen = false
                };
                var cPInterior = new ColourPopup(selectedColourFunct, rightClickColourFunct, this);
                colourPopup.Child = cPInterior;
                colourPopup.MouseLeave += Popup_MouseLeave;
            }
            colourPopup.IsOpen = !colourPopup.IsOpen;
        }
        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!colourButton.IsMouseDirectlyOver)
                colourPopup.IsOpen = false; // Close when the mouse leaves the popup
        }
    }
}
