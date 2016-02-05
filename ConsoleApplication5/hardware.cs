using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace ConsoleApplication5
{
    static class hardware
    {
       public static string processorId() {
            string id ="000000"; 
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ("select ProcessorId from win32_processor ");
            foreach (ManagementObject mo in searcher.Get()) {

            id  =  (string)mo["ProcessorId"];

            }

            return id; 
        }

    }
}
