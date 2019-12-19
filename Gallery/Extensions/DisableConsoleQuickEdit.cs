using System;
using System.Runtime.InteropServices;

namespace Gallery.Extensions
{
    public class DisableConsoleQuickEdit
    {
        const uint ENABLE_QUICK_EDIT = 0x0040;
        const int STD_INPUT_HANDLE = -10;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// <summary>
        /// Fix the console from freezing the bot due to checking for readinput in the console
        /// </summary>
        public static bool Go()
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
            if (!GetConsoleMode(consoleHandle, out uint consoleMode)) return false;
            consoleMode &= ~ENABLE_QUICK_EDIT;
            if (!SetConsoleMode(consoleHandle, consoleMode)) return false;
            return true;
        }
    }
}
