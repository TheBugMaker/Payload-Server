using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading; 

namespace ConsoleApplication5
{
    public static class Cnx
    {
        static Stream stm ;
       // static bool available = true;
        private static StringBuilder sb = new StringBuilder();
        private static bool timer = false ; 

        private static Object lock1 = new Object();  
        public  static void ini(Stream s) {
            stm = s; 
        }

        static void askPermission() {
            lock (lock1){
                
            
            }
        }
        public static bool sendString(String st) {
            
            UTF8Encoding asen = new UTF8Encoding();
            byte[] b = asen.GetBytes(st);
            byte[] leng = BitConverter.GetBytes(b.Length);
            try
            {
                lock (lock1)
                {
                    
                    stm.Write(leng, 0,4); 
                    stm.Write(b, 0, b.Length);
                    return true;
                    
                }
               
            }
            catch {
               // Console.WriteLine("   errrorr "); 
                return false ; 
            }

        }

        public static bool sendArray(byte[] b, String code) {
            try
            {
                UTF8Encoding asen = new UTF8Encoding();
                byte[] leng = BitConverter.GetBytes(b.Length + 2);
                byte[] msg = new byte[b.Length + 2];
                asen.GetBytes(code).CopyTo(msg, 0);
                b.CopyTo(msg, 2);
                lock (lock1)
                {
                    Console.WriteLine(msg.Length);
                    stm.Write(leng, 0, 4);
                    stm.Write(msg, 0, msg.Length);

                    return true;
                }
            }
            catch { return false;  }
          
        }

        public  static void sendDelayed(String msg) {
            sb.Append(msg); 
            if (!timer) {
                Task.Factory.StartNew(() => doSend() );
                timer = true; 
            }
        }

        private async static void doSend() {
            await Task.Delay(1500);
            timer = false;
            String st = sb.ToString();
            sb.Clear();
            if (st.Length > 0) {
                sendString("l1"+st);
            
            }

            
        }

        // RECIVING STUFF  !

        public static byte[] GetMessage(out String code) {
            byte[] b1 = new byte[4];
            byte[] b = new byte[10000];
            int length;
            int k = 0;
            int k1 = stm.Read(b1, 0,4);
            length = BitConverter.ToInt32(b1, 0);
            using (MemoryStream m = new MemoryStream())
            {
                while (length > 0)
                {
                    int size = (length < b.Length) ? length : b.Length;

                    k = stm.Read(b,0, size);
                    m.Write(b, 0, k);
                    length -= k;

                }


                b = m.ToArray();
            }
            UTF8Encoding asen = new UTF8Encoding();
            if (b.Length >= 2)
            {
                code = asen.GetString(new byte[] { b[0], b[1] });
            }
            else {
                code = "xx";
                return null; 
            }
             
            return b.Skip(2).ToArray<byte>();
        }



    }
}
