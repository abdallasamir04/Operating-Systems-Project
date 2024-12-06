using System;
using System.IO;

public static class VirtualDisk
{
    private const int ClusterSize = 1024; // 1 KB
    private const int TotalClusters = 1024; // Total number of clusters
    private static FileStream Disk;
    private static int[] FAT = new int[TotalClusters]; // File Allocation Table

    /// <summary>
    /// Creates or opens the virtual disk file.
    /// </summary>
    public static void CreateOrOpenDisk(string path)
    {
        Disk = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        if (Disk.Length == 0)
        {
            // Initialize a new disk (superblock, FAT, and clusters)
            InitializeDisk();
        }
        else
        {
            // Load FAT from disk
            LoadFAT();
        }
    }

    /// <summary>
    /// Initializes a new virtual disk by creating the superblock and FAT.
    /// </summary>
    private static void InitializeDisk()
    {
        // Initialize the superblock (cluster 0, filled with zeros)
        WriteCluster(new byte[ClusterSize], 0);

        // Initialize FAT with -1 (empty state)
        for (int i = 0; i < FAT.Length; i++)
        {
            FAT[i] = -1;
        }

        // Save FAT to clusters 1 to 4
        SaveFAT();

        // Initialize all other clusters with zeros
        for (int i = 5; i < TotalClusters; i++)
        {
            WriteCluster(new byte[ClusterSize], i);
        }
    }

    /// <summary>
    /// Saves the FAT array to disk in clusters 1 to 4.
    /// </summary>
    private static void SaveFAT()
    {
        byte[] fatBytes = new byte[FAT.Length * sizeof(int)];
        Buffer.BlockCopy(FAT, 0, fatBytes, 0, fatBytes.Length);

        // Write FAT across clusters 1 to 4
        for (int i = 0; i < 4; i++)
        {
            byte[] clusterData = new byte[ClusterSize];
            Array.Copy(fatBytes, i * ClusterSize, clusterData, 0, ClusterSize);
            WriteCluster(clusterData, i + 1);
        }
    }

    /// <summary>
    /// Loads the FAT array from disk (clusters 1 to 4).
    /// </summary>
    private static void LoadFAT()
    {
        byte[] fatBytes = new byte[FAT.Length * sizeof(int)];

        // Read FAT from clusters 1 to 4
        for (int i = 0; i < 4; i++)
        {
            byte[] clusterData = ReadCluster(i + 1);
            Array.Copy(clusterData, 0, fatBytes, i * ClusterSize, ClusterSize);
        }

        Buffer.BlockCopy(fatBytes, 0, FAT, 0, fatBytes.Length);
    }

    /// <summary>
    /// Writes a 1024-byte cluster to the virtual disk.
    /// </summary>
    public static void WriteCluster(byte[] cluster, int clusterIndex)
    {
        if (cluster.Length != ClusterSize)
            throw new ArgumentException("Cluster must be exactly 1024 bytes.");

        Disk.Seek(clusterIndex * ClusterSize, SeekOrigin.Begin);
        Disk.Write(cluster, 0, cluster.Length);
        Disk.Flush();
    }

    /// <summary>
    /// Reads a 1024-byte cluster from the virtual disk.
    /// </summary>
    public static byte[] ReadCluster(int clusterIndex)
    {
        byte[] cluster = new byte[ClusterSize];
        Disk.Seek(clusterIndex * ClusterSize, SeekOrigin.Begin);
        Disk.Read(cluster, 0, cluster.Length);
        return cluster;
    }

    /// <summary>
    /// Closes the virtual disk file.
    /// </summary>
    public static void CloseDisk()
    {
        // Save FAT before closing
        SaveFAT();
        Disk.Close();
    }

    /// <summary>
    /// Updates FAT entry for a specific cluster.
    /// </summary>
    public static void UpdateFAT(int clusterIndex, int value)
    {
        FAT[clusterIndex] = value;
        SaveFAT(); // Persist FAT changes
    }

    /// <summary>
    /// Gets FAT entry for a specific cluster.
    /// </summary>
    public static int GetFATEntry(int clusterIndex)
    {
        return FAT[clusterIndex];
    }
}


