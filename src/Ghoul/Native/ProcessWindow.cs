using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

namespace Ghoul.Native
{
    public class ProcessWindow
    {
        public string ProcessName => TryGetProcessMainModuleFilename();
        public Process Process { get; }
        public IntPtr Handle { get; }
        public string WindowTitle { get; }
        public WindowPosition Position { get; }

        public ProcessWindow(
            IntPtr windowHandle)
        {
            WindowTitle = GetCaptionFor(windowHandle);
            Process = FindProcessFor(windowHandle);
            Handle = windowHandle;
            Position = FindPositionOf(windowHandle);
        }

        private string TryGetProcessMainModuleFilename()
        {
            try
            {
                return Process.MainModule.FileName;
            }
            catch
            {
                return ""; // safer for any caller
            }
        }

        private Process FindProcessFor(
            IntPtr handle)
        {
            var processHandle = Win32Api.GetProcessHandleFromHwnd(handle);
            var processId = Win32Api.GetProcessId(processHandle);
            return Process.GetProcessById(processId);
        }

        private WindowPosition FindPositionOf(
            IntPtr handle)
        {
            Win32Api.GetWindowRect(handle, out var result);
            return new WindowPosition(result);
        }

        private string GetCaptionFor(
            IntPtr handle)
        {
            var builder = new StringBuilder(1024);
            Win32Api.GetWindowText(handle, builder, builder.Capacity);
            return builder.ToString();
        }

        public void MoveTo(
            int left,
            int top,
            int width,
            int height)
        {
            Win32Api.SetWindowPos(
                Handle,
                IntPtr.Zero,
                left,
                top,
                width,
                height,
                Win32Api.SetWindowPosFlags.KeepZIndex
            );

        }
    }
}