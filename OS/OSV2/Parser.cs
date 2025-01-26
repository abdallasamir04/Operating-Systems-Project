using System;
using System.IO;

namespace OSV2
{
    internal class Parser
    {
        public static int cd(string Name, Directory currentDirectory)
        {
            if (string.IsNullOrEmpty(Name))
            {
                Console.WriteLine(Program.getPath());
                return 0;
            }

            if (Name[Name.Length - 1] == '\\')
                Name = Name.TrimEnd('\\');

            string[] Value = Name.Split('\\');

            for (int Count = 0; Count < Value.Count(); Count++)
            {
                if (Value[Count] == ".")
                {
                    if (Value.Length == 1)
                        return 0;
                    else
                    {
                        continue;
                    }
                }
                else if (Value[Count] == "..")
                {
                    if (currentDirectory.Parent != null)
                    {
                        currentDirectory = currentDirectory.Parent;
                        currentDirectory.ReadDirectory();
                    }
                    else
                    {
                        Console.WriteLine("You are already in the root directory.");
                        Program.currentDirectory = currentDirectory;
                        Program.setPath();
                        return 0;
                    }
                }
                else if (Value[Count] == $"{Program.rootDir}:")
                    currentDirectory = Program.root;
                else
                {
                    int index = currentDirectory.searchDirectory(Value[Count]);

                    if (index != Directory.notFoundIndex && currentDirectory.Entries[index].Attr == DirectoryEntry.attDirectory)
                    {
                        currentDirectory = new Directory(new string(currentDirectory.Entries[index].Name),
                            currentDirectory.Entries[index].Attr,
                            currentDirectory.Entries[index].firstCluster,
                            currentDirectory);
                        currentDirectory.ReadDirectory();
                    }
                    else
                    {
                        if (index == Directory.notFoundIndex)
                        {
                            Console.WriteLine($"Error : this path \'{Name}\' does not exist on your disk!");
                            return -1;
                        }
                        else if (currentDirectory.Entries[index].Attr != DirectoryEntry.attDirectory)
                        {
                            Console.WriteLine($"\'{Value[Count]}\' is not a directory.");
                            return -1;
                        }
                    }
                }
            }
            Program.currentDirectory = currentDirectory;
            Program.setPath();
            return 0;
        }
        public static void dir(bool showAnotherDirectory)
        {
            int fileCounter = 0, directoryCounter = 0, fileSizeSum = 0;
            Program.setPath();
            Console.WriteLine($"Directory of {Program.getPath()}\n");

            if (showAnotherDirectory)
            {
                Console.WriteLine("<DIR> .");
                Console.WriteLine("<DIR> ..");
                directoryCounter += 2;
            }

            cd(Program.getPath(), Program.root);
            Program.currentDirectory.ReadDirectory();

            foreach (var entry in Program.currentDirectory.Entries)
            {
                if (entry.Attr == DirectoryEntry.attFile)
                {
                    Console.WriteLine($"{entry.Size} {new string(entry.Name)}");
                    fileCounter++;
                    fileSizeSum += entry.Size;
                }
                else if (entry.Attr == DirectoryEntry.attDirectory)
                {
                    Console.WriteLine($"<DIR> {new string(entry.Name)}");
                    directoryCounter++;
                }
            }

            Console.WriteLine($"\n{fileCounter} File(s) {fileSizeSum} bytes");
            Console.WriteLine($"{directoryCounter} Dir(s) {MiniFat.getFreeSize()} bytes free");

            // طباعة الشجرة
            tree();
        }

        public static void dir(string name)
        {
            Directory originalDirectory = Program.currentDirectory;
            int fileCounter = 0, directoryCounter = 0, fileSizeSum = 0;

            if (name[name.Length - 1] == '\\')
                name = name.TrimEnd('\\');

            string[] path = name.Split('\\');

            if (name == $"{Program.rootDir}:" || name == $"{Program.rootDir}:\\")
            {
                Program.currentDirectory = Program.root;
                dir(true);
                Program.currentDirectory = originalDirectory;
                return;
            }

            if (path.Length > 1)
            {
                name = string.Join("\\", path, 0, path.Length - 1);
                if (cd(name, Program.currentDirectory) == -1)
                    return;
                name = path[path.Length - 1];
            }

            if (name == "..")
            {
                cd("..", Program.currentDirectory);
                dir(false);
                Program.currentDirectory = originalDirectory;
                return;
            }
            else if (name == ".")
            {
                dir(true);
                Program.currentDirectory = originalDirectory;
                return;
            }

            int index = Program.currentDirectory.searchDirectory(name);
            if (index != Directory.notFoundIndex)
            {
                if (Program.currentDirectory.Entries[index].Attr == DirectoryEntry.attDirectory)
                {
                    cd(name, Program.currentDirectory);
                    dir(true);
                    Program.currentDirectory = originalDirectory;
                    return;
                }
                else
                {
                    Program.setPath();
                    Console.WriteLine($"Directory of {Program.getPath()}\n");
                    tree();
                }
            }
            else
            {
                name = string.Join("\\", path, 0, path.Length);
                Console.WriteLine($"Error: The directory or file \'{name}\' does not exist!");
                Program.currentDirectory = originalDirectory;
                return;
            }

            Console.WriteLine($"\n{fileCounter} File(s) {fileSizeSum} bytes");
            Console.WriteLine($"{directoryCounter} Dir(s) {MiniFat.getFreeSize()} bytes free");
            Program.currentDirectory = originalDirectory;
        }

        public static void tree(Directory directory, string indent = "", bool isLast = true)
        {
            Console.Write(indent);
            if (isLast)
            {
                Console.Write("└── ");
                indent += "    ";
            }
            else
            {
                Console.Write("├── ");
                indent += "│   ";
            }
            Console.WriteLine($"{new string(directory.Name)} [{directory.firstCluster}]");

            directory.ReadDirectory();
            int entryCount = directory.Entries.Count;
            for (int i = 0; i < entryCount; i++)
            {
                var entry = directory.Entries[i];
                if (entry.Attr == DirectoryEntry.attDirectory)
                {
                    Directory subDir = new Directory(new string(entry.Name), DirectoryEntry.attDirectory, entry.firstCluster, directory);
                    tree(subDir, indent, i == entryCount - 1);
                }
                else if (entry.Attr == DirectoryEntry.attFile)
                {
                    Console.Write(indent);
                    if (i == entryCount - 1)
                    {
                        Console.Write("└── ");
                    }
                    else
                    {
                        Console.Write("├── ");
                    }
                    Console.WriteLine($"{new string(entry.Name)} [{entry.firstCluster}]");
                }
            }
        }

