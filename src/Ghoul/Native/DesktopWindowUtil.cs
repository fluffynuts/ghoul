using System;
using System.Collections.Generic;
using System.Linq;

namespace Ghoul.Native
{
    public interface IDesktopWindowUtil
    {
        ProcessWindow[] ListWindows();
    }

    public class DesktopWindowUtil
        : IDesktopWindowUtil
    {
        public ProcessWindow[] ListWindows()
        {
            var collection = new List<ProcessWindow>();

            bool Filter(IntPtr hWnd, int lParam)
            {
                if (Win32Api.IsWindowVisible(hWnd))
                {
                    collection.Add(new ProcessWindow(hWnd));
                }

                return true;
            }

            Win32Api.EnumDesktopWindows(IntPtr.Zero, Filter, IntPtr.Zero);
            return collection.Where(o => !string.IsNullOrWhiteSpace(o.WindowTitle)).ToArray();
        }
    }
}