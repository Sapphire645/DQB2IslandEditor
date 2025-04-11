using DQB2IslandEditor.InterfacePK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DQB2IslandEditor.ObjectPK
{
    public abstract class ObjectInfo 
    {
        public readonly ushort objectId;
        public ImageSource objectInventoryImage { get; }
        public ImageSource objectMapImage { get; }
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
        public ObjectInfo(ushort objectId, short imageId, 
            byte tab, Colour colour, string name,
            ImageSource objectInventoryImage, ImageSource objectMapImage)
        {
            this.objectId = objectId;
            this.imageId = imageId;
            this.tab = tab;
            this.colour = colour;
            this.name = name;
            this.objectInventoryImage = objectInventoryImage;
            this.objectMapImage = objectMapImage;
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