        public static void tree()
        {
            Console.WriteLine($"{new string(Program.currentDirectory.Name)} [{Program.currentDirectory.firstCluster}]");
            Program.currentDirectory.ReadDirectory();
            int entryCount = Program.currentDirectory.Entries.Count;
            for (int i = 0; i < entryCount; i++)
            {
                var entry = Program.currentDirectory.Entries[i];
                if (entry.Attr == DirectoryEntry.attDirectory)
                {
                    Directory subDir = new Directory(new string(entry.Name), DirectoryEntry.attDirectory, entry.firstCluster, Program.currentDirectory);
                    tree(subDir, "", i == entryCount - 1);
                }
                else if (entry.Attr == DirectoryEntry.attFile)
                {
                    if (i == entryCount - 1)
                    {
                        Console.WriteLine("└── " + $"{new string(entry.Name)} [{entry.firstCluster}]");
                    }
                    else
                    {
                        Console.WriteLine("├── " + $"{new string(entry.Name)} [{entry.firstCluster}]");
                    }
                }
            }
        }
        public static void type(string fileName)
        {
            Directory originalDirectory = Program.currentDirectory;
            string[] name2 = fileName.Split('\\');

            if (name2.Length > 1)
            {
                fileName = string.Join("\\", name2, 0, name2.Length - 1);
                if (cd(fileName, Program.currentDirectory) == -1)
                {
                    return;
                }
                fileName = name2[(name2.Length) - 1];
            }

            int index = Program.currentDirectory.searchDirectory(fileName);

            if (index != -1 && Program.currentDirectory.Entries[index].Attr == DirectoryEntry.attDirectory)
                Console.WriteLine($"This file : may be this \'{fileName}\' is not file name or ACCESS DENIE!");
            else
            {
                if (index != Directory.notFoundIndex)
                {
                    int firstCluster = Program.currentDirectory.Entries[index].firstCluster;
                    int fileSize = Program.currentDirectory.Entries[index].Size;

                    FileEntry file = new FileEntry(fileName, DirectoryEntry.attFile, firstCluster, Program.currentDirectory, "", fileSize);
                    file.ReadFile();
                    Program.setPath();

                    Console.WriteLine($"\nFile Name : \'{Program.getPath()}\\{fileName}\'\n");
                    file.printContent();
                }
                else
                {
                    Console.WriteLine($"This file : \'{fileName}\' does not exist on your disk!");
                }
            }
            Program.currentDirectory = originalDirectory;
        }

        public static void md(string directoryPathOrName, bool showMessage)
        {
            Directory originalDirectory = Program.currentDirectory;

            if (directoryPathOrName[directoryPathOrName.Length - 1] == '\\')
                directoryPathOrName = directoryPathOrName.TrimEnd('\\');
            string[] fullPathName = directoryPathOrName.Split('\\');
            string directoryName;
            if (fullPathName.Length > 1)
            {
                directoryName = fullPathName[fullPathName.Length - 1];
                directoryPathOrName = string.Join("\\", fullPathName, 0, fullPathName.Length - 1);
            }
            else
            {
                directoryName = fullPathName[0];
                directoryPathOrName = Program.getPath();
                if (directoryPathOrName[directoryPathOrName.Length - 1] == '\\')
                    directoryPathOrName = directoryPathOrName.TrimEnd('\\');
            }

            int availableCluster = MiniFat.getAvailableCluster();
            if (availableCluster != MiniFat.fullCluster)
            {
                if (cd(directoryPathOrName, Program.currentDirectory) == -1)
                    return;
                if (!DirectoryEntry.ISValidName(directoryName, DirectoryEntry.attDirectory))
                {
                    Console.WriteLine($"Error : this directory name \'{directoryName}\' is Invalid name");
                    Program.currentDirectory = originalDirectory;
                    return;
                }
                Directory newDir = new Directory(directoryName, DirectoryEntry.attDirectory, availableCluster, Program.currentDirectory);
                if (Program.currentDirectory.searchDirectory(directoryName) == Directory.notFoundIndex)
                {
                    Program.currentDirectory.addEntry(newDir.getDirectoryEntry());
                    newDir.WriteDirectory();
                    if (showMessage)
                        Console.WriteLine($"Directory '{directoryPathOrName}\\{directoryName}' created successfully.");
                }
                else
                {
                    Console.WriteLine($"Error : this directory \'{directoryName}\' is already exists!");
                }

            }
            else
            {
                Console.WriteLine("No space available to create a new directory.");
            }
            Program.currentDirectory = originalDirectory;
        }

        public static void md_m(string path)
        {
            Directory originalDirectory = Program.currentDirectory;
            string[] fullPathName = path.Split('\\');
            path = Program.getPath();
            foreach (string name in fullPathName)
            {
                if (name != $"{Program.rootDir}:" && !string.IsNullOrEmpty(name))
                {
                    int indexName = Program.currentDirectory.searchDirectory(name);
                    path = $"{path}\\{name}";

                    if (indexName == Directory.notFoundIndex)
                        Parser.md($"{path}", false);
                    else
                        cd(name, Program.currentDirectory);
                }
                else if (name != $"{Program.rootDir}:")
                {
                    Program.currentDirectory = Program.root;
                }
            }
            Console.WriteLine($"Directories '{path}' created successfully.");
            Program.currentDirectory = originalDirectory;
            return;
        }

        public static void rd(string directoryNameOrPath, bool askConfirmation)
        {
            Directory originalDirectory = Program.currentDirectory;

            if (directoryNameOrPath[directoryNameOrPath.Length - 1] == '\\')
                directoryNameOrPath = directoryNameOrPath.TrimEnd('\\');

            string[] fullPathDir = directoryNameOrPath.Split('\\');
            string directoryName = "";
            if (fullPathDir.Length > 1)
            {
                directoryName = string.Join("\\", fullPathDir, 0, fullPathDir.Length - 1);
                directoryNameOrPath = fullPathDir[fullPathDir.Length - 1];
                if (cd(directoryName, Program.currentDirectory) == -1)
                    return;
            }
            else
                directoryName = Program.getPath();

            int deleteIndex = Program.currentDirectory.searchDirectory(directoryNameOrPath);
            if (deleteIndex != Directory.notFoundIndex && Program.currentDirectory.Entries[deleteIndex].Attr == DirectoryEntry.attDirectory)
            {
                Directory deleteDir = new Directory(new string(Program.currentDirectory.Entries[deleteIndex].Name).Trim(),
                    DirectoryEntry.attDirectory,
                    Program.currentDirectory.Entries[deleteIndex].firstCluster,
                    Program.currentDirectory);
                if (deleteDir.firstCluster == MiniFat.emptyCluster)
                {
                    string User = "y";
                    if (askConfirmation)
                    {
                        do
                        {
                            Console.Write($"Are you sure that you want to delete \'{directoryName}\\{directoryNameOrPath}\',Please enter Y for Yes or N for No >> ");
                            User = Console.ReadLine().ToLower();
                        }
                        while (User != "y" && User != "n");
                    }
                    if (User == "y")
                    {
                        deleteDir.deleteDirectory();
                        Console.WriteLine($"Directory \'{directoryName}\\{directoryNameOrPath}\' deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Skip deleting \'{directoryName}\\{directoryNameOrPath}\'.");
                    }
                }
                else
                {
                    Console.WriteLine($"The directory \'{directoryName}\\{directoryNameOrPath}\' is not empty.");
                }
            }
            else
            {
                if (deleteIndex == Directory.notFoundIndex)
                {
                    Console.WriteLine($"Error: this directory \'{directoryName}\\{directoryNameOrPath}\' is not exist!");
                }
                else if (Program.currentDirectory.Entries[deleteIndex].Attr != DirectoryEntry.attDirectory)
                {
                    Console.WriteLine($"\'{directoryName}\\{directoryNameOrPath}\' is not a directory.");
                }
            }
            Program.currentDirectory = originalDirectory;
            return;
        }

