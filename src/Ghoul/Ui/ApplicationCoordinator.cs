using System.Collections.Generic;
using System.Windows.Forms;
using Ghoul.AppLogic;
using Ghoul.AppLogic.Events;
using Ghoul.Utils;
using PeanutButter.TinyEventAggregator;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;

namespace Ghoul.Ui
{
    public interface IApplicationCoordinator
    {
        void Init();
    }

    // ReSharper disable once UnusedMember.Global
    public class ApplicationCoordinator
        : IApplicationCoordinator
    {
        private readonly ILayoutSaver _layoutSaver;
        private readonly ITrayIcon _trayIcon;
        private readonly ILayoutRestorer _layoutRestorer;
        private readonly ISectionNameHelper _sectionNameHelper;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILastLayoutUtility _lastLayoutUtility;

        public ApplicationCoordinator(
            ILayoutSaver layoutSaver,
            ITrayIcon trayIcon,
            ILayoutRestorer layoutRestorer,
            ISectionNameHelper sectionNameHelper,
            IEventAggregator eventAggregator,
            ILastLayoutUtility lastLayoutUtility
        )
        {
            _layoutSaver = layoutSaver;
            _trayIcon = trayIcon;
            _layoutRestorer = layoutRestorer;
            _sectionNameHelper = sectionNameHelper;
            _eventAggregator = eventAggregator;
            _lastLayoutUtility = lastLayoutUtility;
        }

        public void Init()
        {
            var restoreMenuItem = CreateMenuEntries();
            HandleLayoutAddedEventFor(restoreMenuItem);
            BindDoubleClickToRestoreLast();
            _trayIcon.Show();
        }

        private MenuItem CreateMenuEntries()
        {
            AddSaveLayoutMenuItem();
            var restoreMenuItem = AddRestoreLayoutMenu();
            AddRestoreMenusTo(restoreMenuItem);
            AddExitMenuItem();
            return restoreMenuItem;
        }

        private void AddExitMenuItem(
        )
        {
            _trayIcon.AddMenuSeparator();
            _trayIcon.AddMenuItem(
                "Exit",
                () =>
                {
                    _trayIcon.Hide();
                    Application.Exit();
                });
        }

        private void BindDoubleClickToRestoreLast()
        {
            _trayIcon.AddMouseClickHandler(
                MouseClicks.Double,
                MouseButtons.Left,
                () => _lastLayoutUtility.RestoreLastLayout()
            );
        }


        private void HandleLayoutAddedEventFor(MenuItem restoreMenu)
        {
            _eventAggregator
                .GetEvent<LayoutAddedEvent>()
                .Subscribe(
                    newLayout =>
                    {
                        var toRemove = new List<MenuItem>();
                        foreach (MenuItem item in restoreMenu.MenuItems)
                            toRemove.Add(item);
                        toRemove.ForEach(item => item.Parent.MenuItems.Remove(item));
                        AddRestoreMenusTo(restoreMenu);
                    });
        }

        private void AddSaveLayoutMenuItem()
        {
            _trayIcon.AddMenuItem(
                "Save current layout...",
                () => _layoutSaver.SaveCurrentLayout()
            );
        }

        private MenuItem AddRestoreLayoutMenu()
        {
            return _trayIcon.AddSubMenu(Constants.Menus.RESTORE);
        }

        private void AddRestoreMenusTo(
            MenuItem menu
        )
        {
            _sectionNameHelper.ListLayoutNames()
                .ForEach(
                    s =>
                    {
                        _trayIcon.AddMenuItem(
                            s,
                            () =>
                            {
                                _layoutRestorer.RestoreLayout(s);
                            },
                            menu);
                    });
        }
    }
}