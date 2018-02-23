using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ghoul
{
    public class DesktopWindowUtil
    {
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction,
            IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr handle, IntPtr insertAfter, int x, int y, int width, int height,
            uint flags);

        public static class WindowInsertionSpecialValues
        {
            public const int Bottom = 1;
            public const int NoTopMost = -2;
        }

        public void MoveWindow(IntPtr handle, int x, int y, int width, int height)
        {

        }

        public List<ProcessWindow> ListWindows()
        {
            var collection = new List<ProcessWindow>();

            bool Filter(IntPtr hWnd, int lParam)
            {
                if (IsWindowVisible(hWnd))
                {
                    collection.Add(new ProcessWindow(hWnd));
                }

                return true;
            }

            EnumDesktopWindows(IntPtr.Zero, Filter, IntPtr.Zero);
            return collection;
        }
    }
}