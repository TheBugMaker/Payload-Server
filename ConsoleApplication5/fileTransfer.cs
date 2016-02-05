using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace ConsoleApplication5
{
     static class fileTransfer
    {
         private static bool work { get; set; }   
         private static List<String> toUpload = new List<String>();
         private static LinkedList<MyFile> toDownload = new LinkedList<MyFile>(); 

         static fileTransfer(){
            work = false ; 
         }

         public static void transfer() {
             if (!work) {
                
                 work = true; 
                 byte[] buffer = new byte[100000] ;
                 

                 while (toUpload.Count() > 0 && work) { 
                     String name ; 
                         lock (toUpload) {
                             name = toUpload[0]; 
                             toUpload.RemoveAt(0);
                         }
                  
                     using (FileStream fsSource = new FileStream(name, FileMode.Open, FileAccess.Read))
                   {
                       String[] names = name.Split(Path.DirectorySeparatorChar); 
                            Cnx.sendString("s0"+names[names.Length - 1]+":"+fsSource.Length.ToString());
                            Console.WriteLine(Path.PathSeparator); 
                       int a = 0; 
                       do
                       {
                          a = fsSource.Read(buffer, 0, buffer.Length);
                          Console.WriteLine("reading " + a);
                          if (a == 0) break; 
                          Cnx.sendArray(buffer.Take(a).ToArray<byte>(),"s1"); // open and send stuff 
                       } while (a > 0);  
                   }

                   Cnx.sendString("s2");
                  
                 }
                 work = false; 
             }            
         }


         public static void addUpload(String a){
            lock(toUpload)
            {
                toUpload.Add(a) ; 
            }
         }
         public  static void stop() {
             work = false; 
         } 


         //
         //     DOWNLOADING STUFF
         //

         /// <summary>
         ///  add a download 
         /// </summary>
         /// <param name="id"></param>
         /// <param name="fs"></param>
         public static void addDownload(String id , FileStream fs)
         {
             lock (toDownload)
             {
                 toDownload.AddLast(new MyFile(id , fs ) ); 
             }
         }

         public static FileStream getHandle(String id) {
             foreach (var item in toDownload)
             {
                 if (item.hasId(id)) return item.getHandle();
                 
             }
             return null; 
         }

         public static void Close(String id) {
             foreach (var item in toDownload)
             {
                 if (item.close(id)) { toDownload.Remove(item); return;  }

             }
         }
    }
}
