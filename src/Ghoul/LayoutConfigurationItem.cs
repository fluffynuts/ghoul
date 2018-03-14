// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Ghoul
{
    public class LayoutConfigurationItem
    {
        public string ProcessPath { get; }
        public string WindowTitle { get; }
        public WindowPosition Position { get; }

        // TODO: allow user to specify more rules (like a regex for window titles?)
        // TODO: use this configuration item instead of raw dictionary access

        public LayoutConfigurationItem(ProcessWindow processWindow)
        {
            ProcessPath = processWindow.Process.MainModule.FileName;
            WindowTitle = processWindow.WindowTitle;
            Position = processWindow.Position;
        }
    }
}