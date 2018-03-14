using System;
using System.Diagnostics;
using System.Text;
using static Ghoul.Win32Api;

namespace Ghoul
{
    public class ProcessWindow
    {
        public string ProcessName => Process.MainModule.FileName;
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

        private Process FindProcessFor(
            IntPtr handle)
        {
            var processHandle = GetProcessHandleFromHwnd(handle);
            var processId = GetProcessId(processHandle);
            return Process.GetProcessById(processId);
        }

        private WindowPosition FindPositionOf(
            IntPtr handle)
        {
            GetWindowRect(handle, out var result);
            return new WindowPosition(result);
        }

        private string GetCaptionFor(
            IntPtr handle)
        {
            var builder = new StringBuilder(1024);
            GetWindowText(handle, builder, builder.Capacity);
            return builder.ToString();
        }

        public void MoveTo(
            int left,
            int top,
            int width,
            int height)
        {
            SetWindowPos(
                Handle,
                IntPtr.Zero, 
                left,
                top,
                width,
                height,
                0
            );
        }
    }
}