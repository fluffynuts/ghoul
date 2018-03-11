﻿using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PeanutButter.INIFile;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;

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
            var trayIcon = new TrayIcon(Resources.Ghoul);
            var config = LoadConfig();
            var sectionNameHelper = new SectionNameHelper(config);
            AddSaveLayoutItem(trayIcon, config, sectionNameHelper);
            var menu = AddRestoreLayoutMenuTo(trayIcon);
            AddRestoreMenusTo(menu, config, trayIcon, sectionNameHelper);
            AddExitMenuItemTo(trayIcon);
            config.Persist();
            trayIcon.Show();

            Application.Run();
        }

        private static void AddExitMenuItemTo(
            TrayIcon trayIcon
        )
        {
            trayIcon.AddMenuSeparator();
            trayIcon.AddMenuItem(
                "Exit",
                () =>
                {
                    trayIcon.Hide();
                    Application.Exit();
                });
        }

        private static void AddRestoreMenusTo(
            MenuItem menu,
            INIFile config,
            TrayIcon trayIcon,
            SectionNameHelper sectionNameHelper
        )
        {
            var restarter = new ApplicationRestarter(config);
            var restorer = new LayoutRestorer(
                config,
                restarter,
                sectionNameHelper);
            sectionNameHelper.ListLayoutNames()
                .ForEach(
                    s =>
                    {
                        trayIcon.AddMenuItem(
                            s,
                            () => restorer.RestoreLayout(s),
                            menu);
                    });
        }


        private static void AddSaveLayoutItem(
            TrayIcon trayIcon,
            INIFile config,
            SectionNameHelper sectionNameHelper)
        {
            var layoutSaver = new LayoutSaver(
                config,
                new UserInput(),
                sectionNameHelper);
            trayIcon.AddMenuItem(
                "Save current layout...",
                () => layoutSaver.SaveCurrentLayout()
            );
        }

        private static MenuItem AddRestoreLayoutMenuTo(TrayIcon trayIcon)
        {
            return trayIcon.AddSubMenu("Restore layout");
        }

        private static INIFile LoadConfig()
        {
            var configLocation = FindConfig();
            return new INIFile(configLocation);
        }

        private static string FindConfig()
        {
            var programPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var programFolder = Path.GetDirectoryName(programPath);
            return Path.Combine(programFolder, "ghoul.ini");
        }
    }
}