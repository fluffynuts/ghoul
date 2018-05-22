using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ghoul.AppLogic;
using Ghoul.AppLogic.Events;
using Ghoul.Utils;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;

namespace Ghoul.Ui
{
    public interface IConfigWatcher
    {
        void StartWatching();
        void StopWatching();
    }
    
    public class ConfigWatcher: IConfigWatcher
    {
        private readonly IConfigLocator _configLocator;
        private readonly IEventAggregator _eventAggregator;
        private FileSystemWatcher _watcher;

        public ConfigWatcher(
            IConfigLocator configLocator,
            IEventAggregator eventAggregator)
        {
            _configLocator = configLocator;
            _eventAggregator = eventAggregator;
        }

        public void StartWatching()
        {
            var watchPath = Path.GetDirectoryName(_configLocator.FindConfig());
            if (watchPath == null)
                return;
            _watcher = new FileSystemWatcher(watchPath)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.ini"
            };
            _watcher.Changed += OnConfigChanged;
            _watcher.EnableRaisingEvents = true;
        }

        public void StopWatching()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= OnConfigChanged;
        }

        private void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            if (string.Equals(e.FullPath, _configLocator.FindConfig(), StringComparison.OrdinalIgnoreCase))
            {
                _eventAggregator.GetEvent<ConfigChangedEvent>().Publish(_configLocator.FindConfig());
            }
        }
    }

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
        private readonly ITrayIconAnimator _animator;
        private readonly IConfigLocator _configLocator;
        private readonly IConfigWatcher _configWatcher;
        private readonly IINIFile _config;
        private bool _suppressExternalFileChangeHandling;

        public ApplicationCoordinator(
            ILayoutSaver layoutSaver,
            ITrayIcon trayIcon,
            ILayoutRestorer layoutRestorer,
            ISectionNameHelper sectionNameHelper,
            IEventAggregator eventAggregator,
            ILastLayoutUtility lastLayoutUtility,
            ITrayIconAnimator animator,
            IConfigLocator configLocator,
            IConfigWatcher configWatcher,
            IINIFile config
        )
        {
            _layoutSaver = layoutSaver;
            _trayIcon = trayIcon;
            _layoutRestorer = layoutRestorer;
            _sectionNameHelper = sectionNameHelper;
            _eventAggregator = eventAggregator;
            _lastLayoutUtility = lastLayoutUtility;
            _animator = animator;
            _configLocator = configLocator;
            _configWatcher = configWatcher;
            _config = config;
        }

        public void Init()
        {
            var restoreMenuItem = CreateMenuEntries();
            HandleLayoutAddedEventFor(restoreMenuItem);
            BindDoubleClickToRestoreLast();
            
            _eventAggregator.GetEvent<LayoutRestoreStartedEvent>().Subscribe(Busy);
            _eventAggregator.GetEvent<LayoutRestoredEvent>().Subscribe(Rest);
            _eventAggregator.GetEvent<LayoutSaveStartedEvent>().Subscribe(Busy);
            _eventAggregator.GetEvent<LayoutSaveCompletedEvent>().Subscribe(Rest);
            
            _eventAggregator.GetEvent<LayoutSaveStartedEvent>().Subscribe(SuppressExternalFileChangeHandling);
            _eventAggregator.GetEvent<LayoutSaveCompletedEvent>().Subscribe(UnsuppressExternalFileChangeHandling);
            _eventAggregator.GetEvent<ConfigChangedEvent>().Subscribe(OnConfigChangedExternally);

            _configWatcher.StartWatching();
            _trayIcon.Show();
        }

        private void UnsuppressExternalFileChangeHandling(bool obj)
        {
            _suppressExternalFileChangeHandling = false;
        }

        private void SuppressExternalFileChangeHandling(bool obj)
        {
            _suppressExternalFileChangeHandling = true;
        }

        private void OnConfigChangedExternally(string obj)
        {
            _config.Reload();
        }

        private void Rest<T>(T arg)
        {
            _animator.Rest();
        }

        private void Busy<T>(T arg)
        {
            _animator.Busy();
        }

        private MenuItem CreateMenuEntries()
        {
            AddSaveLayoutMenuItem();
            var restoreMenuItem = AddRestoreLayoutMenu();
            AddRestoreMenusTo(restoreMenuItem);
            AddSeparator();
            AddOpenConfigMenuItem();
            AddExitMenuItem();
            return restoreMenuItem;
        }

        private void AddOpenConfigMenuItem()
        {
            _trayIcon.AddMenuItem("Edit config...", () =>
            {
                Process.Start(_configLocator.FindConfig());
            });
        }

        private void AddSeparator()
        {
            _trayIcon.AddMenuSeparator();
        }

        private void AddExitMenuItem(
        )
        {
            _trayIcon.AddMenuItem(
                "Exit",
                () =>
                {
                    _trayIcon.Hide();
                    _configWatcher.StopWatching();
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
                .OrderBy(s => s)
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