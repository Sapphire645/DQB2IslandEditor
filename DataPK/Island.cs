using DQB2IslandEditor.InterfacePK;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DQB2IslandEditor.DataPK
{
    public class Island
    {
        private const uint OFF_ISLAND_NUMBER = 0xC0ED6;

        private const uint OFF_RCHUNK_COUNT = 0x1451AF;
        private const uint SIZE_RCHUNK_COUNT = 2;

        private const uint OFF_VCHUNK_COUNT = 0x24E7C5;
        private const uint SIZE_VCHUNK_COUNT = 2;
        public static readonly byte GRID_DIMENSION = 64;

        private const uint OFF_VCHUNK_GRID = 0x24C7C1;
        private const uint SIZE_VCHUNK_GRID = 0x2000;

        private const uint SIZE_ITEM_ALLOCATION = 0xC8000;
        private const uint OFF_ITEM_DATA = 0x24E7D1;
        private const uint SIZE_ITEM_DATA = 24;
        private const uint OFF_ITEM_ENTRY = 0x150E7D1;
        private const uint SIZE_ITEM_ENTRY = 4;

        private const uint OFF_BLOCK_DATA = 0x183FEF0;
        private const uint SIZE_CHUNK = 0x30000;

        public byte islandNumber { get; private set; }
        private ushort virtualChunkCount;
        private ushort realChunkCount;
        public IslandShell shell { get; private set; }
        public byte[] STGDATHeader { get; private set; }
        public byte[] STGDATBody { get; private set; }

        public string STGDATPath { get; private set; }


        private Chunk[] chunks = new Chunk[GRID_DIMENSION * GRID_DIMENSION];

        public Island(byte[] headerBytes, byte[] fileBytes,byte TYPE, IslandShell shell, string sTGDATPath) //Type: I cannot make multiple ones because they have the same args
        {
            STGDATHeader = headerBytes;
            STGDATBody = fileBytes;
            this.shell = shell;
            this.STGDATPath = STGDATPath;
            //Island Number:
            islandNumber = fileBytes[OFF_ISLAND_NUMBER];

            //
            switch (TYPE)
            {
                case 0: //The chunk editor reading
                    ConstructChunkData(fileBytes);
                    ConstructItemData(fileBytes);
                    break;
                default:
                    break;
            }
            Console.WriteLine(ToString());
            STGDATPath = sTGDATPath;
        }

        public Island(byte[] headerBytes, byte[] fileBytes, byte TYPE, Dictionary<byte,IslandShell> shell, string sTGDATPath) //Type: I cannot make multiple ones because they have the same args
        {
            STGDATHeader = headerBytes;
            STGDATBody = fileBytes;
            this.STGDATPath = STGDATPath;
            //Island Number:
            islandNumber = fileBytes[OFF_ISLAND_NUMBER];
            this.shell = shell[islandNumber];
            //
            switch (TYPE)
            {
                case 0: //The chunk editor reading
                    ConstructChunkData(fileBytes);
                    break;
                default:
                    break;
            }
            Console.WriteLine(ToString());
        }

        private void ConstructChunkData(byte[] fileBytes) {
            //Create Chunk grid
            byte[] bufferOne = new byte[SIZE_VCHUNK_COUNT];
            Array.Copy(fileBytes, OFF_VCHUNK_COUNT, bufferOne, 0, SIZE_VCHUNK_COUNT);
            virtualChunkCount = BitConverter.ToUInt16(bufferOne, 0);

            byte[] bufferTwo = new byte[SIZE_VCHUNK_GRID];
            Array.Copy(fileBytes, OFF_VCHUNK_GRID, bufferTwo, 0, SIZE_VCHUNK_GRID);

            //Extract all chunks
            for (int i = 0; i < SIZE_VCHUNK_GRID; i += 2)
            {
                byte[] chunkOffset = new byte[2];
                //Get the offset of the block data
                Array.Copy(fileBytes, OFF_VCHUNK_GRID + i, chunkOffset, 0, 2);
                short offset = (short)((chunkOffset[1] << 8) | chunkOffset[0]);

                if (offset == -1) //If no block data
                    chunks[i / 2] = new Chunk(null, (ushort)(i / 2));
                else
                {
                    //Get the block data for the chunk.
                    byte[] chunkData = new byte[SIZE_CHUNK];
                    try
                    {
                        Array.Copy(fileBytes, OFF_BLOCK_DATA + offset * SIZE_CHUNK, chunkData, 0, SIZE_CHUNK);
                    }
                    catch //Moonbrooke fix.
                    {
                        Array.Copy(fileBytes, OFF_BLOCK_DATA + offset * SIZE_CHUNK, chunkData, 0, fileBytes.Length - (OFF_BLOCK_DATA + offset * SIZE_CHUNK));
                    }

                    chunks[i / 2] = new Chunk(chunkData, (ushort)(i / 2));
                }
            }

            //Get real chunk
            bufferOne = new byte[SIZE_RCHUNK_COUNT];
            Array.Copy(fileBytes, OFF_RCHUNK_COUNT, bufferOne, 0, SIZE_RCHUNK_COUNT);
            realChunkCount = BitConverter.ToUInt16(bufferOne, 0);
        }

        private void ConstructItemData(byte[] fileBytes)
        {
            byte[] bufferItemData = new byte[SIZE_ITEM_DATA];
            //Lets iterate through all of the item entries.
            //The format for the entry is CC OC OO OO where C is chunk and O is the offset to the item data.
            //With this, we'll place the item instance inside its chunk.
            for (uint i = 0; i < SIZE_ITEM_ALLOCATION; i++)
            {
                //Pointer
                uint entryPointer = (uint)(OFF_ITEM_ENTRY + (i * SIZE_ITEM_ENTRY));

                //Extract the information.
                ushort chunk = (ushort)(fileBytes[entryPointer] + ((fileBytes[entryPointer+1] & 0x0F) << 8));
                uint relativeDataPointer = (uint)(((fileBytes[entryPointer + 1] & 0xF0) >> 4) + 
                    (fileBytes[entryPointer + 2] << 4) + (fileBytes[entryPointer + 3] << 12));
                //Now lets extract the item data.

                uint dataPointer = (uint)(OFF_ITEM_DATA + (relativeDataPointer * SIZE_ITEM_DATA));
                Array.Copy(fileBytes, dataPointer, bufferItemData, 0, 24);

                //First lets see what the item class thinks of this entry, is it empty?
                if (ItemInstance.IsThisEntryEmpty(bufferItemData)) continue;

                //Since it is now, We can now create the item instance inside of the chunk.
                //Console.WriteLine($"Item Entry: {i} {entryPointer} Chunk: {chunk} Offset: {relativeDataPointer}");
                chunks[chunk].AddItem(new ItemInstance(bufferItemData));
                //Yay.
            }
        }
        private void CommitChunkDataToFile()
        {
            //VIRTUAL CHUNK COUNT: HOLD OFF ON THAT ONE FOR NOW. 
            uint currentGridPointer = OFF_VCHUNK_GRID;
            uint currentBlockPointer = OFF_BLOCK_DATA;
            uint chunkCount = 0;

            for (int i = 0; i < chunks.Length; i++)
            {
                var chunk = chunks[i];
                if (chunk.IsEmpty())
                {
                    //Set the offset to -1
                    STGDATBody[currentGridPointer] = 0xFF;
                    STGDATBody[currentGridPointer + 1] = 0xFF;
                }
                else
                {
                    //Set the offset to the current block pointer, then copy the chunks at the end
                    STGDATBody[currentGridPointer] = (byte)(chunkCount & 0xFF);
                    STGDATBody[currentGridPointer + 1] = (byte)((chunkCount >> 8) & 0xFF);

                    Array.Copy(chunk.GetBytes(), 0, STGDATBody, currentBlockPointer, SIZE_CHUNK);
                    currentBlockPointer += SIZE_CHUNK;
                    chunkCount++;
                }
                currentGridPointer += 2;
            }

            //Do real chunk count later.
        }
        public Chunk GetChunk(ushort vChunk)
        {
            return chunks[vChunk];
        }

        //For loading in the island for the first time.
        public ushort GetFirstChunk()
        {
            foreach (Chunk chunk in chunks)
                if (!chunk.IsEmpty()) return chunk.chunkPosition;
            return 0; //Oh no
        }

        public bool IsChunkEmpty(ushort vChunk)
        {
            return chunks[vChunk].IsEmpty();
        }

        public CoordinateFrame ChunkGridFrame()
        {
            byte x0 = GRID_DIMENSION;
            byte x1 = 0;
            byte y0 = GRID_DIMENSION;
            byte y1 = 0;
            for(uint i = 0; i < chunks.Length; i++)
                if (!chunks[i].IsEmpty())
                {
                    if(i % GRID_DIMENSION < x0) x0 = (byte)(i % GRID_DIMENSION);
                    if (i % GRID_DIMENSION > x1) x1 = (byte)(i % GRID_DIMENSION);
                    if (i / GRID_DIMENSION < y0) y0 = (byte)(i / GRID_DIMENSION);
                    if (i / GRID_DIMENSION > y1) y1 = (byte)(i / GRID_DIMENSION);
                }
            return new CoordinateFrame(x0, x1, y0, y1);
        }

        public (byte[], byte[]) CommitChangesToFile()
        {
            CommitChunkDataToFile();
            return (STGDATHeader, STGDATBody);
        }

        public (byte[], byte[]) GetBytes()
        {
            return (STGDATHeader, STGDATBody);
        }


        public void CoverGroundWith(BlockInstance source, BlockInstance newindex)
        {
            foreach (var chunk in chunks) { 
                if(chunk.IsEmpty()) continue;
                chunk.CoverGroundWith(source, newindex);
            }
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder("ISLAND = {\n\tIsland ID : ");
            output.Append(islandNumber);
            output.Append(",\n\tCHUNK GRID = {");


            for(int i = 0; i < GRID_DIMENSION; i++) {
                output.Append("\n\t\t");
                for (int j = 0; j < GRID_DIMENSION; j++)
                {
                    output.Append(chunks[i*GRID_DIMENSION+j].IsEmpty() ? "0 " : "1 ");
                }
            }
            output.Append("\t\n}\n}");
            return output.ToString();
        }
    }
}
