namespace Ghoul
{
    public class LayoutConfigurationItem
    {
        public enum ProcessMatchOptions
        {
            DoNotMatch,
            ByFullPath,
            ByExecutableOnly
        }

        public string ProcessPath { get; set;  }
        public string WindowTitle { get; set; }
        public WindowPosition Position { get; set;  }
        public ProcessMatchOptions MatchProcess { get; set;  }
        public bool MatchWindowTitle { get; set; }
        public string WindowTitleExpression { get; set; }  // TODO: validate the expression can compile

        public LayoutConfigurationItem(ProcessWindow processWindow)
        {
            ProcessPath = processWindow.Process.MainModule.FileName;
            WindowTitle = processWindow.WindowTitle;
            Position = processWindow.Position;
        }
    }
}