        public static void rd_a(string Path)
        {
            Directory originalDirectory = Program.currentDirectory;
            if (Path[Path.Length - 1] == '\\')
                Path = Path.TrimEnd('\\');

            string[] fullPathDir = Path.Split('\\');
            string directoryName;
            if (fullPathDir.Length > 1)
            {
                directoryName = fullPathDir[fullPathDir.Length - 1];
                Path = string.Join("\\", fullPathDir, 0, fullPathDir.Length - 1);
                if (cd(Path, Program.currentDirectory) == -1)
                    return;
            }
            else
            {
                directoryName = Path;
                Path = Program.getPath();
            }
            if (directoryName == $"{Program.rootDir}:")
            {
                string User = "n";
                do
                {
                    Console.Write($"Are you sure that you want to delete root Directory \'{Program.rootDir}:\\\',Please enter Y for Yes or N for No >> ");
                    User = Console.ReadLine().ToLower();
                }
                while (User != "y" && User != "n");
                if (User == "y")
                {
                    for (int index = 5; index < 1024; index++)
                        MiniFat.setClusterPointer(index, MiniFat.emptyCluster);
                    MiniFat.writeFat();
                    Program.root.firstCluster = MiniFat.emptyCluster;
                    Program.currentDirectory = Program.root;
                    Console.WriteLine($"Directory \'{directoryName}\' deleted successfully.");
                    return;
                }
                else
                    Console.WriteLine($"Skip deleting Root Directory \'{directoryName}\'.");

            }

            else
            {
                int indexDirDel = Program.currentDirectory.searchDirectory(directoryName);
                if (indexDirDel != Directory.notFoundIndex && Program.currentDirectory.Entries[indexDirDel].Attr == DirectoryEntry.attDirectory)
                {
                    Directory delDir = new Directory(directoryName,
                        DirectoryEntry.attDirectory,
                        Program.currentDirectory.Entries[indexDirDel].firstCluster,
                        Program.currentDirectory);
                    delDir.ReadDirectory();

                    int Size = delDir.Entries.Count();
                    if (Size > 0)
                    {
                        for (int count = 0; count < Size; count++)
                        {
                            if (delDir.Entries[count].Attr == DirectoryEntry.attDirectory)
                            {
                                string subdirectoryPath = Path + '\\' + directoryName + '\\' + new string(delDir.Entries[count].Name).Trim();
                                rd_a(subdirectoryPath);
                            }
                            else if (delDir.Entries[count].Attr == DirectoryEntry.attFile)
                            {
                                string filePath = Path + '\\' + directoryName;
                                Parser.del(filePath, false);
                            }
                        }
                        Parser.rd($"{Path}\\{directoryName}", false);
                    }
                    else
                        Parser.rd($"{Path}\\{directoryName}", false);

                }
                else
                {
                    Console.WriteLine($"Error: this directory '{Path}\\{directoryName}' is not exist!");
                    Program.currentDirectory = originalDirectory;
                    return;
                }
            }
            Program.currentDirectory = originalDirectory;
            return;
        }

        public static void del(string path, bool askConfirmation)
        {
            Directory originalDirectory = Program.currentDirectory;
            string fileName;

            if (path[path.Length - 1] == '\\')
                path = path.TrimEnd('\\');
            string[] fullPath = path.Split('\\');

            if (fullPath.Length > 1)
            {
                path = string.Join("\\", fullPath, 0, fullPath.Length - 1);
                fileName = fullPath[(fullPath.Length) - 1];
                if (cd(path, Program.currentDirectory) == -1)
                    return;
            }
            else
            {
                fileName = path;
                path = Program.getPath();
            }

            if (fileName == $"{Program.rootDir}:")
            {
                fileName = Program.rootDir;
                string User;
                do
                {
                    Console.Write($"Are you sure that you want to delete All Files in Root Directory \'{path}\' , please Enter Y for yes or N for No :>> ");
                    User = Console.ReadLine().ToLower();
                } while (User != "y" && User != "n");
                if (User == "y")
                {
                    Directory DirFilesDel = new Directory(fileName, DirectoryEntry.attDirectory, MiniFat.firstClusterEmptyInNew, null);
                    DirFilesDel.ReadDirectory();
                    int Size = DirFilesDel.Entries.Count();
                    for (int size = 0; size < Size; size++)
                    {
                        if (DirFilesDel.Entries[size].Attr == DirectoryEntry.attFile)
                            del($"{path}\\{new string(DirFilesDel.Entries[size].Name).Trim()}", false);
                    }
                    Program.currentDirectory = originalDirectory;
                    return;
                }
                else
                {
                    Console.WriteLine($"Skip delete Files From Root Directory \'{path}\'.");
                }

            }
            int index = Program.currentDirectory.searchDirectory(fileName);
            if (index != Directory.notFoundIndex && Program.currentDirectory.Entries[index].Attr == DirectoryEntry.attDirectory)
            {
                string User = "y";
                if (askConfirmation)
                {
                    do
                    {
                        Console.Write($"Are you sure that you want to delete All Files in Directory \'{path}\\{fileName}\' , please Enter Y for yes or N for No :>> ");
                        User = Console.ReadLine().ToLower();
                    } while (User != "y" && User != "n");
                }
                if (User == "y")
                {
                    Directory DirFilesDel = new Directory(fileName,
                        DirectoryEntry.attDirectory,
                        Program.currentDirectory.Entries[index].firstCluster,
                        Program.currentDirectory);
                    DirFilesDel.ReadDirectory();

                    int Size = DirFilesDel.Entries.Count();
                    for (int size = 0; size < Size; size++)
                    {
                        if (DirFilesDel.Entries[size].Attr == DirectoryEntry.attFile)
                        {
                            del($"{path}\\{fileName}\\{new string(DirFilesDel.Entries[size].Name).Trim()}", false);
                        }
                    }
                    Program.currentDirectory = originalDirectory;
                    return;
                }
                else
                {
                    Console.WriteLine($"Skip delete '{path}\\{fileName}'");
                }
            }
            else
            {
                if (index != Directory.notFoundIndex)
                {
                    string User = "y";
                    if (askConfirmation)
                    {
                        do
                        {
                            Console.Write($"Are you sure that you want to delete \'{path}\\{fileName}\' , please Enter Y for yes or N for No :>> ");
                            User = Console.ReadLine().ToLower();
                        } while (User != "y" && User != "n");
                    }
                    if (User == "y")
                    {
                        FileEntry file = new FileEntry(fileName, DirectoryEntry.attFile,
                            Program.currentDirectory.Entries[index].firstCluster,
                            Program.currentDirectory, "",
                            Program.currentDirectory.Entries[index].Size);
                        file.deleteFile();
                        Console.WriteLine($"Directory \'{path}\\{fileName}\' deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Skip delete '{path}\\{fileName}'");
                    }
                }
                else
                {
                    Console.WriteLine($"This file : \'{path}\\{fileName}\' does not exist on your Disk!");
                }
            }
            Program.currentDirectory = originalDirectory;
            return;
        }

