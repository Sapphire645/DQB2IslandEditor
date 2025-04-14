using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.DataPK
{
    public class SaveData
    {
        private const uint STGDAT_SIZE_HEADER = 0x110;
        private const string CONST_HEADER = "aerC"; //It is bigger.

        private const uint CMNDAT_SIZE_HEADER = 0x2A444;

        private const uint OFF_MINIMAP_IMAGE = 0x24a60b;
        private const uint SIZE_MINIMAP_IMAGE = 0x20000;
        private readonly (byte,byte)[] MINIMAP_CORRESPONDANCE = [
            (0, 1), //0 -> ISLE OF AWAKENING (STGDAT01)
            (1, 2), //1 -> FURROWFIELD (STGDAT02)
            (2, 3), //2 -> KHRUMBUL-DUN (STGDAT03)
            (3, 4), //3 -> MOONBROOKE (STGDAT04)
            (4, 5), //4 -> MALHALLA (STGDAT05)
            (7, 9),  // Angler
            (8, 10),  //Skelk
            (10, 12), //B1
            (11, 13), //B2
            (12, 14), //ATOLL
            (13, 16) //B3
                     ];

        private string folderPath;
        private string CMNDATpath;

        public string FolderPath => folderPath;

        public Island Island;
        public Dictionary<byte, IslandShell> islandCMNDATdata;

        public SaveData(string CMNDATpath) {
            //Get the folder.
            if (CMNDATpath.EndsWith("\\CMNDAT.BIN"))
                folderPath = CMNDATpath.Remove(CMNDATpath.Length - "\\CMNDAT.BIN".Length);
            else if (CMNDATpath.EndsWith("\\AUTOCMNDAT.BIN"))
                folderPath = CMNDATpath.Remove(CMNDATpath.Length - "\\AUTOCMNDAT.BIN".Length);
            this.CMNDATpath = CMNDATpath;

            OpenCMNDATCompressedFile();
        }

        public bool ValidSTGDAT(byte island)
        {
            return System.IO.File.Exists(folderPath + "\\STGDAT" + island.ToString("D2") + ".BIN");
        }
        public bool ValidSTGDAT()
        {
            return System.IO.File.Exists(folderPath + "\\AUTOSTGDAT.BIN");
        }

        static public bool ValidCMNDAT(string path)
        {
            //Check if exists.
            if (!System.IO.File.Exists(path)) return false;
            //Read
            byte[] headerBytes = new byte[4];
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                fs.Read(headerBytes, 0, 4);

            //Check if header start is the same. This is the cmndat identifier.
            var actualHeader = Encoding.UTF8.GetString(headerBytes);
            if (CONST_HEADER != actualHeader) return false;
            return true;
        }
        public void OpenCMNDATCompressedFile()
        {
            if (!System.IO.Directory.Exists(folderPath)) return;
            //This will open the file.
            var compressedFileBytes = OpenCMNDATFile(CMNDATpath);
            //Return if failed.
            if (compressedFileBytes == null) return;

            //We need to uncompress now.
            var uncompressedFileBytes = UncompressBytes(compressedFileBytes);

            islandCMNDATdata = new Dictionary<byte, IslandShell>();
            //Get the Minimaps (very important)
            foreach (var islandMinimapOffset in MINIMAP_CORRESPONDANCE)
            {
                var minimapBytes = new byte[SIZE_MINIMAP_IMAGE];
                Array.Copy(uncompressedFileBytes, OFF_MINIMAP_IMAGE + islandMinimapOffset.Item1*SIZE_MINIMAP_IMAGE, minimapBytes, 0, SIZE_MINIMAP_IMAGE);
                islandCMNDATdata[islandMinimapOffset.Item2] = new IslandShell(islandMinimapOffset.Item2, minimapBytes);
            }
        }
        private byte[] OpenCMNDATFile(string path)
        {

            if (!ValidCMNDAT(path)) return null;
            var fileBytes = System.IO.File.ReadAllBytes(path);

            //Now remove the real header. Right now its not used for anything.
            var dataBytes = new byte[fileBytes.Length - CMNDAT_SIZE_HEADER];
            Array.Copy(fileBytes, CMNDAT_SIZE_HEADER, dataBytes, 0, dataBytes.Length);

            return dataBytes;
        }
        public void OpenSTGDATCompressedFile(byte island)
        {
            if (!System.IO.Directory.Exists(folderPath)) return;
            //This will open the file.
            var compressedFileBytes = OpenSTGDATFile(folderPath + "\\STGDAT" + island.ToString("D2") + ".BIN");
            //Return if failed.
            if (compressedFileBytes == (null, null)) return;

            //We need to uncompress now.
            var uncompressedFileBytes = UncompressBytes(compressedFileBytes.Item2);

            //Now we load in the data
            Island = new Island(compressedFileBytes.Item1, uncompressedFileBytes, 0, islandCMNDATdata[island]);
        }
        public void OpenSTGDATCompressedFile() //AutoSTGDAT
        {
            if (!System.IO.Directory.Exists(folderPath)) return;
            //This will open the file.
            var compressedFileBytes = OpenSTGDATFile(folderPath + "\\AUTOSTGDAT.BIN");
            //Return if failed.
            if (compressedFileBytes == (null,null)) return;

            //We need to uncompress now.
            var uncompressedFileBytes = UncompressBytes(compressedFileBytes.Item2);

            //Now we load in the data
            Island = new Island(compressedFileBytes.Item1, uncompressedFileBytes, 0, islandCMNDATdata); //Oh no. This would be on auto cmndat... 
        }
        public void OpenSTGDATCompressedFile(string path)
        {
            //This will open the file.
            var compressedFileBytes = OpenSTGDATFile(path);
            //Return if failed.
            if (compressedFileBytes == (null, null)) return;

            //We need to uncompress now.
            var uncompressedFileBytes = UncompressBytes(compressedFileBytes.Item2);

            //Now we load in the data
            Island = new Island(compressedFileBytes.Item1, uncompressedFileBytes, 0, islandCMNDATdata);
        }

        /*
         This contains the beggining process of reading a file. Both exported and compressed.
         */
        private (byte[], byte[]) OpenSTGDATFile(string path)
        {
            //Check if exists.
            if (!System.IO.File.Exists(path)) return (null,null);
            //Read
            var fileBytes = System.IO.File.ReadAllBytes(path);

            //Check if header start is the same. This is the STGDAT identifier.
            var headerBytes = new byte[4];
            Array.Copy(fileBytes, 0, headerBytes, 0, 4);
            var actualHeader = Encoding.UTF8.GetString(headerBytes);
            if (CONST_HEADER != actualHeader) return (null, null);

            //Now remove the real header. Right now its not used for anything.
            var dataBytes = new byte[fileBytes.Length - STGDAT_SIZE_HEADER];
            Array.Copy(fileBytes, STGDAT_SIZE_HEADER, dataBytes, 0, dataBytes.Length);
            headerBytes = new byte[STGDAT_SIZE_HEADER]; //jfreaggtgh
            Array.Copy(fileBytes, 0, headerBytes, 0, STGDAT_SIZE_HEADER);

            return (headerBytes, dataBytes);
        }

        public void SaveSTGDATCompressedFile(string path)
        {

            //Sigh

            //Backup...
            var fileBytes = Island.GetBytes();
            //Compress
            var compressedFileBytes = CompressBytes(fileBytes.Item2);

            var file = new byte[fileBytes.Item1.Length + compressedFileBytes.Length];
            Array.Copy(fileBytes.Item1, 0, file, 0, STGDAT_SIZE_HEADER);
            Array.Copy(compressedFileBytes, 0, file, STGDAT_SIZE_HEADER, compressedFileBytes.Length);

            if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Backup"))) Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Backup"));
            System.IO.File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "Backup\\" + DateTime.Now.ToString("yyyy-MM-dd_HH;mm")+"-STGDAT"+ Island.islandNumber.ToString("D2")+".BIN"), file);

            //Then
            fileBytes = Island.CommitChangesToFile();
            //Compress
            compressedFileBytes = CompressBytes(fileBytes.Item2);

            file = new byte[fileBytes.Item1.Length + compressedFileBytes.Length];
            Array.Copy(fileBytes.Item1, 0, file, 0, STGDAT_SIZE_HEADER);
            Array.Copy(compressedFileBytes, 0, file, STGDAT_SIZE_HEADER, compressedFileBytes.Length);
            
            System.IO.File.WriteAllBytes(path, file);
        }

        private byte[] UncompressBytes(byte[] compressedFileBytes)
        {
            byte[] _uncompressedBytes;
            //Gonna be honest no idea how this works, I took it from github.com/turtle-insect/DQB2.
            //It is a fast decompression code using memstream instead of the library function.
            using (var input = new MemoryStream(compressedFileBytes))
            {
                using (var zlib = new System.IO.Compression.ZLibStream(input, System.IO.Compression.CompressionMode.Decompress))
                {
                    using (var output = new MemoryStream())
                    {
                        zlib.CopyTo(output);
                        zlib.Flush();
                        _uncompressedBytes = output.ToArray();
                    }
                }
            }
            return _uncompressedBytes;
        }
        private byte[] CompressBytes(byte[] uncompressedFileBytes)
        {
            byte[] _compressedBytes;
            //Gonna be honest no idea how this works, I took it from github.com/turtle-insect/DQB2.
            //It is a fast decompression code using memstream instead of the library function.
            using (var input = new MemoryStream(uncompressedFileBytes))
            {
                using (var output = new MemoryStream())
                {
                    using (var zlib = new System.IO.Compression.ZLibStream(output, System.IO.Compression.CompressionLevel.Fastest))
                    {
                        input.CopyTo(zlib);
                        zlib.Flush();
                        _compressedBytes = output.ToArray();
                    }
                    
                }
            }
            return _compressedBytes;
        }
        //private void CorruptionCheck()
        //{
        //    if (blockBytes.Length / SIZE_CHUNK != chunkCount)
        //        Console.WriteLine("WARNING: Real Chunk Counter differs from the amount of chunks stored. Its unknown if this may cause errors.");
        //}
    }
}
