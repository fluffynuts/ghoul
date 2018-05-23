using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NCrash;
using NCrash.WinForms;

namespace Discovery
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetupCrashHandler();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void SetupCrashHandler()
        {
            var ui = new NormalWinFormsUserInterface();
            var settings = new DefaultSettings()
            {
                HandleProcessCorruptedStateExceptions = true,
                UserInterface = ui
            };
            var reporter = new ErrorReporter(settings);
            
            AppDomain.CurrentDomain.UnhandledException += reporter.UnhandledException;
            TaskScheduler.UnobservedTaskException += reporter.UnobservedTaskException;
            Application.ThreadException += reporter.ThreadException;
        }
    }
}