        public static void rename(string oldName, string newName)
        {
            Directory originalDirectory = Program.currentDirectory;
            if (newName.Split('\\').Length > 1)
            {
                Console.WriteLine("Error : the new file name should be a file name only you cannot provide a full path!");
                return;
            }

            if (oldName[oldName.Length - 1] == '\\')
            {
                oldName = oldName.TrimEnd('\\');
            }

            string[] fullPathOldName = oldName.Split('\\');
            string oldNameFile = fullPathOldName[fullPathOldName.Length - 1];

            if (fullPathOldName.Length > 1)
            {
                oldName = string.Join("\\", fullPathOldName, 0, fullPathOldName.Length - 1);
                if (cd(oldName, Program.currentDirectory) == -1)
                    return;
            }

            int indexOldNameFile = Program.currentDirectory.searchDirectory(oldNameFile);
            int indexNewNameFile = Program.currentDirectory.searchDirectory(newName);

            if (indexOldNameFile != Directory.notFoundIndex)
            {
                if (Program.currentDirectory.Entries[indexOldNameFile].Attr == DirectoryEntry.attFile)
                {
                    if (indexNewNameFile == Directory.notFoundIndex)
                    {
                        if (!DirectoryEntry.ISValidName(newName, DirectoryEntry.attFile))
                        {
                            Console.WriteLine("Error : this file name is invalid name");
                            Program.currentDirectory = originalDirectory;
                            return;
                        }

                        int firstCluster = Program.currentDirectory.Entries[indexOldNameFile].firstCluster;
                        int fileSize = Program.currentDirectory.Entries[indexOldNameFile].Size;
                        FileEntry file = new FileEntry(oldNameFile, DirectoryEntry.attFile, firstCluster, Program.currentDirectory, "", fileSize);
                        file.ReadFile();

                        file.Name = newName.ToCharArray();
                        file.WriteFile();

                        Program.currentDirectory.Entries.RemoveAt(indexOldNameFile);
                        Program.currentDirectory.Entries.Insert(indexOldNameFile, file);
                        Program.currentDirectory.WriteDirectory();
                        Console.WriteLine($"Success Rename File \'{oldNameFile}\' to \'{newName}\'");
                    }
                    else
                        Console.WriteLine($"This New Name : \'{newName}\' is already found on this Directory.");

                }
                else if (Program.currentDirectory.Entries[indexOldNameFile].Attr == DirectoryEntry.attDirectory)
                {
                    if (indexNewNameFile == Directory.notFoundIndex)
                    {
                        if (!DirectoryEntry.ISValidName(newName, DirectoryEntry.attDirectory))
                        {
                            Console.WriteLine("Error : this directory name is invalid name");
                            Program.currentDirectory = originalDirectory;
                            return;
                        }

                        int firstCluster = Program.currentDirectory.Entries[indexOldNameFile].firstCluster;
                        Directory Dir = new Directory(oldNameFile,
                            DirectoryEntry.attDirectory,
                            firstCluster,
                            Program.currentDirectory);
                        Dir.ReadDirectory();

                        Directory newDir = new Directory(newName, DirectoryEntry.attDirectory
                            , firstCluster,
                            Program.currentDirectory);

                        Program.currentDirectory.updateContent(Dir, newDir);
                        Console.WriteLine($"Success Rename Directory \'{oldNameFile}\' to \'{newName}\'");
                    }
                    else
                        Console.WriteLine($"This New Name : \'{newName}\' is already found on this Directory.");

                }
            }
            else
                Console.WriteLine($"This File or Directory : \'{oldNameFile}\' does not exist on disk!");

            Program.currentDirectory = originalDirectory;
        }

