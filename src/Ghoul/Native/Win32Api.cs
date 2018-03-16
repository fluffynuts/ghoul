using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
#pragma warning disable 169

// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global

namespace Ghoul.Native
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

        [DllImport("oleacc.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetProcessHandleFromHwnd(IntPtr hwnd);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetProcessId(IntPtr handle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowRect(IntPtr handle, out RECT rect);

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

        [Flags]
        public enum SetWindowPosFlags
        {
            Async = 0x400,
            DeferRaise = 0x200,
            DrawFrame = 0x0020,
            HideWindow = 0x0080,
            NoActivate = 0x0010,
            NoCopyBits = 0x0100,
            NoMove = 0x0002,
            KeepOwnerZIndex = 0x0200,
            NoRedraw = 0x0008,
            NoSendChanging = 0x0400,
            NoSize = 0x0001,
            KeepZIndex = 0x0004,
            ShowWindow = 0x0040
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int GetDeviceCaps(IntPtr hdc, DeviceCapabilities deviceCapabilities);

        public enum DeviceCapabilities
        {
            VerticalResolution = 10,
            DesktopVerticalResolution = 117
        }

        public static class WindowInsertionSpecialValues
        {
            public const int Bottom = 1;
            public const int NoTopMost = -2;
        }

        // imported from https://msdn.microsoft.com/en-us/library/ms229683.aspx
        //  to support WndProcHooker
        public struct TRIVERTEX
        {
            public int x;
            public int y;
            public ushort Red;
            public ushort Green;
            public ushort Blue;
            public ushort Alpha;

            public TRIVERTEX(int x, int y, Color color)
                : this(x, y, color.R, color.G, color.B, color.A)
            {
            }

            public TRIVERTEX(
                int x,
                int y,
                ushort red,
                ushort green,
                ushort blue,
                ushort alpha)
            {
                this.x = x;
                this.y = y;
                Red = (ushort) (red << 8);
                Green = (ushort) (green << 8);
                Blue = (ushort) (blue << 8);
                Alpha = (ushort) (alpha << 8);
            }
        }

        public struct GRADIENT_RECT
        {
            public uint UpperLeft;
            public uint LowerRight;

            public GRADIENT_RECT(uint ul, uint lr)
            {
                UpperLeft = ul;
                LowerRight = lr;
            }
        }

        public struct GRADIENT_TRIANGLE
        {
            public uint Vertex1;
            public uint Vertex2;
            public uint Vertex3;

            public GRADIENT_TRIANGLE(uint v1, uint v2, uint v3)
            {
                Vertex1 = v1;
                Vertex2 = v2;
                Vertex3 = v3;
            }
        }

        // WM_NOTIFY notification message header.
        [StructLayout(LayoutKind.Sequential)]
        public class NMHDR
        {
            private IntPtr hwndFrom;
            public uint idFrom;
            public uint code;
        }

        //[System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        public struct TVITEM
        {
            public int mask;
            private IntPtr hItem;
            public int state;
            public int stateMask;
            private IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            private IntPtr lParam;
        }

        // Native representation of a point.
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public struct TVHITTESTINFO
        {
            public POINT pt;
            public uint flags;
            private IntPtr hItem;
        }

        // A callback to a Win32 window procedure (wndproc):
        // Parameters:
        //   hwnd - The handle of the window receiving a message.
        //   msg - The message
        //   wParam - The message's parameters (part 1).
        //   lParam - The message's parameters (part 2).
        //  Returns an integer as described for the given message in MSDN.
        public delegate int WndProc(IntPtr hwnd, uint msg, uint wParam, int lParam);

        [DllImport("coredll.dll", SetLastError = true, EntryPoint = "GradientFill")]
        public extern static bool GradientFill(
            IntPtr hdc,
            TRIVERTEX[] pVertex,
            uint dwNumVertex,
            GRADIENT_RECT[] pMesh,
            uint dwNumMesh,
            uint dwMode);

        public const int GRADIENT_FILL_RECT_H = 0x00000000;

        public const int GRADIENT_FILL_RECT_V = 0x00000001;

        // Not supported on Windows CE:
        public const int GRADIENT_FILL_TRIANGLE = 0x00000002;

        [DllImport("coredll.dll")]
        public extern static IntPtr SetWindowLong(
            IntPtr hwnd,
            int nIndex,
            IntPtr dwNewLong);

        public const int GWL_WNDPROC = -4;

        [DllImport("coredll.dll")]
        public extern static int CallWindowProc(
            IntPtr lpPrevWndFunc,
            IntPtr hwnd,
            uint msg,
            uint wParam,
            int lParam);

        [DllImport("coredll.dll")]
        public extern static int DefWindowProc(
            IntPtr hwnd,
            uint msg,
            uint wParam,
            int lParam);

        [DllImport("coredll.dll")]
        public extern static int SendMessage(
            IntPtr hwnd,
            uint msg,
            uint wParam,
            ref TVHITTESTINFO lParam);

        [DllImport("coredll.dll", SetLastError = true)]
        public extern static int SendMessage(
            IntPtr hwnd,
            uint msg,
            uint wParam,
            ref TVITEM lParam);

        [DllImport("coredll.dll")]
        public extern static uint GetMessagePos();

        [DllImport("coredll.dll")]
        public extern static IntPtr BeginPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

        [DllImport("coredll.dll")]
        public extern static bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

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
        public extern static IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll.dll")]
        public extern static bool ReleaseDC(IntPtr hwnd, IntPtr hdc);

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

        public static class WindowMessages
        {
            public const uint WM_PAINT = 0x000F;
            public const uint WM_ERASEBKGND = 0x0014;
            public const uint WM_KEYDOWN = 0x0100;
            public const uint WM_KEYUP = 0x0101;
            public const uint WM_MOUSEMOVE = 0x0200;
            public const uint WM_LBUTTONDOWN = 0x0201;
            public const uint WM_LBUTTONUP = 0x0202;
            public const uint WM_NOTIFY = 0x4E;
        }

        public static class Notifications
        {
            public const uint NM_CLICK = 0xFFFFFFFE;
            public const uint NM_DBLCLK = 0xFFFFFFFD;
            public const uint NM_RCLICK = 0xFFFFFFFB;
            public const uint NM_RDBLCLK = 0xFFFFFFFA;
        }

        public static class Keys
        {
            public const uint VK_SPACE = 0x20;
            public const uint VK_RETURN = 0x0D;
        }

        public class Treeview
        {
            public const uint TV_FIRST = 0x1100;
            public const uint TVM_HITTEST = TV_FIRST + 17;
            public const uint TVHT_NOWHERE = 0x0001;
            public const uint TVHT_ONITEMICON = 0x0002;
            public const uint TVHT_ONITEMLABEL = 0x0004;
            public const uint TVHT_ONITEM = (TVHT_ONITEMICON | TVHT_ONITEMLABEL | TVHT_ONITEMSTATEICON);
            public const uint TVHT_ONITEMINDENT = 0x0008;
            public const uint TVHT_ONITEMBUTTON = 0x0010;
            public const uint TVHT_ONITEMRIGHT = 0x0020;
            public const uint TVHT_ONITEMSTATEICON = 0x0040;
            public const uint TVHT_ABOVE = 0x0100;
            public const uint TVHT_BELOW = 0x0200;
            public const uint TVHT_TORIGHT = 0x0400;
            public const uint TVHT_TOLEFT = 0x0800;
            public const uint TVM_GETITEM = TV_FIRST + 62; //TVM_GETITEMW
            public const uint TVIF_TEXT = 0x0001;
            public const uint TVIF_IMAGE = 0x0002;
            public const uint TVIF_PARAM = 0x0004;
            public const uint TVIF_STATE = 0x0008;
            public const uint TVIF_HANDLE = 0x0010;
            public const uint TVIF_SELECTEDIMAGE = 0x0020;
            public const uint TVIF_CHILDREN = 0x0040;
            public const uint TVIF_DI_SETITEM = 0x1000;
        }
    }
}