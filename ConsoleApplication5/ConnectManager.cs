using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication5
{
    class ConnectManager
    {
        

        public static void  CheckAlive( TcpClient tc ) {
            
            do {
                if (tc.Connected)
                {   
                    
                    Thread.Sleep(3000);
                    Cnx.sendString("xx"); // test connection 
                    //Console.WriteLine("Server IS UP "); 
                    
                }
                else {
                    Program.ServerUp = false;
                    tc.Close(); 
                    Console.WriteLine("Server IS DOWN Down Down "); 
                    break; 
                }
            }while(true) ;  
        }

    }
}
