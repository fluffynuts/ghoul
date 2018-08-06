using System;
using System.Drawing;
using System.Runtime.InteropServices;
// ReSharper disable UnassignedField.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Ghoul.Native
{
    public static partial class Win32Api
    {
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WindowPlacement
        {
            public int Length;
            public int Flags;
            public ShowWindowCommands ShowCommands;
            public Point MinimumPosition;
            public Point MaximumPosition;
            public Rectangle NormalPosition;
            
            public bool IsMinimized => 
                ShowCommands == ShowWindowCommands.Minimized;

            public bool IsMaximized =>
                ShowCommands == ShowWindowCommands.Maximized;
        }


        public enum ShowWindowCommands
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }
    }
}