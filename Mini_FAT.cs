using System;
using System.Collections.Generic;
using System.IO;

public class MiniFAT
{
    private int[] FAT = new int[1024];
    private string diskFileName;

    public MiniFAT(string fileName)
    {
        diskFileName = fileName;
    }

    // Initialize the FAT array
    public void InitializeFAT()
    {
        for (int i = 0; i < FAT.Length; i++)
        {
            if (i == 0 || i == 4)
                FAT[i] = -1;
            else if (i > 0 && i <= 3)
                FAT[i] = i + 1;
            else
                FAT[i] = 0;
        }
    }

    // Print the FAT array (Debugging)
    public void PrintFAT()
    {
        Console.WriteLine("FAT Contents:");
        for (int i = 0; i < FAT.Length; i++)
        {
            Console.WriteLine($"FAT[{i}] = {FAT[i]}");
        }
    }

    // Get the index of the first available cluster
    public int GetAvailableCluster()
    {
        for (int i = 0; i < FAT.Length; i++)
        {
            if (FAT[i] == 0) return i;
        }
        return -1; // Disk is full
    }

    // Count available clusters
    public int GetAvailableClusters()
    {
        int count = 0;
        for (int i = 0; i < FAT.Length; i++)
        {
            if (FAT[i] == 0) count++;
        }
        return count;
    }

    // Get free size in bytes
    public int GetFreeSize()
    {
        return GetAvailableClusters() * 1024; // Cluster size is 1024 bytes
    }

    // Get cluster status
    public int GetClusterStatus(int clusterIndex)
    {
        if (clusterIndex >= 0 && clusterIndex < FAT.Length)
        {
            return FAT[clusterIndex];
        }
        return -1; // Invalid index
    }

    // Set cluster pointer/status
    public void SetClusterPointer(int clusterIndex, int status)
    {
        if (clusterIndex >= 0 && clusterIndex < FAT.Length)
        {
            FAT[clusterIndex] = status;
        }
    }

    // Convert FAT to byte array and write it to disk (clusters 1 to 4)
    public void WriteFAT()
    {
        byte[] fatBytes = new byte[FAT.Length * sizeof(int)];
        Buffer.BlockCopy(FAT, 0, fatBytes, 0, fatBytes.Length);

        using (FileStream fs = new FileStream(diskFileName, FileMode.OpenOrCreate))
        {
            fs.Seek(1024, SeekOrigin.Begin); // Skip cluster 0 (superblock)
            fs.Write(fatBytes, 0, fatBytes.Length);
        }
    }

    // Read FAT array from disk
    public void ReadFAT()
    {
        byte[] fatBytes = new byte[FAT.Length * sizeof(int)];

        using (FileStream fs = new FileStream(diskFileName, FileMode.Open))
        {
            fs.Seek(1024, SeekOrigin.Begin); // Skip cluster 0 (superblock)
            fs.Read(fatBytes, 0, fatBytes.Length);
        }

        Buffer.BlockCopy(fatBytes, 0, FAT, 0, fatBytes.Length);
    }

    // Create superblock (byte array of 1024 items with value 0)
    public byte[] CreateSuperBlock()
    {
        return new byte[1024];
    }

    // Initialize or open the file system
    public void InitializeOrOpenFileSystem()
    {
        bool isNew = !File.Exists(diskFileName);

        if (isNew)
        {
            using (FileStream fs = new FileStream(diskFileName, FileMode.Create))
            {
                // Write superblock
                fs.Write(CreateSuperBlock(), 0, 1024);

                // Initialize FAT and write it
                InitializeFAT();
                WriteFAT();
            }
        }
        else
        {
            // Read FAT from existing disk
            ReadFAT();
        }
    }

    // Close the file system (write the latest FAT)
    public void CloseFileSystem()
    {
        WriteFAT();
    }
}

// Example usage
