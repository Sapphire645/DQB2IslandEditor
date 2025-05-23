using DQB2IslandEditor.InterfacePK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DQB2IslandEditor.ObjectPK
{
    public abstract class ObjectInfo : INotifyPropertyChanged
    {
        public readonly ushort objectId;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangedEventHandler? InventoryImageChanged;
        public ImageSource objectInventoryImage {
            //Try to get image, if fail, then have to call async load
            get {
                if (_loadingImageInventory) return null;
                if (_weakReferenceImageInventory == null)
                {
                    GetObjectInventoryImage();
                    return null;
                }
                else
                {
                    if (_weakReferenceImageInventory.TryGetTarget(out var image))
                        return image;
                    else
                    {
                        GetObjectInventoryImage();
                        return null;
                    }
                }
                
            } set
            {
                _loadingImageInventory = false;
                if (_weakReferenceImageInventory == null)
                    _weakReferenceImageInventory = new WeakReference<ImageSource>(value);
                else
                    _weakReferenceImageInventory.SetTarget(value);
                InventoryImageChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(objectInventoryImage)));
            }
        }
        public ImageSource objectMapImage
        {
            //Try to get image, if fail, then have to call async load
            get
            {
                if (_loadingImageMap) return null;
                if (_weakReferenceImageMap == null)
                {
                    GetObjectMapImage();
                    return null;
                }
                else
                {
                    if (_weakReferenceImageMap.TryGetTarget(out var image))
                        return image;
                    else
                    {
                        GetObjectMapImage();
                        return null;
                    }
                }

            }
            set
            {
                _loadingImageMap = false;
                if (_weakReferenceImageMap == null)
                    _weakReferenceImageMap = new WeakReference<ImageSource>(value);
                else
                    _weakReferenceImageMap.SetTarget(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(objectMapImage)));
            }
        }

        bool _loadingImageMap = false;
        bool _loadingImageInventory = false;
        private async void GetObjectMapImage()
        {
            _loadingImageMap = true;
            Task databaseTask = Task.Run(() => DataBaseReading.GetObjectMapImage(this));
        }
        private async void GetObjectInventoryImage()
        {
            _loadingImageInventory = true;
            Task databaseTask = Task.Run(() => DataBaseReading.GetObjectInventoryImage(this));
        }

        public string name { get; }
        public Colour colour { get; }

        public readonly byte tab; //Filter
        public readonly short imageId; //For future dymanic coding image stuff 

        public string normalDrop { get; private set; }
        public string hardness { get; private set; }
        public string ultimalletDrop { get; private set; }
        public string gameDescription { get; private set; }
        public string description { get; private set; }

        public string fullName => name + (colour != 0 ? " [" + colour + "] " : " ") + objectIdDisplay;
        public string objectIdDisplay => "{" + objectId + "}";


        protected WeakReference<ImageSource> _weakReferenceImageMap = null;
        protected WeakReference<ImageSource> _weakReferenceImageInventory = null;

        public ObjectInfo(ushort objectId, short imageId, 
            byte tab, Colour colour, string name)
        {
            this.objectId = objectId;
            this.imageId = imageId;
            this.tab = tab;
            this.colour = colour;
            this.name = name;
        }

        public void UpdateExtraData(string hardness, string normalDrop, string ultimalletDrop, string gameDescription, string description)
        {
            this.hardness = hardness;
            this.normalDrop = normalDrop;
            this.ultimalletDrop = ultimalletDrop;
            this.gameDescription = gameDescription;
            this.description = description;
        }

        public Brush colourBrush => colour == (Colour)0 ? Brushes.DarkGray :
            (colour == (Colour)1 ? Brushes.White :
            (colour == (Colour)2 ? ((Brush)new BrushConverter().ConvertFromString("#404040")) :
            (colour == (Colour)3 ? Brushes.DarkMagenta :
            (colour == (Colour)4 ? ((Brush)new BrushConverter().ConvertFromString("#FF0094")) :
            (colour == (Colour)5 ? Brushes.Red :
            (colour == (Colour)6 ? Brushes.Lime :
            (colour == (Colour)7 ? Brushes.Yellow :
            Brushes.Blue)))))));
        public Brush colourBrushLight => colour == (Colour)0 ? Brushes.DarkGray :
            (colour == (Colour)1 ? Brushes.FloralWhite :
            (colour == (Colour)2 ? Brushes.DimGray :
            (colour == (Colour)3 ? Brushes.MediumOrchid:
            (colour == (Colour)4 ? Brushes.HotPink:
            (colour == (Colour)5 ? Brushes.LightCoral:
            (colour == (Colour)6 ? Brushes.PaleGreen:
            (colour == (Colour)7 ? Brushes.PaleGoldenrod:
            Brushes.CornflowerBlue)))))));
        public Visibility colourTag => colour != (Colour)0 ? Visibility.Visible : Visibility.Collapsed;

    }
}
