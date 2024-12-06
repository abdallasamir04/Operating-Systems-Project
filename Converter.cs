using System;
using System.Collections.Generic;
using System.Text;

public static class Converter
{
    // Convert integer to array of 4 bytes
    public static byte[] IntToByte(int n)
    {
        return new byte[]
        {
            (byte)((n >> 24) & 0xFF),
            (byte)((n >> 16) & 0xFF),
            (byte)((n >> 8) & 0xFF),
            (byte)(n & 0xFF)
        };
    }

    // Convert array of 4 bytes to integer
    public static int ByteToInt(byte[] bytes)
    {
        int n = 0;
        for (int i = 0; i < bytes.Length; ++i)
        {
            n = (n << 8) | (bytes[i] & 0xFF);
        }
        return n;
    }

    // Convert integer array to byte array
    public static byte[] IntArrayToByteArray(int[] ints)
    {
        List<byte> bytes = new List<byte>();
        foreach (int i in ints)
        {
            byte[] b = IntToByte(i);
            bytes.AddRange(b);
        }
        return bytes.ToArray();
    }

    // Convert byte array to integer array
    public static int[] ByteArrayToIntArray(byte[] bytes)
    {
        int[] ints = new int[bytes.Length / 4];
        for (int i = 0, j = 0; i < bytes.Length; j++, i += 4)
        {
            byte[] b = new byte[4];
            Array.Copy(bytes, i, b, 0, 4);
            ints[j] = ByteToInt(b);
        }
        return ints;
    }

    // Split byte array into clusters (1024 bytes each)
    public static List<byte[]> SplitBytes(byte[] bytes)
    {
        List<byte[]> clusters = new List<byte[]>();
        int numberOfClusters = bytes.Length / 1024;
        int remainder = bytes.Length % 1024;

        for (int i = 0; i < numberOfClusters; i++)
        {
            byte[] cluster = new byte[1024];
            Array.Copy(bytes, i * 1024, cluster, 0, 1024);
            clusters.Add(cluster);
        }

        if (remainder > 0)
        {
            byte[] lastCluster = new byte[1024];
            Array.Copy(bytes, numberOfClusters * 1024, lastCluster, 0, remainder);
            clusters.Add(lastCluster);
        }

        return clusters;
    }

    // Convert DirectoryEntry to byte array
    public static byte[] DirectoryEntryToBytes(DirectoryEntry entry)
    {
        List<byte> bytes = new List<byte>();
        byte[] nameBytes = Encoding.ASCII.GetBytes(entry.DirName.PadRight(11, '\0'));
        bytes.AddRange(nameBytes);
        bytes.Add(entry.DirAttr);
        bytes.AddRange(entry.DirEmpty);
        bytes.AddRange(IntToByte(entry.DirFirstCluster));
        bytes.AddRange(IntToByte(entry.DirFileSize));
        return bytes.ToArray();
    }

    // Convert byte array to DirectoryEntry
    public static DirectoryEntry BytesToDirectoryEntry(byte[] bytes)
    {
        string name = Encoding.ASCII.GetString(bytes, 0, 11).TrimEnd('\0');
        byte attr = bytes[11];
        byte[] empty = new byte[12];
        Array.Copy(bytes, 12, empty, 0, 12);
        int firstCluster = ByteToInt(new byte[] { bytes[24], bytes[25], bytes[26], bytes[27] });
        int fileSize = ByteToInt(new byte[] { bytes[28], bytes[29], bytes[30], bytes[31] });

        return new DirectoryEntry(name, attr, empty, firstCluster, fileSize);
    }

    // Convert array of Directory Entries to byte array
    public static byte[] DirectoryEntriesToBytes(List<DirectoryEntry> entries)
    {
        List<byte> bytes = new List<byte>();
        foreach (var entry in entries)
        {
            bytes.AddRange(DirectoryEntryToBytes(entry));
        }
        return bytes.ToArray();
    }

    // Convert byte array to array of Directory Entries
    public static List<DirectoryEntry> BytesToDirectoryEntries(byte[] bytes)
    {
        List<DirectoryEntry> entries = new List<DirectoryEntry>();
        for (int i = 0; i < bytes.Length; i += 32)
        {
            byte[] entryBytes = new byte[32];
            Array.Copy(bytes, i, entryBytes, 0, 32);
            if (entryBytes[0] == 0) break;
            entries.Add(BytesToDirectoryEntry(entryBytes));
        }
        return entries;
    }

    // Convert string to byte array
    public static byte[] StringToByteArray(string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }

    // Convert byte array to string
    public static string ByteArrayToString(byte[] byteArray)
    {
        return Encoding.ASCII.GetString(byteArray);
    }
}

// DirectoryEntry class for storing file information
public class DirectoryEntry
{
    public string DirName { get; set; }
    public byte DirAttr { get; set; }
    public byte[] DirEmpty { get; set; }
    public int DirFirstCluster { get; set; }
    public int DirFileSize { get; set; }

    public DirectoryEntry(string dirName, byte dirAttr, byte[] dirEmpty, int dirFirstCluster, int dirFileSize)
    {
        DirName = dirName;
        DirAttr = dirAttr;
        DirEmpty = dirEmpty;
        DirFirstCluster = dirFirstCluster;
        DirFileSize = dirFileSize;
    }
}
