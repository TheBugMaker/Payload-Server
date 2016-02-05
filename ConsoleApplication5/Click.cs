using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;


namespace ConsoleApplication5
{   
    static class Click
    {   
        private static int u_width = (int)Screen.PrimaryScreen.Bounds.Width;
        private static int u_height = (int)Screen.PrimaryScreen.Bounds.Height;


        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        private const int MOUSEEVENTF_MIDDLEUP = 0x40;
        

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);


        public static void right(float x, float y)
        {
            uint X = (uint)((x * u_width) );
            uint Y = (uint)((y * u_height) );

            Console.WriteLine(x + "  " + y);
            Console.WriteLine(X + "  " + Y);
            SetCursorPos((int)X,(int)Y);
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);

        }
        public static void left(float x, float y)
        {
            uint X = (uint)((x * u_width)  );
            uint Y = (uint)((y * u_height) );
            SetCursorPos((int)X, (int)Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
        public static void leftd(float x, float y)
        {
            uint X = (uint)((x * u_width)  );
            uint Y = (uint)((y * u_height) );
            SetCursorPos((int)X, (int)Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
        public static void middle(float x, float y)
        {
            uint X = (uint)((x * u_width) );
            uint Y = (uint)((y * u_height) );
            SetCursorPos((int)X, (int)Y);
            mouse_event(MOUSEEVENTF_MIDDLEDOWN | MOUSEEVENTF_MIDDLEUP, X, Y, 0, 0);
        }
    }
}
