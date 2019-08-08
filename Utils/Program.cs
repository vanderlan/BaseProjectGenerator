using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            string actualName = "Invest";
            string newName = "VAuthServer";

            RenameFolder(@"c:\Projeto\BaseProject", actualName, newName);

            Task.Delay(100);

            AllFilesRename(@"c:\Projeto\BaseProject", actualName, newName);

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
                    var fullPath = rootDir + onlyName;
                    var newFullPath = rootDir + onlyName.Replace(oldProjectName, newProjectname);

                    File.Move(file, newFullPath);
                }
            }
        }

        static void AllFilesRename(string sDir, string oldProjectName, string newProjectname)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        Console.WriteLine(f);
                        ReplaceContent(f, oldProjectName, newProjectname);
                    }
                    AllFilesRename(d, oldProjectName, newProjectname);
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private static void ReplaceContent(string file, string oldProjectName, string newProjectname)
        {
            var fileContents = File.ReadAllText(file);

            if (fileContents.Contains(oldProjectName))
            {
                fileContents = fileContents.Replace(oldProjectName, newProjectname);

                File.WriteAllText(file, fileContents);

                Console.WriteLine("{0} - Contentent replaced", file);
            }
        }
    }
}
