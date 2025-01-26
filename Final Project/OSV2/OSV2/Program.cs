using System.Collections.Concurrent;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Xml.Linq;

namespace OSV2
{
  
    class Program
    {
        
        public const string rootDir = "A";//Name Root Directory

        public static int firstCluster = (MiniFat.getClusterStatus(MiniFat.firstClusterEmptyInNew) == MiniFat.emptyCluster) ? MiniFat.emptyCluster : MiniFat.firstClusterEmptyInNew;//Root First Cluster Status

        public static Directory root = new Directory(rootDir, 
            DirectoryEntry.attDirectory,
            firstCluster, null);
        
        public static Directory currentDirectory = root;//Create object Current Directory
        
        public static string Path = $"{rootDir}:\\";//Path current directory 

        public static void setPath()//Set new path current directory
        {
            string path = "";//create empty string to add new path
            Directory tempDir = currentDirectory;//create object directory and = with current directory

            while (tempDir.Parent != null)//while directory not root directory
            {
                path = $"{new string(tempDir.Name).Trim()}\\{path}";//get name directory
                tempDir = tempDir.Parent;//get back to parent directory
            }
            Path = $"{rootDir}:\\" + path.TrimEnd('\\');//add name root directory and save in path directory
        }

      
        public static string getPath()//return path current directory
        {
            if (Path[Path.Length - 1] == '\\')//remove last char if = \
                Path = Path.TrimEnd('\\');
            return Path;
        }

       
        public static void Main()
        {
            VirtualDisk.InitializeOrOpenFile("A.txt");
            MiniFat.CreateOrOpenFatArray();
            Console.WriteLine("\n====================================================\r\nWelcome to our Virtual Disk Shell!\r\nThis project simulates a command-line interface (CLI) for managing files and directories.\r\nYou can navigate directories, manage files, and perform various file operations.\r\n\r\nDeveloped by:\r\nAbdalla Mahmoud Samir \r\n====================================================\n\r\r\r under the super vision of Eng.Khaled Gamal\n====================================================");

            while (true)//does not close system 
            {
                setPath();
                Console.Write($"{getPath()}\\>>>>> ");
                try
                {
                    string inputCommand = Console.ReadLine(); // Read user input
                    Tokenizer.parseInput(inputCommand);
                    setPath(); 
                }
                catch (Exception e)
                {
                    // Handle errors silently and continue the loop
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }
    }
}