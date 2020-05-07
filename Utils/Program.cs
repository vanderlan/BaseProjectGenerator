using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectBaseName = "VBase Project";
            var newProjectName = "Samsung Cloud";

            string[] actualNames = { 
                projectBaseName.Replace(" ", ""),
                projectBaseName.Replace(" ", "").ToLowerInvariant(),
                projectBaseName
            };
            
            string[] newNames = {
                newProjectName.Replace(" ", ""), 
                newProjectName.Replace(" ", "").ToLowerInvariant(), 
                newProjectName
            };

            string rootDir = @"C:\Users\VanderlanGomes\source\repos\back-end-baseproject";
            
            string newDir = @"C:\Projects\"+ newNames[0];

            DirectoryCopy(rootDir, newDir, true);

            for (int i = 0; i < actualNames.Length; i++)
            {
                RenameFolder(newDir, actualNames[i], newNames[i]);
                
                Task.Delay(100);

                AllFilesRename(newDir, actualNames[i], newNames[i]);
            }

            Console.WriteLine("Completed");
        }

        private static void RenameFolder(string dir, string oldName, string newName)
        {
            var rootDir = new DirectoryInfo(dir);

            RenameFile(Directory.EnumerateFiles(dir), oldName, newName);

            foreach (var childrensDir in rootDir.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                RenameFolder(childrensDir.Parent.FullName + @"\" + childrensDir.Name, oldName, newName);

                string strFoldername = childrensDir.Name;

                if (strFoldername.Contains(oldName))
                {
                    strFoldername = strFoldername.Replace(oldName, newName);
                    string strFolderRoot = childrensDir.Parent.FullName + "\\" + strFoldername;

                    childrensDir.MoveTo(strFolderRoot);

                    Console.WriteLine("{0} renamed to {1}", strFoldername, strFolderRoot);
                }

                var files = Directory.EnumerateFiles(childrensDir.FullName);

                RenameFile(files, oldName, newName);
            }
        }

        private static void RenameFile(IEnumerable<string> files, string oldProjectName, string newProjectname)
        {
            foreach (string file in files)
            {
                var onlyName = Path.GetFileName(file);
                var rootDir = Path.GetFullPath(file).Replace(onlyName, string.Empty);

                if (onlyName.Contains(oldProjectName))
                {
                    var newFullPath = rootDir + onlyName.Replace(oldProjectName, newProjectname);

                    File.Move(file, newFullPath);
                }
            }
        }

        static void AllFilesRename(string sDir, string oldProjectName, string newProjectname)
        {
            try
            {
                foreach (string file in Directory.GetFiles(sDir))
                {
                    Console.WriteLine(file);
                    ReplaceContent(file, oldProjectName, newProjectname);
                }

                foreach (string dir in Directory.GetDirectories(sDir))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        Console.WriteLine(file);
                        ReplaceContent(file, oldProjectName, newProjectname);
                    }

                    AllFilesRename(dir, oldProjectName, newProjectname);
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private static void ReplaceContent(string file, string oldProjectName, string newProjectname)
        {
            if (file.Contains(".git") || file.Contains(".vs"))
                return;

            var fileContents = File.ReadAllText(file);

            if (fileContents.Contains(oldProjectName))
            {
                fileContents = fileContents.Replace(oldProjectName, newProjectname);

                File.WriteAllText(file, fileContents);
                Console.WriteLine("{0} - Contentent replaced", file);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            string[] ignoreDirs = { ".vs", "bin", "Debug", "obj", ".git", "TestResults" };

            var dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();

            foreach (var file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);

                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    if(!ignoreDirs.Contains(subdir.Name))
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);

                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
        }
    }
}
