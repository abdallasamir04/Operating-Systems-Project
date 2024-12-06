using System;
using System.IO;

public class Directory
{
    private string _path;

    public Directory(string path)
    {
        _path = path;
    }

    public string GetPath()
    {
        return _path;
    }

    // Check if the directory or file exists
    public bool Exists(string path)
    {
        return System.IO.Directory.Exists(path) || File.Exists(path);
    }

    // Create a directory
    public void CreateDirectory(string path)
    {
        if (!Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
        else
        {
            Console.WriteLine($"Error: Directory {path} already exists.");
        }
    }

    // Delete a file or directory
    public void Delete(string path)
    {
        if (Exists(path))
        {
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
                Console.WriteLine($"Directory {path} deleted successfully.");
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine($"File {path} deleted successfully.");
            }
        }
        else
        {
            Console.WriteLine($"Error: {path} does not exist.");
        }
    }

    // Get the file system entries (files and subdirectories)
    public string[] GetFileSystemEntries(string path)
    {
        if (Exists(path))
        {
            return System.IO.Directory.GetFileSystemEntries(path);
        }
        else
        {
            return new string[] { };
        }
    }

    // Read a file content
    public string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            Console.WriteLine($"Error: {path} not found.");
            return null;
        }
    }

    // Get the parent directory
    public Directory GetParent()
    {
        string parentPath = Path.GetDirectoryName(_path);
        return new Directory(parentPath);
    }

    // Check if the path is a file
    public bool IsFile(string path)
    {
        return File.Exists(path);
    }
}
