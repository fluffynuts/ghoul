using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ghoul
{
    public static class Win32Api
    {
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum ScrollbarOrientation
        {
            SB_HORZ = 0x0,
            SB_VERT = 0x1,
            SB_CTL = 0x2,
            SB_BOTH = 0x3
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
            ExactSpelling = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("oleacc.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetProcessHandleFromHwnd(IntPtr hwnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetProcessId(IntPtr handle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowRect(IntPtr handle, out RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetScrollPos(IntPtr hwnd, ScrollbarOrientation orientation);

    }
}