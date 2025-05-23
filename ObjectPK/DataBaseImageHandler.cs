using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DQB2IslandEditor.ObjectPK
{
    public class DataBaseImageHandler
    {
        private BitmapImage _sheetOne;
        private BitmapImage _sheetTwo;
        private byte SIZE;
        private String ERR_PATH;
        private byte SHEET_DIMENSION;
        private bool tile;
        private Dictionary<ushort, WeakReference<CroppedBitmap>> _storage = new Dictionary<ushort, WeakReference<CroppedBitmap>>();
        private readonly object _lock = new object();
        private readonly object _lockStorage = new object();
        private Dictionary<ushort, object> _storageLoading = new Dictionary<ushort, object>();

        public DataBaseImageHandler(BitmapImage _sheetOne,
            BitmapImage _sheetTwo, byte SIZE, String ERR_PATH, byte sHEET_DIMENSION, bool tile)
        {
            this._sheetOne = _sheetOne;
            this._sheetTwo = _sheetTwo;
            this.SIZE = SIZE;
            this.ERR_PATH = ERR_PATH;
            SHEET_DIMENSION = sHEET_DIMENSION;
            this.tile = tile;
        }

        public async void GetObjectImage(ObjectInfo parent)
        {
            int ImageID = parent.imageId;
            object currentLock;
            //Stop from loading two at the same time. hopefully does not slow down too much since its just 1 lock
            //Get if the image is loading somewhere else
            lock (_lock)
            {
                if (_storageLoading.ContainsKey((ushort)ImageID))
                    currentLock = _storageLoading[(ushort)ImageID];
                else
                {
                    currentLock = new object();
                    _storageLoading.Add((ushort)ImageID, currentLock);
                }
            }
            //If it is loading somewhere else, it will get blocked until it is not.
            lock (currentLock)
            {
                //Check the storage.
                lock (_lockStorage)
                {
                    if (_storage.ContainsKey((ushort)ImageID))
                        if (_storage[(ushort)ImageID].TryGetTarget(out CroppedBitmap IconDone))
                        {
                            //If suceeded then go ahead.
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                if (!tile)
                                    parent.objectInventoryImage = IconDone;
                                else
                                    parent.objectMapImage = IconDone;
                            }));
                            return;
                        }
                }
                //No success, we have to create it.
                CroppedBitmap Icon;
                try
                {
                    var _sheet = _sheetOne;
                    if (ImageID >= SHEET_DIMENSION * SHEET_DIMENSION)
                    {
                        ImageID = ImageID % (SHEET_DIMENSION * SHEET_DIMENSION);
                        _sheet = _sheetTwo;
                    }
                    Icon = new CroppedBitmap(_sheet, new Int32Rect((ImageID % SHEET_DIMENSION) * SIZE, (ImageID / SHEET_DIMENSION) * SIZE, SIZE, SIZE));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(ImageID);
                    Icon = new CroppedBitmap(new BitmapImage(new Uri("pack://application:,,,/" + ERR_PATH)), new Int32Rect(0, 0, SIZE, SIZE));
                }
                Icon.Freeze();

                lock (_lockStorage)
                {
                    if (_storage.ContainsKey((ushort)ImageID))
                        _storage[(ushort)ImageID].SetTarget(Icon);
                    else
                        _storage.Add((ushort)ImageID, new WeakReference<CroppedBitmap>(Icon));
                }
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (!tile)
                        parent.objectInventoryImage = Icon;
                    else
                        parent.objectMapImage = Icon;
                }));
            }
        }
    }
}
