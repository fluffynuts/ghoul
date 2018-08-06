using System;

namespace Ghoul.Native
{
    public static partial class Win32Api
    {
        /// <summary>
        /// Provides SB_* constants
        /// </summary>
        public enum ScrollbarOrientation
        {
            Horizontal = 0x0,
            Vertical = 0x1,
            Control = 0x2,
            Both = 0x3
        }

        /// <summary>
        /// Provides SWP_* flags
        /// </summary>
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

        /// <summary>
        /// Provides the WM_* flags
        /// </summary>
        [Flags]
        public enum WindowMessages
        {
            Paint = 0x00F,
            EraseBackground = 0x0014,
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            MouseMove = 0x0200,
            LeftButtonDown = 0x0201,
            LeftButtonUp = 0x0202,
            Notify = 0x4E
        }

        /// <summary>
        /// Provides the NM_* flags
        /// </summary>
        [Flags]
        public enum Notifications : uint
        {
            Click = 0xFFFFFFFE,
            DoubleClick = 0xFFFFFFFD,
            RightClick = 0xFFFFFFFB,
            RightDoubleClick = 0xFFFFFFFA
        }

        /// <summary>
        /// Provides the VK_ lookup values
        /// </summary>
        public enum Keys
        {
            Space = 0x20,
            Return = 0x0D
        }

        /// <summary>
        /// how to show a window, using ShowWindow
        /// </summary>
        public enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            Maximize = 3,
            ShowNormalNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActivate = 7,
            ShowNoActivate = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimized = 11
        }
    }
}