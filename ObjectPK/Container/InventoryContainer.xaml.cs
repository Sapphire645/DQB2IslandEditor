using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;


namespace DQB2IslandEditor.ObjectPK.Container
{
    /// <summary>
    /// Interaction logic for InventoryContainer.xaml
    /// </summary>
    public partial class InventoryContainer : UserControl
    {
        public ObjectInfo CurrentObject
        { get { return _currentObject; }
            private set {
                if(_currentObject != null)
                    _currentObject.InventoryImageChanged -= updateImage;
                _currentObject = value;
                _currentObject.InventoryImageChanged += updateImage;
                if(IsLoaded)
                    ImageObject.Source = _currentObject.objectInventoryImage;
                ToolTipObject.objectInfoPanel = _currentObject;
            } }

        private void updateImage(object sender, PropertyChangedEventArgs e)
        {
            if(IsLoaded)
                ImageObject.Source = _currentObject.objectInventoryImage;
        }

        private ObjectInfo _currentObject;
        private UserControl _submenu = null;

        private Action<ObjectInfo> _selectBlock;
        private Action<ObjectInfo> _favouriteBlock;

        public bool hasSubmenu => _submenu != null;
        private Popup colourPopup = null;
        private Border colourBorder = null;
        private bool _isPopupOpen = false;
        public InventoryContainer(ObjectInfo objectInfo, Action<ObjectInfo> selectBlock, Action<ObjectInfo> favouriteBlock)
        {
            DataContext = this;
            
            _selectBlock = selectBlock;
            _favouriteBlock = favouriteBlock;

            InitializeComponent();
            CurrentObject = objectInfo;
        }
        // 0 = Colour

        public void createSubMenu(bool colour, byte type, List<ObjectInfo> subInfos)
        {
            Image icon = new Image()
            {
                Width = 20,
                Height = 20,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                IsHitTestVisible = false
            };
            icon.Source = DataBaseReading.GetInventoryIcon(type);
            g.Children.Add(icon);
            switch (type)
            {
                case 0: //Colour menu
                    if (colour)
                        _submenu = new ColourPopup(SelectedItem, _favouriteBlock, _currentObject, subInfos);
                    break;

            }
        }
        private void ColourPopup()
        {
            if (colourPopup == null)
            {
                colourPopup = new Popup
                {
                    PlacementTarget = this,
                    Placement = PlacementMode.Center,
                    VerticalOffset = this.ActualHeight,
                    StaysOpen = false
                };
                colourPopup.Child = _submenu;
                colourPopup.MouseLeave += Popup_MouseLeave;
            }
            _isPopupOpen = !_isPopupOpen;
            colourPopup.IsOpen = _isPopupOpen;
            if (_isPopupOpen) SlotButton.Background = Brushes.Gold;
        }
        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsMouseOver)
            {
                _isPopupOpen = !_isPopupOpen;
            }
            SlotButton.Background = SlotButton.BorderBrush;
        }

        private void ButtonRightClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _favouriteBlock.Invoke(_currentObject);
        }

        private void ButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if(_submenu == null)
                _selectBlock.Invoke(_currentObject);
            else
            {
                ColourPopup();
            }
        }

        private void SelectedItem(ObjectInfo selected)
        {
            colourPopup.IsOpen = false;
            CurrentObject = selected;
            _selectBlock.Invoke(_currentObject);
            if(_currentObject.colour != Colour.Plain)
            {
                if (colourBorder == null)
                {
                    colourBorder = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Width = 14,
                        Height = 23,
                        CornerRadius = new CornerRadius(4),
                        Margin = new Thickness(5),
                        IsHitTestVisible = false
                    };
                    g.Children.Add(colourBorder);
                }
                colourBorder.Background = _currentObject.colourBrush;
                colourBorder.Visibility = Visibility.Visible;
            }
            else
                if (colourBorder != null) colourBorder.Visibility = Visibility.Collapsed;
        }

        private void LoadImage(object sender, RoutedEventArgs e)
        {
            ImageObject.Source = _currentObject.objectInventoryImage;
        }
    }
}
