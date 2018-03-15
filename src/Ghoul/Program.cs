using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DryIoc;
using Ghoul.AppLogic;
using Ghoul.AppLogic.Events;
using Ghoul.Utils;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
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
            var container = Bootstrapper.Bootstrap(trayIcon);

            AddSaveLayoutItem(container);
            var menu = AddRestoreLayoutMenu(container);
            AddRestoreMenusTo(menu, trayIcon, container);
            AddExitMenuItemTo(trayIcon);
            BindToLayoutAdded(menu, container);
            BindDoubleClickToRestoreLast(container);

            trayIcon.Show();

            Application.Run();
        }

        private static void BindDoubleClickToRestoreLast(IContainer container)
        {
            var trayIcon = container.Resolve<ITrayIcon>();
            var lastLayoutUtil = container.Resolve<ILastLayoutUtility>();
            trayIcon.AddMouseClickHandler(
                MouseClicks.Double,
                MouseButtons.Left,
                () => lastLayoutUtil.RestoreLastLayout()
            );
        }

        private static void StoreLastRestoredLayout(
            string layoutName,
            IINIFile config)
        {
            config.SetValue(Constants.Sections.GENERAL, Constants.Keys.LAST_LAYOUT, layoutName);
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
            ITrayIcon trayIcon,
            IContainer container)
        {
            var restorer = container.Resolve<ILayoutRestorer>();
            var sectionNameHelper = container.Resolve<ISectionNameHelper>();
            sectionNameHelper.ListLayoutNames()
                .ForEach(
                    s =>
                    {
                        trayIcon.AddMenuItem(
                            s,
                            () =>
                            {
                                restorer.RestoreLayout(s);
                                StoreLastRestoredLayout(s, container.Resolve<IINIFile>());
                            },
                            menu);
                    });
        }

        private static void BindToLayoutAdded(MenuItem restoreMenu, IContainer container)
        {
            var trayIcon = container.Resolve<ITrayIcon>();
            var eventAggregator = container.Resolve<IEventAggregator>(); // TODO: replace with IEventAggregator
            eventAggregator
                .GetEvent<LayoutAddedEvent>()
                .Subscribe(
                    newLayout =>
                    {
                        var toRemove = new List<MenuItem>();
                        foreach (MenuItem item in restoreMenu.MenuItems)
                            toRemove.Add(item);
                        toRemove.ForEach(item => item.Parent.MenuItems.Remove(item));
                        AddRestoreMenusTo(restoreMenu, trayIcon, container);
                    });
        }


        private static void AddSaveLayoutItem(
            IContainer container)
        {
            var layoutSaver = container.Resolve<ILayoutSaver>();
            var trayIcon = container.Resolve<ITrayIcon>();
            trayIcon.AddMenuItem(
                "Save current layout...",
                () => layoutSaver.SaveCurrentLayout()
            );
        }

        private static MenuItem AddRestoreLayoutMenu(IContainer container)
        {
            var trayIcon = container.Resolve<ITrayIcon>();
            return trayIcon.AddSubMenu(Constants.Menus.RESTORE);
        }
    }
}