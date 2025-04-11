using System.Windows.Controls;
using System.Windows.Data;


namespace DQB2IslandEditor.ObjectPK.Container
{
    /// <summary>
    /// Interaction logic for ColourPopup.xaml
    /// </summary>
    public partial class ColourPopup : UserControl
    {
        InventoryContainer dad;
        public ColourPopup(Action<ObjectInfo>? selectedColourFunct, Action<ObjectInfo>? rightClickColourFunct, InventoryContainer dad)
        {
            InitializeComponent();
            this.dad = dad;
            White.Click += (_, _) => { selectedColourFunct(dad.alternateColours[(Colour)1]); };
            Black.Click += (_, _) => { selectedColourFunct(dad.alternateColours[(Colour)2]); };
            Purple.Click += (_, _) => { selectedColourFunct(dad.alternateColours[(Colour)3]); };
            Pink.Click += (_, _) => { selectedColourFunct(dad.alternateColours[(Colour)4]); };
            Red.Click += (_, _) => { selectedColourFunct(dad.alternateColours[(Colour)5]); };
            Green.Click += (_, _) => { selectedColourFunct(dad.alternateColours[(Colour)6]); };
            Yellow.Click += (_, _) => { selectedColourFunct(dad.alternateColours[(Colour)7]); };
            Blue.Click += (_, _) => { selectedColourFunct(dad.alternateColours[(Colour)8]); };

            White.MouseRightButtonDown += (_, _) => { rightClickColourFunct(dad.alternateColours[(Colour)1]); };
            Black.MouseRightButtonDown += (_, _) => { rightClickColourFunct(dad.alternateColours[(Colour)2]); };
            Purple.MouseRightButtonDown += (_, _) => { rightClickColourFunct(dad.alternateColours[(Colour)3]); };
            Pink.MouseRightButtonDown += (_, _) => { rightClickColourFunct(dad.alternateColours[(Colour)4]); };
            Red.MouseRightButtonDown += (_, _) => { rightClickColourFunct(dad.alternateColours[(Colour)5]); };
            Green.MouseRightButtonDown += (_, _) => { rightClickColourFunct(dad.alternateColours[(Colour)6]); };
            Yellow.MouseRightButtonDown += (_, _) => { rightClickColourFunct(dad.alternateColours[(Colour)7]); };
            Blue.MouseRightButtonDown += (_, _) => { rightClickColourFunct(dad.alternateColours[(Colour)8]); };
        }
    }
}
