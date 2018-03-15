using System;
using System.Windows.Forms;
using DryIoc;
using Ghoul.Ui;
using Ghoul.Utils;

namespace Ghoul
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var container = Bootstrapper.Bootstrap();
            container.Resolve<IApplicationCoordinator>().Init();

            Application.Run();
        }
    }
}