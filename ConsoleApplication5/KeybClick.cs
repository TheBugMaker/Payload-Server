using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleApplication5
{
    static class KeybClick
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

        private static bool shift = false;
        private static bool ctrl = false;
        private static bool alt = false;

       
        public static void type(byte[] b)
        {
            for (int i = 2; i < b.Length && b[i]!=0 ;i++ )
            {
                Console.WriteLine(b[i]);
                switch (b[i])
                {

                    case 16: 
                        Console.WriteLine(shift);
                        if (shift)
                        {
                            keybd_event(b[i], 0, KEYEVENTF_KEYUP, 0);
                            shift = false;
                        }
                        else {
                            keybd_event(b[i], 0, KEYEVENTF_EXTENDEDKEY, 0);
                            shift=true;
                        }
                        Console.WriteLine(b[i]);
                        Console.WriteLine(shift);
                        break;
                 
                    case 17: if (ctrl)
                        {
                            keybd_event(b[i], 0, KEYEVENTF_KEYUP, 0);
                            ctrl = false; 
                        }
                        else
                        {
                            keybd_event(b[i], 0, KEYEVENTF_EXTENDEDKEY, 0);
                            ctrl = true;
                        }
                        break;
                    case 18: if (alt)
                        {
                            keybd_event(b[i], 0, KEYEVENTF_KEYUP, 0);
                            alt = false;
                        }
                        else
                        {
                            keybd_event(b[i], 0, KEYEVENTF_EXTENDEDKEY, 0);
                            alt = true;
                        }
                        break;
                   
                        

                    default:
                        
                        keybd_event(b[i], 0, KEYEVENTF_EXTENDEDKEY, 0);
                        keybd_event(b[i], 0,KEYEVENTF_KEYUP, 0);
                        break;
                }
            }

        }

    }
}
