using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable FieldCanBeMadeReadOnly.Global

#pragma warning disable 169

// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global

namespace Ghoul.Native
{
    public static partial class Win32Api
    {
        [DllImport("oleacc.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetProcessHandleFromHwnd(IntPtr hwnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetProcessId(IntPtr handle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowRect(IntPtr handle, out Rect rect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetScrollPos(IntPtr hwnd, ScrollbarOrientation orientation);

        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(
            "user32.dll",
            EntryPoint = "GetWindowText",
            ExactSpelling = false,
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport(
            "user32.dll",
            EntryPoint = "EnumDesktopWindows",
            ExactSpelling = false,
            CharSet = CharSet.Auto,
            SetLastError = true)]
        public static extern bool EnumDesktopWindows(
            IntPtr hDesktop,
            EnumDelegate lpEnumCallbackFunction,
            IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(
            IntPtr handle,
            IntPtr insertAfter,
            int x,
            int y,
            int width,
            int height,
            SetWindowPosFlags flags);


        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int GetDeviceCaps(IntPtr hdc, DeviceCapabilities deviceCapabilities);

        public enum DeviceCapabilities
        {
            VerticalResolution = 10,
            DesktopVerticalResolution = 117
        }

        // A callback to a Win32 window procedure (wndproc):
        // Parameters:
        //   hwnd - The handle of the window receiving a message.
        //   msg - The message
        //   wParam - The message's parameters (part 1).
        //   lParam - The message's parameters (part 2).
        //  Returns an integer as described for the given message in MSDN.
        public delegate int WndProc(IntPtr hwnd, uint msg, uint wParam, int lParam);

        [DllImport("coredll.dll")]
        public static extern IntPtr SetWindowLong(
            IntPtr hwnd,
            int nIndex,
            IntPtr dwNewLong);

        public const int GWL_WNDPROC = -4;

        [DllImport("coredll.dll")]
        public static extern int CallWindowProc(
            IntPtr lpPrevWndFunc,
            IntPtr hwnd,
            uint msg,
            uint wParam,
            int lParam);

        [DllImport("coredll.dll")]
        public static extern int DefWindowProc(
            IntPtr hwnd,
            uint msg,
            uint wParam,
            int lParam);


        [DllImport("coredll.dll")]
        public static extern uint GetMessagePos();

        [DllImport("coredll.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

        [DllImport("coredll.dll")]
        public static extern bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

        public struct PAINTSTRUCT
        {
            private IntPtr hdc;
            public bool fErase;
            public Rectangle rcPaint;
            public bool fRestore;
            public bool fIncUpdate;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [DllImport("coredll.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll.dll")]
        public static extern bool ReleaseDC(IntPtr hwnd, IntPtr hdc);

        // Helper function to convert a Windows lParam into a Point.
        //   lParam - The parameter to convert.
        // Returns a Point where X is the low 16 bits and Y is the
        // high 16 bits of the value passed in.
        public static Point LParamToPoint(int lParam)
        {
            uint ulParam = (uint) lParam;
            return new Point(
                (int) (ulParam & 0x0000ffff),
                (int) ((ulParam & 0xffff0000) >> 16));
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(
            IntPtr hwnd, ref WindowPlacement windowPlacement);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(
            IntPtr hwnd, ref WindowPlacement windowPlacment);

        public static WindowPlacement GetWindowPlacementByHandle(IntPtr hwnd)
        {
            var result = new WindowPlacement();
            result.Length = Marshal.SizeOf(result);
            GetWindowPlacement(hwnd, ref result);
            return result;
        }

        public static void SetWindowPlacementByHandle(
            IntPtr hwnd,
            WindowPlacement windowPlacement
        )
        {
            SetWindowPlacement(hwnd, ref windowPlacement);
        }
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
    }
}