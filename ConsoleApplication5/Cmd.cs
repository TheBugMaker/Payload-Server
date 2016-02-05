using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics ; 

namespace ConsoleApplication5
{
    public static class Cmd
    {
        static Cmd() { 
        
        }

        private static Process p;
        private static bool working = false;
        private static bool isSet = false; 

        public static void set (){
            p = new Process(); 
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            // sending data 
            p.OutputDataReceived += (s, e) =>
            {
                
                Cnx.sendString("00" + e.Data);
                Console.WriteLine(" data  :"+e.Data.Length); 
            };
            p.ErrorDataReceived += (s, e) =>
            {  
                Cnx.sendString("00"+e.Data);

            };

            isSet = true; 
            
        }

        public static void start()
        {

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            working = true;
        }
               
     
           
        
            public static void comand(String st){
                if (!isSet) set(); 
                if (!working) start(); 
                 p.StandardInput.WriteLine(st);

            }

            public static void stop() {
                if (!isSet) set();
                if (working)
                {
                    p.Kill();
                    p.CancelOutputRead();
                    p.CancelErrorRead();
                    working = false; 
                } 
            }

            public static  bool isworking() {
                return working; 
            }

            public static String DoCommand(String cnd) {
                String output = ""; 
                Process p = new Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = cnd;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;

                p.Start();
               
                output = p.StandardOutput.ReadToEnd();  // output of cmd
                output = (output.Length == 0) ? " " : output;
                p.WaitForExit();
                return output; 
            } 
            
    }
}
