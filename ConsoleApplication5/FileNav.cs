using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles; 
namespace ConsoleApplication5
{
    static class FileNav
    {


        static String nav_path = Directory.GetCurrentDirectory();
        public static String get_nav_path()
        {
            return nav_path;
        }


         public static String get_drives(int c,String path) // 2 gets all  ;  1 gets only files
        {   
            StringBuilder str = new StringBuilder();
            if (c>=2){
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                str.Append(d.Name+"\n");
            }
            
            str.Append("<\n");
            
            
                str.Append(path+ "\n<\n");
            }

            Console.WriteLine("Dir   : >" + path); 
            if (Directory.Exists(path))
            {
                Console.WriteLine("accepted"); 
                nav_path = path;
                if (c >= 1)
                {
                    DirectoryInfo fi = new DirectoryInfo(path);
                    if (fi.Attributes.HasFlag(FileAttributes.ReparsePoint)) {
                        path = fi.GetSymbolicLinkTarget(); 
                    }

                    try
                    {
                        String[] dir = Directory.GetDirectories(path);
                        if (dir.Length > 0)
                            foreach (String d in dir)
                            {
                                String[] temp = d.Split(Path.DirectorySeparatorChar);
                                str.Append(temp[temp.Length - 1] + "\n");
                            }

                        str.Append("<\n");

                        dir = Directory.GetFiles(@path);
                        if (dir.Length > 0)
                            foreach (String d in dir)
                            {
                                String[] temp = d.Split(Path.DirectorySeparatorChar);
                                str.Append(temp[temp.Length - 1] + "\n");
                            }
                    }
                    catch (UnauthorizedAccessException ex)
                    {

                        str.Append("PERMISSION DENIED"); str.Append("\n");
                    }
                    catch {
                        str.Append("an error has occured \n"); 
                    }
                }
                return str.ToString();
            }
            return "Nothing Found"; 
        }
         public static String dynamic_nav(String dir)
         {
             Console.WriteLine(dir);
             nav_path.TrimEnd(Path.DirectorySeparatorChar);
             if (dir == "..")
             {
                 nav_path = (Directory.GetParent(nav_path).Exists) ? Directory.GetParent(nav_path).FullName : nav_path;
                 return get_drives(1, nav_path);
             }
             else
             {
                 if (Directory.Exists(nav_path + Path.DirectorySeparatorChar + dir))
                 {
                     nav_path = nav_path + Path.DirectorySeparatorChar + dir;
                     return get_drives(1, nav_path);
                 }
                 else if (Directory.Exists(dir))
                 {
                     nav_path = dir;
                     return get_drives(1, nav_path);
                 }
             }
             return "Nothing found";
         }

         public static String Search(String path,String patern) {
             StringBuilder str = new StringBuilder();
             try
             {
                 
                 String []dir = Directory.GetFiles(path, patern, SearchOption.AllDirectories);
                 if (dir.Length > 0)
                     foreach (String d in dir)
                     {
                      
                         str.Append(d.Remove(d.IndexOf(path),path.Length) +"\n");
                     }
             }
             catch (UnauthorizedAccessException ex)
             {

                 str.Append("PERMISSION DENIED \n");
             }
             catch
             {
                 str.Append("an error has occured \n");
             }
             if (str.Length == 0) str.Append("nothing found \n");
             Console.WriteLine("Ready to return "+str.ToString() );
             return str.ToString(); 
         
         }

         public static String Search(String patern) {
             StringBuilder str = new StringBuilder(); 
             try
             {
                 String[] dir = Directory.GetDirectories(nav_path,patern , SearchOption.AllDirectories);
                 if (dir.Length > 0)
                     foreach (String d in dir)
                     {
                         String[] temp = d.Split(Path.DirectorySeparatorChar);
                         str.Append(temp[temp.Length - 1] + "\n");
                     }

                 str.Append("<\n");

                 dir = Directory.GetFiles(nav_path, patern, SearchOption.AllDirectories);
                 if (dir.Length > 0)
                     foreach (String d in dir)
                     {
                         String[] temp = d.Split(Path.DirectorySeparatorChar);
                         str.Append(temp[temp.Length - 1] + "\n");
                     }
             }
             catch (UnauthorizedAccessException ex)
             {

                 str.Append("PERMISSION DENIED"); str.Append("\n");
             }
             catch
             {
                 str.Append("an error has occured \n");
             }

             return str.ToString(); 
         }

         private const int FILE_SHARE_READ = 1;
         private const int FILE_SHARE_WRITE = 2;

         private const int CREATION_DISPOSITION_OPEN_EXISTING = 3;

         private const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

         // http://msdn.microsoft.com/en-us/library/aa364962%28VS.85%29.aspx
         [DllImport("kernel32.dll", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
         public static extern int GetFinalPathNameByHandle(IntPtr handle, [In, Out] StringBuilder path, int bufLen, int flags);

         // http://msdn.microsoft.com/en-us/library/aa363858(VS.85).aspx
         [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
         public static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
         IntPtr SecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

         public static string GetSymbolicLinkTarget(this FileSystemInfo symlink)
         {
             SafeFileHandle fileHandle = CreateFile(symlink.FullName, 0, 2, System.IntPtr.Zero, CREATION_DISPOSITION_OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, System.IntPtr.Zero);
             if (fileHandle.IsInvalid)
                 return "\0"; 

             StringBuilder path = new StringBuilder(512);
             int size = GetFinalPathNameByHandle(fileHandle.DangerousGetHandle(), path, path.Capacity, 0);
             if (size < 0)
                 return "\0"; 
             // The remarks section of GetFinalPathNameByHandle mentions the return being prefixed with "\\?\"
             // More information about "\\?\" here -> http://msdn.microsoft.com/en-us/library/aa365247(v=VS.85).aspx
             if (path[0] == '\\' && path[1] == '\\' && path[2] == '?' && path[3] == '\\')
                 return path.ToString().Substring(4);
             else
                 return path.ToString();
         }

    }
}
