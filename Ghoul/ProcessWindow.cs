using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Ghoul
{
    public class ProcessWindow
    {
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public Process Process { get; }
        public IntPtr Handle { get; }
        public string Caption { get; }
        public WindowPosition Position { get; }

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
            ExactSpelling = false, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);


        [DllImport("oleacc.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr GetProcessHandleFromHwnd(IntPtr hwnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int GetProcessId(IntPtr handle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowRect(IntPtr handle, out RECT rect);

        public ProcessWindow(IntPtr handle)
        {
            Caption = GetCaptionFor(handle);
            Process = FindProcessFor(handle);
            Handle = handle;
            Position = FindPositionOf(handle);
        }

        private Process FindProcessFor(IntPtr handle)
        {
            var processHandle = GetProcessHandleFromHwnd(handle);
            var processId = GetProcessId(processHandle);
            return Process.GetProcessById(processId);
        }

        private WindowPosition FindPositionOf(IntPtr handle)
        {
            RECT result = default(RECT);
            GetWindowRect(handle, out result);
            return new WindowPosition(result);
        }

        private string GetCaptionFor(IntPtr handle)
        {
            var builder = new StringBuilder(1024);
            GetWindowText(handle, builder, builder.Capacity);
            return builder.ToString();
        }
    }

    public class WindowPosition
    {
        public WindowPosition(ProcessWindow.RECT rect)
        {
            Top = rect.Top;
            Left = rect.Left;
            Right = rect.Right;
            Bottom = rect.Bottom;
            Width = rect.Right - rect.Left;
            Height = rect.Bottom - rect.Top;
        }

        public int Left { get; }
        public int Right { get; }
        public int Bottom { get; }
        public int Width { get; }
        public int Height { get; set; }
        public int Top { get; set; }
    }
}