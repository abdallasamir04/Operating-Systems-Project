using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSV2
{
    internal class DirectoryEntry //Class Directory Entry
    {
        public const byte attFile = 0X0;//Attribute File
        public const byte attDirectory = 0X10;//Attribute Directory
        public char[] Name = new char[11];// Directory or file name (11 characters max)
        public byte Attr; // Attribute (0x0 for file, 0x10 for directory)
        public byte[] Empty = new byte[12];// Reserved space for future use
        public int firstCluster;// The first cluster number where the file/folder data is located
        public int Size;// The size of the file (only for files)
       
        public DirectoryEntry(string name, byte attr, int firstCluster, int Size = 0) //Constructor 
        {
            if (name == null)
                return;
            this.Attr = attr;
            this.firstCluster = firstCluster;
            this.Size = Size;

            if (attr == attFile)//If input File
            {
                string[] fileName = name.Split('.');
                HandleFileName(fileName[0].ToCharArray(), fileName[1].ToCharArray());
            }
            if (attr == attDirectory)//If input Folder
            {
                HandleDirName(name.ToCharArray());
            }
        }
        
        public static bool ISValidName(string Name, byte Attr)//check if name is valid char 
        {
            if (Attr == attFile)
            {
                if (Name.Split('.').Length != 2)
                    return false;
                Name = Name.Replace(".", "");
            }
            char[] invalidChars = {'!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=', '+', '[', ']', '{', '}',
                ';', ':', ',', '.', '<', '>', '/', '?', '\\', '|', '~', '`'};

            return !Name.Any(c => invalidChars.Contains(c));
        }
        
        private void HandleFileName(char[] fileName, char[] extension)//To handle File Name with Size name = 7 and Extension name = 3
        {
            int count = 0;
           
            for (int i = 0; i < Math.Min(7, fileName.Length); i++)
            {
                Name[count++] = fileName[i];
            }
            Name[count++] = '.';
            for (int i = 0; i < Math.Min(3, extension.Length); i++)
            {
                Name[count++] = extension[i];
            }
            while (count < 11)
            {
                Name[count++] = ' ';
            }
        }
        
        private void HandleDirName(char[] name)//To handle Folder Name with Size = 11
        {
            int count = 0;
            for (int i = 0; i < Math.Min(11, name.Length); i++)
            {
                Name[count++] = name[i];
            }
            while (count < 11)
            {
                Name[count++] = ' ';
            }
        }
    }
}
/// <summary>
/// Constructor to create a directory or file entry.
/// </summary>
/// <param name="name">The name of the file or folder.</param>
/// <param name="attr">The attribute (0x0 for file, 0x10 for folder).</param>
/// <param name="firstCluster">The first cluster where the file or folder is located.</param>
/// <param name="Size">The size of the file (optional, default is 0).</param
/// <summary>
/// Validates the name of a file or folder.
/// </summary>
/// <param name="Name">The name to be validated.</param>
/// <param name="Attr">The attribute (0x0 for file, 0x10 for folder).</param>
/// <returns>True if the name is valid, otherwise false.</returns>
/// <summary>
/// Handles the formatting of a file name, ensuring it fits the 8.3 format (7 characters for name, 3 for extension).
/// </summary>
/// <param name="fname">The file name.</param>
/// <param name="extension">The file extension.</param>
/// <summary>
/// Handles the formatting of a folder name, ensuring it fits the 11 character format.
/// </summary>
/// <param name="name">The folder name.</param>