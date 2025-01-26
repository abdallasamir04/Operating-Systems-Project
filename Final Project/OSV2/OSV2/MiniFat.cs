using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OSV2
{
    internal class MiniFat
    {
        public const int clusterSize = 1024;//Size Cluster

        public const int totalCluster = 1024;//Total Cluster

        public const int fullCluster = -1;//last Cluster have a data

        public const int emptyCluster = 0;//Empty cluster index

        public const int firstClusterEmptyInNew = 5;//First cluster Empty when start app run is new
        
        static int[] FAT = new int[totalCluster]; //Fat Array

        public static void InitializeFAT() //Initialize Fat Array if New -1 2 3 4 -1 0 0 0...
        {
            for (int i = 0; i < clusterSize; i++)
            {
                FAT[i] = (i == 0 || i == firstClusterEmptyInNew - 1) ? fullCluster : ((i > 0 && i <= firstClusterEmptyInNew - 2) ? i + 1 : emptyCluster);
            }
        }

        public static byte[] CreateSuperBlock() //Create SuperBlock if New Cluster Index 0 = 0000....
        {
            return new byte[clusterSize];
        }

        public static void writeFat() //Write Fat Array
        {
            List<byte[]> FATBytes = Converter.SplitBytes(Converter.ConvertIntListToByteList(FAT.ToList()).ToArray()); //Convert int Fat Array To byte Fat Array
            for (int i = 0; i < FATBytes.Count; i++) //Divide byte Fat Array To 4 Fat Array with Size 1024   
                VirtualDisk.WriteCluster(FATBytes[i], i + 1);
        }

        public static void readFat() //Read Fat Array From Disk
        {
            List<byte> FATData = new();
            for (int i = 1; i <= firstClusterEmptyInNew - 1; i++)//Fat Array For 1 To 4 Cluster
            {
                FATData.AddRange(VirtualDisk.ReadCluster(i));
            }
            FAT = Converter.ByteArrayToIntArray(FATData.ToArray(), clusterSize); //Update Fat Array From File System
        }

        public static void printFAT()// Show All Status's Clusters in Fat Array
        {
            for (int i = 0; i < FAT.Length; i++)
            {
                Console.WriteLine($"FAT[{i}] = {FAT[i]}");
            }
        }

        public static int getAvailableCluster() //Get The First Cluster Empty after cluster 4
        {
            for (int List = firstClusterEmptyInNew; List < clusterSize; List++)
            {
                if (FAT[List] == emptyCluster)
                {
                    return List;
                }
            }
            return fullCluster;
        }

        public static int getAvailableClusters() //Get The Total Clusters Empty
        {
            int Counter = 0;
            for (int List = firstClusterEmptyInNew; List < clusterSize; List++)
            {
                if (FAT[List] == emptyCluster)
                {
                    Counter++;
                }
            }
            return Counter;
        }

        public static int getFreeSize() //Get The Free Size in Disk
        {
            return getAvailableClusters() * clusterSize; //by byte
        }

        public static int getClusterStatus(int clusterIndex)//To get Status of Cluster if(Zero Data = 0 , Full Data = -1 or Complementary Data = n)  
        {
            return (clusterIndex >= 0 && clusterIndex < clusterSize) ? FAT[clusterIndex] : fullCluster;
        }

        public static void setClusterPointer(int clusterIndex, int status)//To tying (Connect) The Clusters Together
        {
            if (clusterIndex >= 0 && clusterIndex < clusterSize && status >= fullCluster && status < clusterSize)
                FAT[clusterIndex] = status;
        }

        public static void CreateOrOpenFatArray() //Read Fat Array if Found or Initialize Fat Array if not found
        {
            if (VirtualDisk.IsNew())//Check if disk is new 
            {
                InitializeFAT();
                VirtualDisk.WriteCluster(CreateSuperBlock(), 0);//Write super block
                writeFat();
            }
            else//if disk found
                readFat();
        }

        public static void CloseTheSystem() // To Close System And Save Fat Array 
        {
            writeFat();
            Environment.Exit(0);
        }
    }
}

/// <summary>
/// Represents a Mini File Allocation Table (FAT) system for managing disk clusters.
/// This class provides functionalities for:
/// - Initializing and managing the FAT table.
/// - Reading and writing the FAT table to a virtual disk.
/// - Querying and updating cluster statuses.
/// - Calculating disk space information.
///
/// Key Features:
/// - FAT size: 1024 clusters.
/// - Reserved clusters: First 5 clusters.
/// - Supports connecting and disconnecting clusters.
/// - Handles superblock creation and disk initialization.
///
/// Methods:
/// - **InitializeFAT**: Initializes the FAT array for a new system.
/// - **CreateSuperBlock**: Creates a superblock with default data.
/// - **writeFat**: Writes the current FAT array to the virtual disk.
/// - **readFat**: Reads the FAT array from the virtual disk.
/// - **printFAT**: Prints the status of all clusters in the FAT array.
/// - **getAvailableCluster**: Returns the index of the first available (empty) cluster.
/// - **getAvailableClusters**: Returns the total number of available clusters.
/// - **getFreeSize**: Calculates the free disk space in bytes.
/// - **getClusterStatus**: Retrieves the status of a specific cluster.
/// - **setClusterPointer**: Links clusters together by updating their pointers.
/// - **CreateOrOpenFatArray**: Initializes the FAT if the disk is new or reads it if it exists.
/// - **CloseTheSystem**: Saves the FAT and closes the system gracefully.
///
/// Constants:
/// - **totalClusterSize**: The total number of clusters in the FAT (1024).
/// - **firstClusterEmptyInNew**: The first available cluster index in a new system (5).
///
/// Usage:
/// 1. Call `CreateOrOpenFatArray()` to initialize or load the FAT system.
/// 2. Use cluster management methods like `getAvailableCluster` or `setClusterPointer`
///    to manage cluster connections and data.
/// 3. Save changes by calling `Write_FAT()` and close the system with `CloseTheSystem()`.
///
/// Dependencies:
/// - Relies on `VirtualDisk` for reading and writing clusters.
/// - Utilizes `Converter` for data transformation between byte arrays and integers.
///
/// </summary>
