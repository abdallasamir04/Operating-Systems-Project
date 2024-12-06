using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class FileEntry
{
    public string FileName { get; set; }
    public byte DirAttr { get; set; }
    public byte[] DirEmpty { get; set; }
    public int DirFirstCluster { get; set; }
    public int DirFileSize { get; set; }

    // Constructor for a directory entry
    public FileEntry(string fileName, byte dirAttr, byte[] dirEmpty, int dirFirstCluster, int dirFileSize)
    {
        FileName = fileName;
        DirAttr = dirAttr;
        DirEmpty = dirEmpty;
        DirFirstCluster = dirFirstCluster;
        DirFileSize = dirFileSize;
    }

    // Convert the directory entry to a byte array
    public static byte[] DirectoryEntryToBytes(FileEntry entry)
    {
        return Converter.DirectoryEntryToBytes(new DirectoryEntry(entry.FileName, entry.DirAttr, entry.DirEmpty, entry.DirFirstCluster, entry.DirFileSize));
    }

    // Convert byte array back to directory entry
    public static FileEntry BytesToDirectoryEntry(byte[] bytes)
    {
        DirectoryEntry entry = Converter.BytesToDirectoryEntry(bytes);
        return new FileEntry(entry.DirName, entry.DirAttr, entry.DirEmpty, entry.DirFirstCluster, entry.DirFileSize);
    }

    // Write the directory entry to disk
    public static void WriteEntryToDisk(string diskPath, byte[] entryBytes)
    {
        using (FileStream fs = new FileStream(diskPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            fs.Seek(1024 * 5, SeekOrigin.Begin);
            fs.Write(entryBytes, 0, entryBytes.Length);
            fs.Flush();
        }
    }

    // Read the directory entry from disk
    public static FileEntry ReadEntryFromDisk(string diskPath, int entryIndex)
    {
        byte[] entryBytes = ReadEntryBytes(diskPath, entryIndex);
        return BytesToDirectoryEntry(entryBytes);
    }

    // Helper to read entry bytes from disk at a given index
    private static byte[] ReadEntryBytes(string diskPath, int entryIndex)
    {
        using (FileStream fs = new FileStream(diskPath, FileMode.Open, FileAccess.Read))
        {
            fs.Seek(1024 * 5 + (entryIndex * 32), SeekOrigin.Begin);
            byte[] entryBytes = new byte[32];
            fs.Read(entryBytes, 0, 32);
            return entryBytes;
        }
    }
}
