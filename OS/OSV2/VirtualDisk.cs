using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSV2
{
    internal class VirtualDisk
    {
        private const int clusterSize = MiniFat.clusterSize; //the Size Cluster in Virtual Disk

        private static string Disk = "Team.txt"; //Default Virtual Disk

        public static bool isNew = false;

        public static void InitializeOrOpenFile(string path)//Create or Open Virtual Disk without File Name "<User Input>" 
        {
            Disk = !string.IsNullOrEmpty(path) ? path : Disk;

            if (!File.Exists(Disk))//If File Not Found
            {
                using FileStream file = new(Disk, FileMode.Create, FileAccess.ReadWrite); //Create New File With Name path
                isNew = true;//Check the file Is New
                Console.WriteLine($"Creating a new file : {Disk}");
            }
            else
            {
                using FileStream file = new(Disk, FileMode.Open, FileAccess.ReadWrite); //If Found >> Open File 
                isNew = false;//Check the File Is Old
                Console.WriteLine($"Opening existing file : {Disk}");
            }
        }

        public static void WriteCluster(byte[] data, int clusterIndex) //Write In Cluster with Size 1024 byte
        {
            using FileStream file = new(Disk, FileMode.OpenOrCreate, FileAccess.Write); //To Open File
            file.Seek(clusterIndex * clusterSize, SeekOrigin.Begin); //To Go the Pointer in Start point to write 
            file.Write(data, 0 , clusterSize); //to start write from first cluster to end (1024 byte)
            file.Flush();
        }

        public static byte[] ReadCluster(int clusterIndex)//Read In Cluster with Size 1024 byte
        {
            using FileStream file = new(Disk, FileMode.OpenOrCreate, FileAccess.Read);//to open file to read
            file.Seek(clusterIndex * clusterSize, SeekOrigin.Begin);//To Go the Pointer in Start point
            byte[] clusterData = new byte[clusterSize];//initialize byte array
            file.Read(clusterData, 0, clusterSize);//to start read from first cluster to end (1024 byte)
            return clusterData;
        }

        public static bool IsNew()//Check If File is New Open or Not
        {
            return isNew;
        }
    }
}
/// <summary>
/// Represents a virtual disk system that simulates a physical storage device.
/// This class provides functionalities for creating, opening, reading from, 
/// and writing to a virtual disk file. The storage is divided into fixed-size 
/// clusters for easy management, and it integrates with the `Mini_FAT` system.
/// 
/// Key Features:
/// - **Cluster Size**: Each cluster has a fixed size defined by `Mini_FAT.totalClusterSize`.
/// - **Virtual Disk File**: Default disk name is "Tomas.txt", but it can be overridden by specifying a custom path.
/// - **New Disk Check**: Determines whether the disk is newly created or an existing one.
/// 
/// Methods:
/// - **InitializeOrOpenFile(string path)**:
///   Creates a new virtual disk file if it does not exist or opens an existing one.
///   - Parameters: 
///     - `path`: Path of the virtual disk file. Uses the default if empty.
///   - Behavior:
///     - If the disk does not exist, it creates a new file and sets `isNew` to `true`.
///     - If the disk exists, it opens the file and sets `isNew` to `false`.
/// 
/// - **WriteCluster(byte[] data, int clusterIndex)**:
///   Writes data to a specific cluster in the virtual disk.
///   - Parameters:
///     - `data`: The byte array to be written.
///     - `clusterIndex`: The index of the cluster (starting from 0).
///   - Behavior:
///     - Writes data to the specified cluster, ensuring it is exactly `clusterSize` bytes.
/// 
/// - **ReadCluster(int clusterIndex)**:
///   Reads data from a specific cluster in the virtual disk.
///   - Parameters:
///     - `clusterIndex`: The index of the cluster to read.
///   - Returns:
///     - A byte array of size `clusterSize` containing the cluster's data.
/// 
/// - **is_New()**:
///   Checks if the virtual disk is newly created.
///   - Returns:
///     - `true` if the disk is new, otherwise `false`.
/// 
/// Dependencies:
/// - **Mini_FAT**: Uses its `totalClusterSize` constant for defining cluster size.
/// - **FileStream**: Handles low-level file operations such as reading and writing to the disk.
/// 
/// Usage:
/// 1. Call `InitializeOrOpenFile()` to set up the virtual disk.
/// 2. Use `WriteCluster` and `ReadCluster` to manage cluster-level data.
/// 3. Check `is_New()` to determine whether the file system needs initialization.
/// 
/// Limitations:
/// - Assumes the file size is consistent with the cluster-based storage model.
/// - No exception handling for file-related errors such as permission issues.
/// - Depends on the `Mini_FAT` class for cluster size configuration.
/// 
/// Notes:
/// - The cluster size is dynamically tied to `Mini_FAT.totalClusterSize`, enhancing flexibility.
/// - This implementation ensures compatibility with the FAT system for file allocation and management.
/// 
/// </summary>