        public static int copy(string sourcePath, string destinationPath, bool showOutput)
        {
            int filesCopiedCount = 0;
            Directory originalDirectory = Program.currentDirectory;
            Directory sourceDir = Program.currentDirectory;
            Directory distanceDir = Program.currentDirectory;

            if (sourcePath[sourcePath.Length - 1] == '\\')
                sourcePath = sourcePath.TrimEnd('\\');

            string[] fullPathSource = sourcePath.Split('\\');
            string[] fullPathDistance;
            string nameFileSource;
            string nameFileDistance;

            if (fullPathSource.Length > 1)
            {
                sourcePath = string.Join("\\", fullPathSource, 0, fullPathSource.Length - 1);
                if (cd(sourcePath, sourceDir) == -1)
                    return 0;
                nameFileSource = fullPathSource[fullPathSource.Length - 1];
                sourceDir = Program.currentDirectory;
            }
            else
            {
                nameFileSource = sourcePath;
                sourcePath = Program.getPath();
            }

            if (!string.IsNullOrEmpty(destinationPath))
            {
                if (destinationPath[destinationPath.Length - 1] == '\\')
                    destinationPath = destinationPath.TrimEnd('\\');

                fullPathDistance = destinationPath.Split('\\');
                nameFileDistance = fullPathDistance[fullPathDistance.Length - 1];

                if (fullPathDistance.Length > 1)
                {
                    destinationPath = string.Join("\\", fullPathDistance, 0, fullPathDistance.Length - 1);
                    if (cd(destinationPath, distanceDir) == -1)
                        return 0;
                    distanceDir = Program.currentDirectory;
                    nameFileDistance = fullPathDistance[fullPathDistance.Length - 1];
                }
                else
                {
                    nameFileDistance = destinationPath;
                    Program.currentDirectory = distanceDir;
                    Program.setPath();
                    destinationPath = Program.getPath();
                }
            }
            else
            {
                fullPathDistance = new string[0];
                Program.currentDirectory = originalDirectory;
                Program.setPath();
                destinationPath = Program.getPath();
                nameFileDistance = nameFileSource;
            }

            int indexNameFileDistance = distanceDir.searchDirectory(nameFileDistance);
            int indexNameFileSource = sourceDir.searchDirectory(nameFileSource);
            if (indexNameFileSource != Directory.notFoundIndex || (nameFileSource == $"{Program.rootDir}:"))
            {
                if (indexNameFileSource != Directory.notFoundIndex && sourceDir.Entries[indexNameFileSource].Attr == DirectoryEntry.attFile)
                {
                    if ((nameFileDistance != nameFileSource || sourceDir != distanceDir) && indexNameFileDistance == Directory.notFoundIndex)
                    {
                        if (!DirectoryEntry.ISValidName(nameFileDistance, DirectoryEntry.attFile))
                        {
                            Console.WriteLine("Error : this file name is invalid name");
                            Program.currentDirectory = originalDirectory;
                            return 0;
                        }
                        FileEntry oldFile = new FileEntry(nameFileSource, DirectoryEntry.attFile,
                            sourceDir.Entries[indexNameFileSource].firstCluster,
                            sourceDir, "", sourceDir.Entries[indexNameFileSource].Size);
                        oldFile.ReadFile();
                        FileEntry newFile = new FileEntry(nameFileDistance, DirectoryEntry.attFile,
                            MiniFat.emptyCluster, distanceDir, oldFile.Content,
                            oldFile.Size);
                        if (distanceDir.canAddEntry(newFile.getDirectoryEntry()))
                        {
                            distanceDir.addEntry(newFile.getDirectoryEntry());
                            newFile.WriteFile();
                            Console.WriteLine($"{sourcePath}\\{nameFileSource}");
                            filesCopiedCount++;
                        }
                        else
                        {
                            Console.WriteLine("\tNo space available to import new file.");
                        }

                    }
                    else
                    {
                        if ((nameFileDistance == $"{Program.rootDir}:") || (indexNameFileDistance != Directory.notFoundIndex && distanceDir.Entries[indexNameFileDistance].Attr == DirectoryEntry.attDirectory))
                        {
                            cd($"{destinationPath}\\{nameFileDistance}", distanceDir);
                            distanceDir = Program.currentDirectory;
                            Program.setPath();
                            destinationPath = Program.getPath();
                            filesCopiedCount += Parser.copy($"{sourcePath}\\{nameFileSource}", $"{destinationPath}\\{nameFileSource}", false);
                        }
                        else if (sourceDir.firstCluster == distanceDir.firstCluster && nameFileDistance == nameFileSource)
                            Console.WriteLine($"This file \'{sourcePath}\\{nameFileSource}\' cannot be copied onto itself");
                        else
                        {
                            Console.WriteLine($"This file \'{destinationPath}\\{nameFileDistance}\' is already found in Directory.");
                            string User;
                            do
                            {
                                Console.Write($"Note: do you want to overwrite this file \'{nameFileDistance}\' , please enter y for Yes or n for No >> ");
                                User = Console.ReadLine().ToLower();
                            } while (User != "y" && User != "n");
                            if (User == "y")
                            {
                                FileEntry oldFile = new FileEntry(nameFileSource, DirectoryEntry.attFile, sourceDir.Entries[indexNameFileSource].firstCluster, sourceDir, "", sourceDir.Entries[indexNameFileSource].Size);
                                oldFile.ReadFile();
                                FileEntry newFile = new FileEntry(nameFileDistance, DirectoryEntry.attFile, distanceDir.Entries[indexNameFileDistance].firstCluster, distanceDir, "", distanceDir.Entries[indexNameFileDistance].Size);
                                if (distanceDir.canAddEntry(newFile.getDirectoryEntry()))
                                {
                                    distanceDir.Entries.RemoveAt(indexNameFileDistance);
                                    newFile.Content = oldFile.Content;
                                    newFile.Size = oldFile.Size;
                                    distanceDir.Entries.Insert(indexNameFileDistance, newFile.getDirectoryEntry());
                                    newFile.WriteFile();
                                    Console.WriteLine($"{sourcePath}\\{nameFileSource}");
                                    filesCopiedCount++;
                                }
                                else
                                {
                                    Console.WriteLine("\tNo space available to import new file.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Skip overwrite '{nameFileDistance}'");
                            }
                        }
                    }
                }
                else
                {
                    if (indexNameFileDistance != Directory.notFoundIndex && distanceDir.Entries[indexNameFileDistance].Attr == DirectoryEntry.attDirectory || fullPathDistance.Length == 0 || nameFileDistance == $"{Program.rootDir}:")
                    {
                        if (cd(nameFileSource, sourceDir) == -1)
                            return -1;
                        sourceDir = Program.currentDirectory;
                        sourceDir.ReadDirectory();
                        Program.currentDirectory = originalDirectory;
                        int size = sourceDir.Entries.Count();
                        int IndexNameFileDistance = distanceDir.searchDirectory(nameFileDistance);
                        if (IndexNameFileDistance != Directory.notFoundIndex && distanceDir.Entries[IndexNameFileDistance].Attr == DirectoryEntry.attDirectory)
                        {
                            for (int i = 0; i < size; i++)
                            {
                                if (sourceDir.Entries[i].Attr == DirectoryEntry.attFile)
                                {
                                    if (fullPathDistance.Length > 0)
                                        filesCopiedCount += Parser.copy($"{sourcePath}\\{nameFileSource}\\{new string(sourceDir.Entries[i].Name).Trim()}", $"{destinationPath}\\{nameFileDistance}\\{new string(sourceDir.Entries[i].Name).Trim()}", false);

                                    else
                                        filesCopiedCount += Parser.copy($"{sourcePath}\\{nameFileSource}\\{new string(sourceDir.Entries[i].Name).Trim()}", $"{destinationPath}\\", false);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < size; i++)
                            {
                                if (sourceDir.Entries[i].Attr == DirectoryEntry.attFile)
                                {
                                    filesCopiedCount += Parser.copy($"{sourcePath}\\{nameFileSource}\\{new string(sourceDir.Entries[i].Name).Trim()}", $"{destinationPath}\\{nameFileDistance}\\{new string(sourceDir.Entries[i].Name).Trim()}", false);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!DirectoryEntry.ISValidName(nameFileDistance, DirectoryEntry.attFile))
                        {
                            Console.WriteLine($"This path : \'{destinationPath}\\{nameFileDistance}\' does not exist on your Disk!");
                            Program.currentDirectory = originalDirectory;
                            return 0;
                        }
                        else
                        {
                            bool isNew = true;
                            int indexFileDistance = distanceDir.searchDirectory(nameFileDistance);
                            if (indexFileDistance != Directory.notFoundIndex)
                            {
                                string User;
                                do
                                {
                                    Console.Write($"Note: do you want to overwrite this file \'{nameFileDistance}\' , please enter y for Yes or n for No >> ");
                                    User = Console.ReadLine().ToLower();
                                } while (User != "y" && User != "n");
                                if (User == "n")
                                {
                                    Console.WriteLine($"Skip overwrite '{nameFileDistance}'");
                                    Program.currentDirectory = originalDirectory;
                                    return 0;
                                }
                                else
                                {
                                    isNew = false;
                                }
                            }
                            string Content = "";
                            cd(nameFileSource, sourceDir);
                            sourceDir = Program.currentDirectory;
                            sourceDir.ReadDirectory();
                            int size = sourceDir.Entries.Count();
                            Program.currentDirectory = originalDirectory;
                            for (int i = 0; i < size; i++)
                            {
                                if (sourceDir.Entries[i].Attr == DirectoryEntry.attFile)
                                {
                                    FileEntry file = new FileEntry(new string(sourceDir.Entries[i].Name).Trim(), DirectoryEntry.attFile,
                                        sourceDir.Entries[i].firstCluster, sourceDir,
                                        "", sourceDir.Entries[i].Size);
                                    file.ReadFile();
                                    Content += file.Content;
                                    if (nameFileSource == $"{Program.rootDir}:")
                                        Console.WriteLine($"{sourcePath}\\{new string(sourceDir.Entries[i].Name).Trim()}");
                                    else
                                        Console.WriteLine($"{sourcePath}\\{nameFileSource}\\{new string(sourceDir.Entries[i].Name).Trim()}");
                                }
                            }
                            filesCopiedCount++;

                            FileEntry newFile = new FileEntry(nameFileDistance, DirectoryEntry.attFile, MiniFat.emptyCluster, distanceDir, Content, Content.Length);
                            if (distanceDir.canAddEntry(newFile.getDirectoryEntry()))
                            {
                                if (!isNew)
                                {
                                    FileEntry oldFile = new FileEntry(nameFileDistance, DirectoryEntry.attFile,
                                        distanceDir.Entries[indexFileDistance].firstCluster,
                                        distanceDir, "", distanceDir.Entries[indexFileDistance].Size);
                                    distanceDir.Entries.RemoveAt(indexFileDistance);
                                    oldFile.Content = Content;
                                    oldFile.Size = Content.Length;
                                    distanceDir.Entries.Insert(indexFileDistance, oldFile.getDirectoryEntry());
                                    oldFile.WriteFile();
                                }
                                else
                                {
                                    distanceDir.addEntry(newFile.getDirectoryEntry());
                                    newFile.WriteFile();
                                }

                            }
                            else
                            {
                                Console.WriteLine("\tNo space available to import new file.");
                            }
                        }

                    }
                }
            }
            else
            {
                Console.WriteLine($"This file : \'{sourcePath}\\{nameFileSource}\' does not exist on your disk!");
                Program.currentDirectory = originalDirectory;
                return 0;
            }
            if (showOutput)
                Console.WriteLine($"\t{filesCopiedCount} file(s) copied.");
            Program.currentDirectory = originalDirectory;
            return filesCopiedCount;
        }

        public static int import(string path1, string path2, bool show)
        {
            Directory originalDirectory = Program.currentDirectory;
            bool sourceDir;
            int Counter = 0;
            string[] fullPathDistance;
            string nameDirOrFileDistance;
            int indexNameDirOrFileDistance;

            if (System.IO.File.Exists(path1))
            {
                sourceDir = false;
            }
            else if (System.IO.Directory.Exists(path1))
            {
                sourceDir = true;
            }
            else
            {
                if (!(path1.Split('\\').Length > 1))
                {
                    path1 = $"{System.IO.Directory.GetCurrentDirectory()}\\{path1}";
                }
                string name = Path.GetFileName(path1);
                path1 = path1.Substring(0, path1.LastIndexOf('\\'));
                if (!System.IO.Directory.Exists(path1))
                {
                    Console.WriteLine($"Could not find a part of the path \'{path1}\'.");
                    Console.WriteLine($"this path: \'{path1}\' does not exist on your computer!");
                }
                else
                {
                    Console.WriteLine($"Could not find file \'{path1}\\{name}\'");
                    Console.WriteLine($"This file: \'{name}\' does not exist on your computer!");
                }
                return 0;
            }

            if (!string.IsNullOrEmpty(path2))
            {
                if (path2[path2.Length - 1] == '\\')
                    path2 = path2.TrimEnd('\\');
                fullPathDistance = path2.Split('\\');
                if (fullPathDistance.Length > 1)
                {
                    nameDirOrFileDistance = fullPathDistance[fullPathDistance.Length - 1];
                    path2 = string.Join('\\', fullPathDistance, 0, fullPathDistance.Length - 1);
                    if (cd(path2, Program.currentDirectory) == -1)
                        return 0;
                }
                else
                {
                    nameDirOrFileDistance = fullPathDistance[fullPathDistance.Length - 1];
                    path2 = Program.getPath();
                }
            }
            else
            {
                path2 = Program.getPath();
                if (sourceDir)
                    nameDirOrFileDistance = null;
                else
                    nameDirOrFileDistance = Path.GetFileName(path1);
            }

            if (sourceDir)
            {
                if (string.IsNullOrEmpty(nameDirOrFileDistance))
                {
                    string[] files = System.IO.Directory.GetFiles(path1);
                    foreach (string file in files)
                    {
                        Counter += import($"{Path.GetFullPath(file)}", $"{path2}\\{Path.GetFileName(file)}", false);
                    }
                }
                else
                {
                    string Content = "";
                    indexNameDirOrFileDistance = Program.currentDirectory.searchDirectory(nameDirOrFileDistance);
                    if ((nameDirOrFileDistance == $"{Program.rootDir}:") || (indexNameDirOrFileDistance != Directory.notFoundIndex && Program.currentDirectory.Entries[indexNameDirOrFileDistance].Attr == DirectoryEntry.attDirectory))
                    {
                        cd(nameDirOrFileDistance, Program.currentDirectory);
                        import($"{Path.GetFullPath(path1)}", "", true);
                        Program.currentDirectory = originalDirectory;
                        return 0;
                    }
                    FileEntry oldFile = new FileEntry(nameDirOrFileDistance, DirectoryEntry.attFile, MiniFat.emptyCluster, Program.currentDirectory, "", 0);
                    if (indexNameDirOrFileDistance != Directory.notFoundIndex)
                    {
                        string User;
                        Console.WriteLine($"This Name : \'{nameDirOrFileDistance}\' already found in this Directory.");
                        do
                        {
                            Console.Write($"Note : do you want to overwrite this file : \'{nameDirOrFileDistance}\',please enter y for Yes or n for No >> ");
                            User = Console.ReadLine().ToLower();
                        } while (User != "y" && User != "n");
                        if (User == "y")
                        {
                            oldFile = new FileEntry(nameDirOrFileDistance, DirectoryEntry.attFile, Program.currentDirectory.Entries[indexNameDirOrFileDistance].firstCluster, Program.currentDirectory, "", Program.currentDirectory.Entries[indexNameDirOrFileDistance].Size);
                            oldFile.ReadFile();
                        }
                        else
                        {
                            Console.WriteLine($"Skip Import Directory {Path.GetFileName(path1)}");
                            Program.currentDirectory = originalDirectory;
                            return 0;
                        }
                    }
                    string[] files = System.IO.Directory.GetFiles(path1);
                    foreach (string file in files)
                    {
                        Console.WriteLine($"{Path.GetFileName(file)}");
                        Content += System.IO.File.ReadAllText(file);
                    }
                    Counter++;
                    oldFile.Content = Content;
                    oldFile.Size = Content.Length;
                    if (Program.currentDirectory.canAddEntry(oldFile))
                    {
                        if (oldFile.firstCluster != MiniFat.emptyCluster)
                        {
                            Program.currentDirectory.Entries.RemoveAt(indexNameDirOrFileDistance);
                            Program.currentDirectory.Entries.Insert(indexNameDirOrFileDistance, oldFile.getDirectoryEntry());
                        }
                        else
                        {
                            Program.currentDirectory.addEntry(oldFile.getDirectoryEntry());
                        }
                        oldFile.WriteFile();
                    }
                    else
                    {
                        Console.WriteLine("\tNo space available to import new file.");
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(nameDirOrFileDistance))
                {
                    nameDirOrFileDistance = Path.GetFileName(path1);
                }
                indexNameDirOrFileDistance = Program.currentDirectory.searchDirectory(nameDirOrFileDistance);
                if (indexNameDirOrFileDistance != Directory.notFoundIndex && Program.currentDirectory.Entries[indexNameDirOrFileDistance].Attr == DirectoryEntry.attDirectory)
                {
                    cd(nameDirOrFileDistance, Program.currentDirectory);
                    import($"{Path.GetFullPath(path1)}", "", true);
                    Program.currentDirectory = originalDirectory;
                    return 0;
                }
                FileEntry oldFile = new FileEntry(nameDirOrFileDistance, DirectoryEntry.attFile, MiniFat.emptyCluster, Program.currentDirectory, "", 0);

                if (indexNameDirOrFileDistance != Directory.notFoundIndex)
                {
                    string User;
                    Console.WriteLine($"This Name : \'{nameDirOrFileDistance}\' already found in this Directory.");
                    do
                    {
                        Console.Write($"Note : do you want to overwrite this file : \'{nameDirOrFileDistance}\',please enter y for Yes or n for No >> ");
                        User = Console.ReadLine().ToLower();
                    } while (User != "y" && User != "n");
                    if (User == "y")
                    {
                        oldFile = new FileEntry(nameDirOrFileDistance, DirectoryEntry.attFile, Program.currentDirectory.Entries[indexNameDirOrFileDistance].firstCluster, Program.currentDirectory, "", Program.currentDirectory.Entries[indexNameDirOrFileDistance].Size);
                        oldFile.ReadFile();
                    }
                    else
                    {
                        Console.WriteLine($"Skip Import File {Path.GetFileName(path1)}");
                        Program.currentDirectory = originalDirectory;
                        return 0;
                    }
                }
                oldFile.Content = System.IO.File.ReadAllText(path1);
                oldFile.Size = System.IO.File.ReadAllText(path1).Length;
                if (Program.currentDirectory.canAddEntry(oldFile))
                {
                    if (oldFile.firstCluster != MiniFat.emptyCluster)
                    {
                        Program.currentDirectory.Entries.RemoveAt(indexNameDirOrFileDistance);
                        Program.currentDirectory.Entries.Insert(indexNameDirOrFileDistance, oldFile.getDirectoryEntry());
                    }
                    else
                    {
                        Program.currentDirectory.addEntry(oldFile.getDirectoryEntry());
                    }
                    oldFile.WriteFile();
                    Console.WriteLine($"{Path.GetFileName(path1)}");
                    Counter++;
                }
                else
                {
                    Console.WriteLine("\tNo space available to import new file.");
                }
            }
            Program.currentDirectory = originalDirectory;
            if (show)
                Console.WriteLine($"\t{Counter} file(s) imported.");
            return Counter;
        }

        public static int export(string path1, string path2, bool show)
        {
            Directory originalDirectory = Program.currentDirectory;
            int Counter = 0;
            string[] fullPathSource;
            string nameDirOrFileSource;
            string nameDirOrFileDistance;
            int indexNameDirOrFileSource;

            if (path1[path1.Length - 1] == '\\')
                path1 = path1.TrimEnd('\\');

            fullPathSource = path1.Split('\\');
            if (fullPathSource.Length > 1)
            {
                nameDirOrFileSource = fullPathSource[fullPathSource.Length - 1];
                path1 = string.Join('\\', fullPathSource, 0, fullPathSource.Length - 1);
                if (cd(path1, Program.currentDirectory) == -1)
                    return 0;
            }
            else
            {
                nameDirOrFileSource = fullPathSource[fullPathSource.Length - 1];
                path1 = Program.getPath();
            }

            indexNameDirOrFileSource = Program.currentDirectory.searchDirectory(nameDirOrFileSource);

            if ((nameDirOrFileSource == $"{Program.rootDir}:") || (indexNameDirOrFileSource != Directory.notFoundIndex && Program.currentDirectory.Entries[indexNameDirOrFileSource].Attr == DirectoryEntry.attDirectory))
            {
                if (string.IsNullOrEmpty(path2) || System.IO.Directory.Exists(path2))
                {
                    cd(nameDirOrFileSource, Program.currentDirectory);
                    Program.currentDirectory.ReadDirectory();
                    int Size = Program.currentDirectory.Entries.Count();
                    for (int size = 0; size < Size; size++)
                    {
                        if (Program.currentDirectory.Entries[size].Attr == DirectoryEntry.attFile)
                            Counter += export($"{path1}\\{nameDirOrFileSource}\\{new string(Program.currentDirectory.Entries[size].Name).Trim()}", $"{path2}", false);
                    }
                }
                else
                {
                    string Content = "";
                    if (System.IO.File.Exists(path2))
                    {
                        nameDirOrFileDistance = Path.GetFileName(path2);

                        string User;
                        Console.WriteLine($"This Name : \'{nameDirOrFileDistance}\' already found in this Directory in physical disk!.");
                        do
                        {
                            Console.Write($"Note : do you want to overwrite this file : \'{nameDirOrFileDistance}\',please enter y for Yes or n for No >> ");
                            User = Console.ReadLine().ToLower();
                        } while (User != "y" && User != "n");
                        if (User == "n")
                        {
                            Console.WriteLine($"Skip Export Directory {Path.GetFileName(path2)}");
                            Program.currentDirectory = originalDirectory;
                            return 0;
                        }
                    }

                    cd(nameDirOrFileSource, Program.currentDirectory);
                    Program.currentDirectory.ReadDirectory();
                    int Size = Program.currentDirectory.Entries.Count();
                    for (int size = 0; size < Size; size++)
                    {
                        if (Program.currentDirectory.Entries[size].Attr == DirectoryEntry.attFile)
                        {
                            DirectoryEntry fileEntry = new DirectoryEntry(new string(Program.currentDirectory.Entries[size].Name).Trim(),
                                DirectoryEntry.attFile, Program.currentDirectory.Entries[size].firstCluster,
                                Program.currentDirectory.Entries[size].Size);
                            FileEntry file = new FileEntry(fileEntry, Program.currentDirectory);
                            file.ReadFile();
                            Console.WriteLine($"{path1}\\{new string(file.Name).Trim()}");
                            Content += file.Content;
                        }
                    }
                    Counter++;
                    System.IO.File.WriteAllText(path2, Content);
                }
            }

            else if (indexNameDirOrFileSource != Directory.notFoundIndex && Program.currentDirectory.Entries[indexNameDirOrFileSource].Attr == DirectoryEntry.attFile)
            {
                if (string.IsNullOrEmpty(path2))
                {
                    path2 = Environment.CurrentDirectory;
                }

                if (System.IO.Directory.Exists(path2))
                {

                    path2 = $"{path2}\\{nameDirOrFileSource}";
                    nameDirOrFileDistance = $"{path2}\\{nameDirOrFileSource}";
                }

                if (System.IO.File.Exists(path2))
                {
                    nameDirOrFileDistance = Path.GetFileName(path2);

                    string User;
                    Console.WriteLine($"This Name : \'{nameDirOrFileDistance}\' already found in this Directory in physical disk!.");
                    do
                    {
                        Console.Write($"Note : do you want to overwrite this file : \'{nameDirOrFileDistance}\',please enter y for Yes or n for No >> ");
                        User = Console.ReadLine().ToLower();
                    } while (User != "y" && User != "n");
                    if (User == "n")
                    {
                        Console.WriteLine($"Skip Export Directory {Path.GetFileName(path2)}");
                        Program.currentDirectory = originalDirectory;
                        return 0;
                    }
                }
                else
                {
                    nameDirOrFileDistance = path2;
                }

                DirectoryEntry fileEntry = new DirectoryEntry(nameDirOrFileSource, DirectoryEntry.attFile, Program.currentDirectory.Entries[indexNameDirOrFileSource].firstCluster, Program.currentDirectory.Entries[indexNameDirOrFileSource].Size);
                FileEntry file = new FileEntry(fileEntry, Program.currentDirectory);
                file.ReadFile();
                Console.WriteLine($"{path1}\\{new string(file.Name).Trim()}");

                System.IO.File.WriteAllText(nameDirOrFileDistance, file.Content);
                Counter++;
            }

            else
            {
                if (DirectoryEntry.ISValidName(nameDirOrFileSource, DirectoryEntry.attFile))
                    Console.WriteLine($"This file : \'{path1}\\{nameDirOrFileSource}\' does not exist on your disk!");
                else
                    Console.WriteLine($"This path : \'{path1}\\{nameDirOrFileSource}\' does not exist on your disk!");

                Program.currentDirectory = originalDirectory;
                return 0;
            }

            if (show)
                Console.WriteLine($"\t{Counter} file(s) exported.");
            Program.currentDirectory = originalDirectory;
            return Counter;
        }

        public static void cls()
        {
            Console.Clear();
        }

        public static void quit()
        {
            MiniFat.CloseTheSystem();
        }

        public static void help(string Command)
        {
            if (string.IsNullOrEmpty(Command))
            {
                Console.WriteLine("cd           - Change the current default directory to.\n               If the argument is not present, report the current directory.\n               If the directory does not exist an appropriate error should be reported.");
                Console.WriteLine("cls          - Clear the screen.");
                Console.WriteLine("dir          - List the contents of directory.");
                Console.WriteLine("quit         - Quit the shell.");
                Console.WriteLine("copy         - Copies one or more files to another location.");
                Console.WriteLine("del          - Deletes one or more files.");
                Console.WriteLine("help         - Get Information about Commands.");
                Console.WriteLine("md           - Creates a directory.");
                Console.WriteLine("rd           - Removes a directory.");
                Console.WriteLine("rename       - Renames a file.");
                Console.WriteLine("type         - Displays the contents of a text file.");
                Console.WriteLine("import       - import text file(s) from your computer.");
                Console.WriteLine("export       - export text file(s) to your computer.");

            }
            else
            {
                switch (Command)
                {
                    case "help":
                        Console.WriteLine($"\n{Command} -- This Command Used to get information about Command. \n\t1-help --- Get Information about All Commands.\n\t2-help [Command] ---- Get Information about Command.\n");
                        break;
                    case "cls":
                        Console.WriteLine($"\n{Command} -- This Command Used to Clear Screen CMD.\n\t1-cls --- To clean screen.");
                        break;
                    case "quit":
                        Console.WriteLine($"\n{Command} -- This Command Used to Exit From Application CMD.\n\t1-quit --- To exit my application.");
                        break;
                    case "cd":
                        Console.WriteLine($"\n{Command} -- This Command Used to Change Current Directory.\n\t1-cd --- Show path currant Directory.\n\t2-cd . ---- Get current Directory.\n\t3-cd .. ----- Get Parent Current Directory.\n\t4-cd [Path] ------ Get [Path] Directory.\n");
                        break;
                    case "md":
                        Console.WriteLine($"\n{Command} -- This Command Used to Make Directory.\n\t1- md [path\\name] --- Make Directory in path by Name <name>.\n\t2- md -m [path\\name] ---- Make Directory in path by Name and Create Directory if not found from path.\n");
                        break;
                    case "rd":
                        Console.WriteLine($"\n{Command} -- This Command Used to Remove Directory.\n\t1-rd [path\\Directory Name]+ --- Delete Directory by Name [name] if empty.\n\t2- rd -a [path\\Directory Name]+ ---- Delete Directory by Name [name] with not care if empty or not\n");
                        break;
                    case "dir":
                        Console.WriteLine($"\n{Command} -- This Command Used to Show List in Directory.\n\t1-dir --- Show list currant Directory.\n\t2-dir .  ---- Show all list currant Directory.\n\t3-dir ..  ----- Show all list parent Directory.\n\t4-dir [Path]+ ----- Show all list path Directory.");
                        break;
                    case "type":
                        Console.WriteLine($"\n{Command} -- This Command Used to Show Content in Files.\n\t1-type [Name]+ --- Show content in file name in currant directory.\n\t2-type [Path\\Name]+  ---- Show content in file name in path directory.\n\t");
                        break;
                    case "del":
                        Console.WriteLine($"\n{Command} -- This Command Used to del Files.\n\t1-del [Name]+ --- Delete file name in currant directory.\n\t2-del [Path\\Name]+ ---- Delete file name in path directory.\n\t3-del [Path\\Directory]+ ----- Delete all files in path directory\n");
                        break;
                    case "copy":
                        Console.WriteLine($"\n{Command} -- This Command Used to copy Files.\n\t1-copy [path\\Name File or directory] --- To copy file(s) from path to current directory\n\t2-copy [path1\\Name File1 or directory] [path2] ---- To copy file(s) from path to path2 directory");
                        break;
                    case "rename":
                        Console.WriteLine($"\n{Command} -- This Command Used to rename Files or Directories.\n\t1-rename [path\\Old name]  [New name] --- To rename file or Directory from path.\n");
                        break;
                    case "import":
                        Console.WriteLine($"\n{Command} -- This Command Used to import Files from Physical Disk to Virtual Disk.\n\t1-import [path\\name]  --- To import file from path Physical Disk to Currant Virtual Disk.\n\t2-import [path\\name] [path] ---- To import file from path Physical Disk to path Virtual Disk.\n\t3-import [path\\name] [path\\name] ----- To import file from path Physical Disk to path Virtual Disk by name.");
                        break;
                    case "export":
                        Console.WriteLine($"\n{Command} -- This Command Used to export Files from Virtual Disk to Physical Disk.\n\t1-export [path\\name]  --- To export file from path Virtual Disk to Currant Physical Disk.\n\t2-export [path\\name] [path] ---- To export file from path Virtual Disk to path Physical Disk.\n\t3-export [path\\name] [path\\name] ----- To import file from path Virtual Disk to path Physical Disk by name.");
                        break;
                    default:
                        Console.WriteLine($"Error: {Command} => -- This command id not Supported by the help utility.");
                        return;

                }
            }
        }
    }
}