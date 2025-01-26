using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSV2
{
    internal class FileEntry : DirectoryEntry
    {
        public string Content;//The data in file
        Directory Parent;//The directory parent

        public FileEntry(string Name, byte Attr, int firstCluster, Directory Parent, string Content, int fileSize) : base(Name, Attr, firstCluster, fileSize)//create object file entry from directory entry and content file
        {
            this.Content = Content;//get Content
            if (Parent != null)//if not root directory
                this.Parent = Parent;//parent file = directory 
        }

        public FileEntry(DirectoryEntry d, Directory pa) : base(new string(d.Name), d.Attr, d.firstCluster)
        {
            Array.Copy(this.Empty, d.Empty, this.Empty.Length);
            Size = d.Size;
            Content = "";
            if (pa != null)
                this.Parent = pa;
        }

        public int getMySizeOnDisk() //Get Size my file 
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

        public void emptyMyClusters()//empty my file from mini fat table
        {
            int clusterIndex = this.firstCluster;

            int next = MiniFat.getClusterStatus(clusterIndex);

            do
            {
                MiniFat.setClusterPointer(clusterIndex, MiniFat.emptyCluster);
                clusterIndex = next;
                if (clusterIndex != MiniFat.fullCluster)
                    next = MiniFat.getClusterStatus(clusterIndex);

            } while (clusterIndex != MiniFat.fullCluster);
        }

        public DirectoryEntry getDirectoryEntry()//Get information file entry from directory entry
        {
            DirectoryEntry me = new DirectoryEntry(new string(this.Name), this.Attr, this.firstCluster, this.Size);
            Array.Copy(this.Empty, me.Empty, this.Empty.Length);
            me.Size = this.Size;//get my size file
            return me;
        }

        public void WriteFile()//Write Content file in clusters 
        {
            DirectoryEntry Old = getDirectoryEntry();//get information directory entry before
            if (!string.IsNullOrEmpty(Content))//if content is found
            {
                byte[] byteContent = Converter.ConvertCharToByte(Content);//convert content from string to byte
                List<byte[]> listOfArrayOfBytes = Converter.SplitBytes(byteContent);//split byte Content by size of cluster

                int clusterFATIndex;
                if (this.firstCluster != MiniFat.emptyCluster)//if first cluster have a content
                {
                    emptyMyClusters();//Empty All Data in Cluster index to rewrite 
                    clusterFATIndex = this.firstCluster;
                    this.firstCluster = clusterFATIndex;
                }
                else//First Cluster is empty cluster
                {
                    clusterFATIndex = MiniFat.getAvailableCluster();//Get available cluster with status empty cluster
                    if (clusterFATIndex != MiniFat.fullCluster)//if found cluster
                        this.firstCluster = clusterFATIndex;
                }
                int lastCluster = MiniFat.fullCluster;//last cluster is full cluster status
                for (int i = 0; i < listOfArrayOfBytes.Count; i++)
                {
                    if (clusterFATIndex != MiniFat.fullCluster)
                    {
                        VirtualDisk.WriteCluster(listOfArrayOfBytes[i], clusterFATIndex);//Write cluster
                        MiniFat.setClusterPointer(clusterFATIndex, MiniFat.fullCluster);//Set Status this cluster with full cluster
                        if (lastCluster != MiniFat.fullCluster)//if last cluster is not equal full cluster status
                            MiniFat.setClusterPointer(lastCluster, clusterFATIndex);//set status cluster
                        lastCluster = clusterFATIndex;//update last cluster
                        clusterFATIndex = MiniFat.getAvailableCluster();//get new empty cluster index
                    }
                }
            }
            else//if not input content
            {
                if (firstCluster != MiniFat.emptyCluster)//first cluster if not 0
                    emptyMyClusters();
                firstCluster = MiniFat.emptyCluster;//first cluster = 0
            }
            DirectoryEntry New = getDirectoryEntry();
            if (this.Parent != null)//if not root directory
            {
                this.Parent.updateContent(Old, New);//update content
            }
            MiniFat.writeFat();//rewrite fat array table
        }

        public void ReadFile()//Read content file from clusters
        {
            Content = "";//clear Content before read content
            if (this.firstCluster != MiniFat.emptyCluster)//if content is find
            {
                int cluster = this.firstCluster;
                int next = MiniFat.getClusterStatus(cluster);//get index mini fat array status
                List<byte> dataContent = new List<byte>();//create list byte array to save range cluster
                do
                {
                    dataContent.AddRange(VirtualDisk.ReadCluster(cluster));//save content byte cluster in data content array
                    cluster = next;
                    if (cluster != MiniFat.fullCluster)
                        next = MiniFat.getClusterStatus(cluster);
                }
                while (cluster != MiniFat.fullCluster);//if not end file
                Content += Converter.ConvertByteToString(dataContent.ToArray()).Trim('\0');//convert content from byte to string
            }
            /////////////////////////////////////////Console.WriteLine(Content+'\n');
        }

        public void deleteFile()//Delete File from Mini fat array and directory array
        {
            emptyMyClusters();
            if (this.Parent != null)//if not root directory
            {
                this.Parent.removeEntry(getDirectoryEntry());//delete index information directory entry
            }
        }

        public void printContent()//print Content File and Name File
        {
            Console.WriteLine($"\n{Content}\n");
        }
    }
}
/// <summary>
/// Summary of the FileEntry class, its variables, and methods:
/// 
/// The FileEntry class represents a file in a directory, extending the DirectoryEntry class.
/// It contains the file's content and a reference to its parent directory.
/// 
/// Variables:
/// - Content: A string that stores the data/content of the file.
/// - Parent: A Directory object that references the parent directory of the file.
/// 
/// Key Methods:
/// - getMySizeOnDisk(): Calculates the size of the file on disk by traversing the clusters in the MiniFat.
/// - emptyMyClusters(): Frees the clusters occupied by the file in the MiniFat.
/// - getDirectoryEntry(): Returns a DirectoryEntry object representing the file's metadata.
/// - WriteFile(): Writes the file's content to the disk, updating the MiniFat and parent directory.
/// - ReadFile(): Reads the file's content from the disk and stores it in the Content property.
/// - deleteFile(): Deletes the file by freeing its clusters and removing its entry from the parent directory.
/// - printContent(): Prints the file's name and content to the console.
/// 
/// The class is essential for managing file operations within the directory structure of the OSV2 system.
/// </summary>