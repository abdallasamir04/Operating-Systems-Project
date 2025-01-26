using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OSV2
{
    internal class Directory : DirectoryEntry
    {
        public Directory Parent; //Parent Directory To This Directory

        public List<DirectoryEntry> Entries; //The Content On this Directory

        public const int notFoundIndex = -1;//use in search

        public Directory(string Name, byte Attr, int firstCluster, Directory Parent) : base(Name, Attr, firstCluster)//Get Content Form base and Connect this Folder by Parent Folder
        {
            Entries = new List<DirectoryEntry>();
            this.Parent = Parent;
        }

        public DirectoryEntry getDirectoryEntry()//Get information  about directory from directory entry
        {
            DirectoryEntry Me = new DirectoryEntry(new string(this.Name), this.Attr, this.firstCluster, 0);
            Array.Copy(this.Empty, Me.Empty, this.Empty.Length);
            return Me;
        }

        public int getMySizeOnDisk() //Get Size My directory
        {
            int size = 0;
            int clusterIndex = this.firstCluster;
            do
            {
                size++;
                clusterIndex = MiniFat.getClusterStatus(clusterIndex);
            } while (clusterIndex != MiniFat.fullCluster);
            return size;
        }

        public bool canAddEntry(DirectoryEntry entry) //Show if can Add New directory or not 
        {
            int neededClusters = (int)Math.Ceiling((double)((Entries.Count + 1) * 32) / MiniFat.clusterSize);//add size new directory entry and get size byte it is needed
            neededClusters += (int)Math.Ceiling((double)entry.Size / MiniFat.clusterSize);//get size directory entry bytes it is needed
            return (getMySizeOnDisk() + MiniFat.getAvailableClusters()) >= neededClusters;
        }

        public void emptyMyCluster()//to Empty Clusters in Fat Array
        {
            if (this.firstCluster != MiniFat.emptyCluster)//if false this mean it is empty
            {
                int Cluster = this.firstCluster;
                int next = MiniFat.getClusterStatus(Cluster);
                if (Cluster == MiniFat.firstClusterEmptyInNew && next == MiniFat.emptyCluster)//this mean i am in root directory and not data 
                    return;

                do
                {
                    MiniFat.setClusterPointer(Cluster, MiniFat.emptyCluster);
                    Cluster = next;
                    if (Cluster != MiniFat.fullCluster)
                    {
                        next = MiniFat.getClusterStatus(Cluster);
                    }
                }
                while (Cluster != MiniFat.fullCluster);
            }
        }

        public void WriteDirectory() //Write DirOrFile Array in Clusters
        {
            byte[] dirsContent = new byte[Entries.Count * 32];//create array with size number directory entry in parent array multiple directory entry information size
            for (int i = 0; i < Entries.Count; i++)//Convert Array Dir To Byte And and put byte in dirsContent Array
            {
                byte[] dirContent = Converter.DirToByte(this.Entries[i]);
                for (int j = i * 32, k = 0; k < dirContent.Length; k++, j++)
                    dirsContent[j] = dirContent[k];
            }
            List<byte[]> dirsBytes = Converter.SplitBytes(dirsContent);//Split bytes 1024 in dirsBytes Array
            int clusterFATIndex = (this.firstCluster != MiniFat.emptyCluster) ? this.firstCluster : MiniFat.getAvailableCluster();//Get Cluster Index
            this.firstCluster = clusterFATIndex;
            int lastCluster = MiniFat.fullCluster;
            for (int i = 0; i < dirsBytes.Count; i++)//Write dirsBytes Array in Clusters and Connected with them
            {
                if (clusterFATIndex != MiniFat.fullCluster)
                {
                    VirtualDisk.WriteCluster(dirsBytes[i], clusterFATIndex);
                    MiniFat.setClusterPointer(clusterFATIndex, MiniFat.fullCluster);
                    if (lastCluster != MiniFat.fullCluster)
                        MiniFat.setClusterPointer(lastCluster, clusterFATIndex);
                    lastCluster = clusterFATIndex;
                    clusterFATIndex = MiniFat.getAvailableCluster();//get new index number with 0 status
                }
            }
            if (Entries.Count == 0)//if not directory entry in array
            {
                if (firstCluster != MiniFat.emptyCluster)//if first cluster not equal 0
                    MiniFat.setClusterPointer(firstCluster, MiniFat.emptyCluster);//first cluster status in fat array table equal 0
                firstCluster = MiniFat.emptyCluster;//first cluster in information directory entry equal 0
            }
            if (this.Parent != null)//if directory does not root directory
            {
                DirectoryEntry currentEntry = this.getDirectoryEntry();//get information my directory
                int index = this.Parent.searchDirectory(new string(currentEntry.Name).Trim());//get index directory parent array 
                this.Parent.updateContent(this.Parent.Entries[index], currentEntry);
            }
            MiniFat.writeFat();//rewrite fat array
        }

        public void ReadDirectory() //Read Content Directory From Disk 
        {
            Entries.Clear();//clear array to write new data
            if (this.firstCluster != MiniFat.emptyCluster)
            {
                int cluster = this.firstCluster;
                int next;
                List<byte> dataCluster = new List<byte>();//create list byte array to add range cluster size
                do
                {
                    dataCluster.AddRange(VirtualDisk.ReadCluster(cluster));
                    next = MiniFat.getClusterStatus(cluster);//get status index fat array
                    cluster = next;
                }
                while (next != MiniFat.fullCluster);
                Entries = Converter.DirByteToDirEntry(dataCluster);
            }
        }
       
        public void addEntry(DirectoryEntry NewDirEntry)//Add New Directory
        {
            Entries.Add(NewDirEntry);//add directory entry content in array directory
            WriteDirectory();
        }
   
        public void removeEntry(DirectoryEntry DelDirEntry)//delete directory entry from array parent directory
        {
            ReadDirectory();
            int index = searchDirectory(new string(DelDirEntry.Name).Trim());
            if (index != notFoundIndex)
            {
                Entries.RemoveAt(index);
                WriteDirectory();
            }
        }

        public void deleteDirectory()//Delete directory from Mini Fat table and directory entry from array directory parent
        {
            emptyMyCluster();
            if (this.Parent != null)
            {
                int index = this.Parent.searchDirectory(new string(this.Name));
                if (index != notFoundIndex)
                {
                    this.Parent.ReadDirectory();
                    this.Parent.Entries.RemoveAt(index);
                    this.Parent.WriteDirectory();
                }
            }
        }

        public void updateContent(DirectoryEntry OLD, DirectoryEntry New) //Update Content Directory
        {
            ReadDirectory();//Read directory content
            int index = searchDirectory(new string(OLD.Name).Trim());//search by name old directory entry to get index number of array
            if (index != notFoundIndex)//if found
            {
                Entries[index] = New;//index old directory entry replace new directory entry
                WriteDirectory();
            }
        }

        public int searchDirectory(string name)//Search Directory in this Folder
        {
            ReadDirectory();//Read content directory
            name = name.PadRight(11, ' ').Substring(0, 11);//Get (name) wth size 11
            for (int i = 0; i < Entries.Count; i++)//Loop from first directory entry to last in directory parent
            {
                if (new string(Entries[i].Name).Equals(name))//if name input is equal same name directory entry
                {
                    return i;
                }
            }
            return notFoundIndex;//else end loop and not found
        }
    }
}
/// <summary>
/// This program simulates a directory-based file system using a FAT (File Allocation Table) structure.
/// It defines a `Directory` class that extends `DirectoryEntry` and provides functionalities to manage
/// directories and their contents (files or subdirectories).
/// 
/// Key Components:
/// 
/// 1. **Directory Class**:
///    - Represents a directory in the file system.
///    - Stores information about its parent directory and its contents (files or subdirectories).
///    - Provides methods to manage directory entries:
///        - Add, remove, update, and search entries.
///        - Read and write directory data to disk.
///        - Calculate the size of the directory and manage associated FAT clusters.
/// 
/// 2. **Core Methods**:
///    - `addEntry()`: Adds a new directory or file entry.
///    - `removeEntry()`: Removes a directory or file entry.
///    - `updateContent()`: Updates an existing directory entry.
///    - `searchDirectory()`: Searches for an entry by name.
///    - `emptyMyCluster()`: Clears all clusters associated with the directory in the FAT.
///    - `WriteDirectory()`: Writes the directory content to the disk.
///    - `ReadDirectory()`: Reads the directory content from the disk.
/// 
/// 3. **Directory and File System Integration**:
///    - Uses a `Mini_FAT` table to manage clusters and their relationships.
///    - Handles conversion between directory entries and byte data for storage purposes.
/// 
/// 4. **Helper Features**:
///    - Supports checking if new entries can fit in the available disk space.
///    - Ensures efficient data storage and retrieval by splitting and managing byte arrays across clusters.
/// 
/// 5. **Program Objectives**:
///    - Emulates file system behavior in a controlled environment.
///    - Facilitates directory and file management, including CRUD operations (Create, Read, Update, Delete).
///    - Demonstrates the use of clusters in a FAT-like file system.
/// 
/// Usage:
/// - Create instances of `Directory` to simulate directories.
/// - Use the provided methods to add, remove, update, and search for entries.
/// - Interact with a virtual disk and FAT table for data storage and retrieval.
/// 
/// Notes:
/// - The program is a simplified simulation and may not cover all edge cases in a real-world file system.
/// - Integration with `Mini_FAT`, `VirtualDisk`, and `Converter` components is required for full functionality.
/// </summary>