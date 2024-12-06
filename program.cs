using System;
using System.IO;

namespace OS
{
    public class Program
    {
        private static string currentDirectory = System.IO.Directory.GetCurrentDirectory();
        private static DirectoryOperations currentDirObject = new DirectoryOperations(currentDirectory); // Initialize with a default directory object

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the basic file system simulator.");
            Console.WriteLine("Type 'help' for a list of commands.");

            while (true)
            {
                // Displaying the prompt
                Console.Write($"{currentDirectory} > ");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                // Process the command
                ProcessCommand(input);
            }
        }

        private static void ProcessCommand(string input)
        {
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                Console.WriteLine("Error: No command provided.");
                return;
            }

            string command = parts[0].ToLower();
            string argument = parts.Length > 1 ? string.Join(" ", parts, 1, parts.Length - 1) : null;

            switch (command)
            {
                case "help":
                    ExecuteHelp(argument);
                    break;
                case "cd":
                    ChangeDirectory(argument);
                    break;
                case "cls":
                    ClearScreen(argument);
                    break;
                case "dir":
                    ListDirectoryContents(argument);
                    break;
                case "quit":
                    QuitShell(argument);
                    break;
                case "copy":
                    CopyFiles(argument);
                    break;
                case "rename":
                    RenameFile(argument);
                    break;
                case "del":
                    DeleteFiles(argument);
                    break;
                case "md":
                    CreateDirectory(argument);
                    break;
                case "rd":
                    RemoveDirectory(argument);
                    break;
                case "type":
                    DisplayFileContent(argument);
                    break;
                case "import":
                    ImportFiles(argument);
                    break;
                case "export":
                    ExportFiles(argument);
                    break;
                default:
                    Console.WriteLine("Error: Command not recognized. Type 'help' for a list of available commands.");
                    break;
            }
        }

        private static void ExecuteHelp(string command = null)
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("help - Provides help information for commands.");
            Console.WriteLine("cd [directory] - Changes the current directory.");
            Console.WriteLine("dir - Lists the content of the current directory.");
            Console.WriteLine("md [directory] - Creates a new directory.");
            Console.WriteLine("del [file|directory] - Deletes a file or directory.");
            Console.WriteLine("cls - Clears the console screen.");
            Console.WriteLine("quit - Exits the shell.");
        }

        private static void ChangeDirectory(string path)
        {
            if (string.IsNullOrEmpty(path) || path == ".")
            {
                Console.WriteLine($"Current directory: {currentDirectory}");
            }
            else if (path == "..")
            {
                currentDirObject = currentDirObject.GetParent(); // Get parent directory
                currentDirectory = currentDirObject.GetPath();
                Console.WriteLine($"Changed to: {currentDirectory}");
            }
            else
            {
                string newPath = Path.IsPathRooted(path) ? path : Path.Combine(currentDirectory, path);

                if (currentDirObject.Exists(newPath))
                {
                    currentDirectory = newPath;
                    currentDirObject = new DirectoryOperations(newPath); // Update the Directory object
                    Console.WriteLine($"Changed to: {currentDirectory}");
                }
                else
                {
                    Console.WriteLine("Error: The system cannot find the specified folder.");
                }
            }
        }

        private static void ClearScreen(string argument)
        {
            if (!string.IsNullOrEmpty(argument))
            {
                Console.WriteLine("Error: The syntax of the command is incorrect.");
            }
            else
            {
                Console.Clear();
            }
        }

        private static void ListDirectoryContents(string path)
        {
            string targetPath = string.IsNullOrEmpty(path) || path == "." ? currentDirectory :
                                 path == ".." ? currentDirObject.GetParent().GetPath() :
                                 Path.IsPathRooted(path) ? path : Path.Combine(currentDirectory, path);

            if (string.IsNullOrEmpty(targetPath) || !currentDirObject.Exists(targetPath))
            {
                Console.WriteLine("Error: The path is not found.");
                return;
            }

            var entries = currentDirObject.GetFileSystemEntries(targetPath); // Using custom method
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }

        private static void QuitShell(string argument)
        {
            if (!string.IsNullOrEmpty(argument))
            {
                Console.WriteLine("Error: The syntax of the command is incorrect.");
            }
            else
            {
                Console.WriteLine("Exiting shell...");
                Environment.Exit(0);
            }
        }

        private static void CopyFiles(string argument)
        {
            Console.WriteLine("Copy functionality not implemented yet.");
        }

        private static void RenameFile(string argument)
        {
            Console.WriteLine("Rename functionality not implemented yet.");
        }

        private static void DeleteFiles(string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                Console.WriteLine("Error: The syntax of the del command is incorrect.");
                return;
            }

            string targetPath = Path.Combine(currentDirectory, argument);
            currentDirObject.Delete(targetPath); // Using custom delete method
        }

        private static void CreateDirectory(string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                Console.WriteLine("Error: The syntax of the md command is incorrect.");
                return;
            }

            string newDirectory = Path.Combine(currentDirectory, argument);
            currentDirObject.CreateDirectory(newDirectory); // Using custom create directory method
        }

        private static void RemoveDirectory(string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                Console.WriteLine("Error: The syntax of the rd command is incorrect.");
                return;
            }

            string targetDir = Path.Combine(currentDirectory, argument);
            currentDirObject.Delete(targetDir); // Using custom delete method for directories
        }

        private static void DisplayFileContent(string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                Console.WriteLine("Error: The syntax of the type command is incorrect.");
                return;
            }

            string targetFile = Path.Combine(currentDirectory, argument);
            string content = currentDirObject.ReadFile(targetFile); // Custom method to read a file
            if (!string.IsNullOrEmpty(content))
            {
                Console.WriteLine(content);
            }
        }

        private static void ImportFiles(string argument)
        {
            Console.WriteLine("Import functionality not implemented yet.");
        }

        private static void ExportFiles(string argument)
        {
            Console.WriteLine("Export functionality not implemented yet.");
        }
    }

    public class DirectoryOperations
    {
        private string _path;

        public DirectoryOperations(string path)
        {
            _path = path;
        }

        public string GetPath()
        {
            return _path;
        }

        public bool Exists(string path)
        {
            return System.IO.Directory.Exists(path) || File.Exists(path);  // Fully qualify System.IO.Directory
        }

        public void CreateDirectory(string path)
        {
            if (!Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);  // Fully qualify System.IO.Directory
            }
            else
            {
                Console.WriteLine($"Error: Directory {path} already exists.");
            }
        }

        public void Delete(string path)
        {
            if (Exists(path))
            {
                if (System.IO.Directory.Exists(path))  // Fully qualify System.IO.Directory
                {
                    System.IO.Directory.Delete(path, true);  // Fully qualify System.IO.Directory
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

        public string[] GetFileSystemEntries(string path)
        {
            if (System.IO.Directory.Exists(path))  // Fully qualify System.IO.Directory
            {
                return System.IO.Directory.GetFileSystemEntries(path);  // Fully qualify System.IO.Directory
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        public DirectoryOperations GetParent()
        {
            string parentPath = Path.GetDirectoryName(_path);
            return new DirectoryOperations(parentPath);
        }

        public string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return null;
        }
    }
}
