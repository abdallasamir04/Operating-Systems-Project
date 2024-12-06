using System;

public class Directory_Entry
{
    public char[] dir_name = new char[11]; // 8+3 filename format (11 characters)
    public byte dir_attr;                 // 0x0 for regular file, 0x10 for directory
    public byte[] dir_empty = new byte[12]; // Unused, but required in structure
    public int dir_firstCluster;           // Cluster number of first cluster in file
    public int dir_filesize;              // Size of file in bytes if it's a regular file

    // Constructor for creating a directory entry
    public Directory_Entry(string name, byte dir_attr, int dir_fCluster, string extension = "")
    {
        this.dir_attr = dir_attr;
        this.dir_firstCluster = dir_fCluster;
        this.dir_filesize = 0; // Initialize to 0 for directories, can be modified later

        if (dir_attr == 0x10)  // Directory
        {
            AssignDIRName(name);  // Assign the directory name
        }
        else  // Regular file
        {
            AssignFileName(name, extension);  // Assign the file name and extension
        }
    }

    // Method to clean the name to match the 8+3 file format and upper case
    public string CleanTheName(string s)
    {
        s = s.ToUpper();  // Convert the name to uppercase (case-insensitive)

        if (s.Length > 11)
            s = s.Substring(0, 11); // Truncate if the length exceeds 11 characters

        return s;
    }

    // Method to assign a file name to the dir_name in the 8+3 format
    public void AssignFileName(string name, string extension)
    {
        string fullFileName = name.Substring(0, Math.Min(name.Length, 7));  // Max 7 chars for file name part
        string ext = extension.Substring(0, Math.Min(extension.Length, 3)); // Max 3 chars for file extension part
        string formattedName = fullFileName + "." + ext;

        // Clean and assign the name to the dir_name (11 characters in total)
        formattedName = CleanTheName(formattedName);

        // Fill the dir_name with the formatted name
        for (int i = 0; i < 11; i++)
        {
            dir_name[i] = formattedName[i];
        }
    }

    // Method to assign a directory name to the dir_name (exactly 11 characters)
    public void AssignDIRName(string name)
    {
        // Ensure the name is 11 characters long, padding with null characters if needed
        name = name.ToUpper().PadRight(11, '\0');

        // Assign the directory name
        for (int i = 0; i < 11; i++)
        {
            dir_name[i] = name[i];
        }

        this.dir_attr = 0x10;  // Mark as a directory
    }

    // Utility function to print the directory entry for debugging or inspection
    public void PrintEntry()
    {
        Console.WriteLine("Name: {0}", new string(dir_name));
        Console.WriteLine("Attributes: 0x{0:X2}", dir_attr);
        Console.WriteLine("First Cluster: {0}", dir_firstCluster);
        Console.WriteLine("File Size: {0} bytes", dir_filesize);
    }

    // Method to get the size of the directory entry on disk (example: 32 bytes for directory entry)
    public int GetSizeOnDisk()
    {
        // For simplicity, assume a directory entry is 32 bytes in size
        // Modify this logic based on your actual file system structure
        return 32;
    }

    // Method to get the name of the directory entry (as a string)
    public string GetName()
    {
        return new string(dir_name).TrimEnd('\0'); // Remove null characters
    }

    // Method to clear the cluster (if needed)
    public void ClearCluster()
    {
        dir_firstCluster = 0;
    }
}
