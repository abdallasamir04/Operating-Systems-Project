using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSV2
{
    internal class Converter
    {
        /// <summary>
        /// Converts a string message to a byte array using ASCII encoding.
        /// This method transforms each character in the string into its corresponding byte representation.
        /// </summary>
        /// <param name="message">The string message to be converted.</param>
        /// <returns>A byte array representing the string in ASCII encoding.</returns>
        public static byte[] ConvertCharToByte(string message) //Convert from string to byte
        {
            return Encoding.UTF8.GetBytes(message);
        }

        /// <summary>
        /// Converts a byte array to a string using ASCII encoding.
        /// This method decodes the byte array into a string by interpreting each byte as a character.
        /// </summary>
        /// <param name="message">The byte array to be converted.</param>
        /// <returns>A string decoded from the byte array using ASCII encoding.</returns>
        public static string ConvertByteToString(byte[] message) //Convert from byte to character
        {
            return Encoding.UTF8.GetString(message);/*BitConverter.ToString(message)*/;
        }

        /// <summary>
        /// Converts a list of integers to a list of bytes, suitable for storing in a virtual disk's FAT.
        /// This method transforms each integer into a byte array and adds it to a list of bytes.
        /// </summary>
        /// <param name="intList">The list of integers to be converted.</param>
        /// <returns>A list of bytes representing the integers.</returns>
        public static List<byte> ConvertIntListToByteList(List<int> intList) //Convert from int fat to byte fat to save in visual disk
        {
            List<byte> byteList = new List<byte>();//Create New List byte to add byte fat array
            foreach (int num in intList)
            {
                byteList.AddRange(BitConverter.GetBytes(num));//Convert from int array to byte array with range int array
            }
            return byteList;
        }

        /// <summary>
        /// Converts a byte array to an integer array, with each integer representing 4 bytes.
        /// This method reads the byte array in chunks of 4 bytes and converts each chunk into an integer.
        /// </summary>
        /// <param name="byteArray">The byte array to be converted.</param>
        /// <param name="length">The length of the resulting integer array.</param>
        /// <returns>An integer array representing the byte array.</returns>
        public static int[] ByteArrayToIntArray(byte[] byteArray, int length)//Convert from byte array to int array to save in FAT array
        {
            int[] intArray = new int[length];//create int array to add int number in array
            for (int i = 0; i < length; i++)
            {
                intArray[i] = BitConverter.ToInt32(byteArray, i * 4);//convert byte to int and use (4) cause 1 int = 4 byte
            }
            return intArray;
        }

        /// <summary>
        /// Splits a byte array into smaller byte arrays (clusters) based on a specified cluster size.
        /// This method divides a large byte array into smaller clusters of a predefined size.
        /// </summary>
        /// <param name="BigData">The byte array to be split.</param>
        /// <returns>A list of byte arrays, each representing a cluster of data.</returns>
        public static List<byte[]> SplitBytes(byte[] BigData) //To Divide Byte To List with cluster size
        {
            List<byte[]> result = new List<byte[]>();//Create list byte array to save cluster data with size
            byte[] bytes = new byte[MiniFat.clusterSize];//Create array with size cluster size all data = 0 
            for (int i = 0; i < BigData.Length; i++)
            {
                bytes[i % MiniFat.clusterSize] = BigData[i];//Replace zero with value in array bigdata
                if ((i + 1) % MiniFat.clusterSize == 0 || i + 1 == BigData.Length)//if index array is multiple cluster size or end the array
                {
                    result.Add(bytes);//add array in list
                    bytes = new byte[MiniFat.clusterSize];//create new array with all data = 0 
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a directory entry object to a byte array suitable for storing on a disk.
        /// This method serializes a DirectoryEntry object into a fixed-size byte array.
        /// </summary>
        /// <param name="directory">The directory entry to be converted.</param>
        /// <returns>A byte array representing the directory entry.</returns>
        public static byte[] DirToByte(DirectoryEntry directory) //Convert Content Directory to Array of byte
        {
            byte[] bytes = new byte[32];//Create array to save the information directory

            //Convert name from string to bytes
            byte[] name = ConvertCharToByte(new string(directory.Name));
            Array.Copy(name, 0, bytes, 0, name.Length);
            //Copy Attributes bytes
            bytes[11] = directory.Attr;
            //Copy Empty bytes
            Array.Copy(directory.Empty, 0, bytes, 12, directory.Empty.Length);
            //Convert first cluster from int to bytes
            byte[] firstClusterBytes = BitConverter.GetBytes(directory.firstCluster);
            Array.Copy(firstClusterBytes, 0, bytes, 12 + directory.Empty.Length, firstClusterBytes.Length);
            //Convert size from int to bytes
            byte[] sizeBytes = BitConverter.GetBytes(directory.Size);
            Array.Copy(sizeBytes, 0, bytes, 28, sizeBytes.Length);

            return bytes;
        }

        /// <summary>
        /// Converts a list of byte data representing directory entries into a list of DirectoryEntry objects.
        /// This method reads the byte data in chunks of 32 bytes, with each chunk representing a directory entry.
        /// </summary>
        /// <param name="byteArray">The list of bytes representing directory entries.</param>
        /// <returns>A list of DirectoryEntry objects created from the byte data.</returns>
        public static List<DirectoryEntry> DirByteToDirEntry(List<byte> byteArray)//Convert from byte data in virtual disk to directory array
        {
            List<DirectoryEntry> dirEntries = new List<DirectoryEntry>();//Create Directory array to save object with data type directory
            for (int i = 0; i < byteArray.Count; i += 32)//any directory information include 32 byte
            {
                byte[] b = byteArray.Skip(i).Take(32).ToArray();//take 32 byte to array b to easy work

                if (b.Length < 32 || b[0] == 0)
                    break;
                //Convert name from byte to string
                string name = ConvertByteToString(b.Take(11).ToArray()).Trim();
                //Copy attributes
                byte attr = b[11];
                //Convert first cluster from byte to int
                int firstCluster = BitConverter.ToInt32(b, 24);
                //Convert size from byte to int
                int Size = BitConverter.ToInt32(b, 28);
                DirectoryEntry entry = new DirectoryEntry(name.Trim(), attr, firstCluster, Size);//Create object directory entry
                //add the object to array
                dirEntries.Add(entry);
            }
            return dirEntries;
        }
    }
}
