using System.Diagnostics;
using System.Drawing;

namespace Ghoul.Ui
{
    public interface IIconProvider
    {
        Icon MainIcon();
        Icon WaitIcon();
    }

    // ReSharper disable once UnusedMember.Global
    public class IconProvider : IIconProvider
    {
        public Icon MainIcon()
        {
            return Debugger.IsAttached
                ? Resources.minecraft_ghast
                : Resources.main_icon;
        }

        public Icon WaitIcon()
        {
            return Resources.hourglass;
        }
    }
}