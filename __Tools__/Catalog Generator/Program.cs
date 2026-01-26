using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;


namespace Catalog_Generator
{
    class Program
    {
        public const string CatelogName = "__Catalog__";

        public static DirectoryInfo root = null;

        static void Main(string[] args)
        {
            Console.WriteLine("输入操作：(d)删除所有目录/(g)生成目录");

            string op = Console.ReadLine();

            Console.WriteLine("请输入笔记根目录：");
            string path = Console.ReadLine();
            if (path == "")
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine("Path Not Exist");
                return;
            }
            root = new DirectoryInfo(path);


            if (op == "d")
            {
                DeleteAllCatalog(root);
            }
            else if(op == "g")
            {
                GenCatalogAtDir(root, true);
            }
        }

        static void DeleteAllCatalog(DirectoryInfo dir)
        {
            foreach(var file in dir.GetFiles())
            {
                if(file.Name.Contains(CatelogName))
                {
                    File.Delete(file.FullName);
                    Console.WriteLine("已删除目录：" + file.FullName);
                }
            }

            foreach(var child in dir.GetDirectories())
            {
                DeleteAllCatalog(child);
            }
        }

        static void GenCatalogAtDir(DirectoryInfo dir, bool isRoot = false)
        {
            //目录  
            string mapDir = MapToCatalogPath(dir); 
            if(!Directory.Exists(mapDir))
            {
                Directory.CreateDirectory(mapDir);
            }

            //文件名  
            string catalogName;
            if (isRoot == false)
                catalogName = CatelogName + dir.Name + ".md";
            else
                catalogName = $"{CatelogName}root.md";
            
            string catalogFullName = mapDir + "\\" + catalogName;


            //开始写入  
            StringWriter writer = new StringWriter();

            //TITLE  
            writer.WriteLine("# 目录  \n\n");


            // LINK: RETURN  
            if( isRoot == false)
            {
                if(dir.Parent.FullName == root.FullName)
                {
                    writer.WriteLine(ReplaceSeperator($"[👈【返回】](/{CatelogName}/{CatelogName}root)  \n\n"));
                }
                else
                {
                    writer.WriteLine(ReplaceSeperator("[👈【返回】](" + ReplaceSeperator(GetRelativePath(MapToCatalogPath(dir.Parent)) + "\\" + CatelogName + dir.Parent.Name) + ")  \n\n"));
                }
            }



            // ITEM:  CHILD DIRS  
            foreach (var childDir in dir.GetDirectories())
            {
                if (childDir.Name.Contains(CatelogName)) continue;
                if (childDir.Name.Contains("Images")) continue;
                if ((childDir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;

                string dirName = childDir.Name; dirName = dirName.Replace(" ", " ");
                string catalogDir = MapToCatalogPath(childDir); 
                string relativeCatalogDir = GetRelativePath(catalogDir); 
                string relaPath = ReplaceSeperator(relativeCatalogDir + "\\" + CatelogName + dirName + "");

                writer.WriteLine("[📁" + dirName + "](" + relaPath + ")  \n");

                //递归遍历子目录
                GenCatalogAtDir(childDir);
            }


            // ITEM:  FILES  
            foreach (var f in dir.GetFiles())
            {
                if (f.Name.Contains(CatelogName)) continue;
                if (f.Name.Contains("index.md")) continue;
                if ((f.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;

                string fNameNoExtension = f.Name.Substring(0, (f.Name.Length - f.Extension.Length));

                switch (f.Extension)
                {
                    case ".md":
                        {
                            string name = fNameNoExtension; name = name.Replace(" ", " ");
                            string fullNameNoExtension = f.FullName.Substring(0, (f.FullName.Length - f.Extension.Length));//no ".md" extension in jekyll  
                            string relaPath = GetRelativePath(fullNameNoExtension);  
                            writer.WriteLine("[📜" + name + "](" + ReplaceSeperator(relaPath) + ")  \n");
                        }
                        break;
                    case ".txt":
                        {
                            string name = fNameNoExtension; name = name.Replace(" ", " ");
                            string fullNameNoExtension = f.FullName.Substring(0, (f.FullName.Length - f.Extension.Length));//no ".md" extension in jekyll  
                            string relaPath = GetRelativePath(fullNameNoExtension);
                            writer.WriteLine("[📜" + name + "](" + ReplaceSeperator(relaPath) + ".txt)  \n");
                        }
                        break;
                    default:
                        break;
                }
            }


            writer.WriteLine("\n\n\n\n\n\n> " + System.DateTime.Now.ToString());
            writer.Flush();

            File.WriteAllText(catalogFullName, writer.ToString());
            writer.Dispose();

            Console.WriteLine("建立目录:" + catalogFullName);
        }


        static string MapToCatalogPath(DirectoryInfo dir)
        {
            string path = dir.FullName;
            string relaPath = GetRelativePath(path);

            if (dir.FullName != root.FullName)
            {
                return root.FullName + "\\" + CatelogName + relaPath; ;
            }
            else
            {
                return root.FullName + "\\" + CatelogName;
            }
        }

        static string GetRelativePath(string path)
        {
            string rootPath = root.FullName;

            if (path == rootPath)
                return "\\";
            else
                return "\\" + path.Substring(rootPath.Length + 1);
        }

        static string ReplaceSeperator(string path)
        {
            return path.Replace('\\', '/');
        }
    }
}
