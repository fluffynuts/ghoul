using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
    public interface IApplicationCoordinator
    {
        void Init();
    }

    // ReSharper disable once UnusedMember.Global
    public class ApplicationCoordinator
        : IApplicationCoordinator
    {
        public const int MAX_RELOAD_ATTEMPTS = 5;
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
        private readonly ILogger _logger;
        private readonly IDeviceReenumerator _deviceReenumerator;
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
            IINIFile config,
            ILogger logger,
            IDeviceReenumerator deviceReenumerator
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
            _logger = logger;
            _deviceReenumerator = deviceReenumerator;
        }

        public void Init()
        {
            var restoreMenuItem = CreateMenuEntries();
            HandleLayoutAddedEventFor(restoreMenuItem);
            BindDoubleClickToRestoreLast();

            _eventAggregator.GetEvent<LayoutRestoreStartedEvent>()
                .Subscribe(Busy);
            _eventAggregator.GetEvent<LayoutRestoredEvent>()
                .Subscribe(Rest);
            _eventAggregator.GetEvent<LayoutSaveStartedEvent>()
                .Subscribe(Busy);
            _eventAggregator.GetEvent<LayoutSaveCompletedEvent>()
                .Subscribe(Rest);

            _eventAggregator.GetEvent<LayoutSaveStartedEvent>()
                .Subscribe(SuppressExternalFileChangeHandling);
            _eventAggregator.GetEvent<LayoutSaveCompletedEvent>()
                .Subscribe(UnsuppressExternalFileChangeHandling);
            _eventAggregator.GetEvent<ConfigChangedEvent>()
                .Subscribe(OnConfigChangedExternally);

            _configWatcher.StartWatching();
            _trayIcon.Show();
        }

        private void UnsuppressExternalFileChangeHandling(bool _)
        {
            _suppressExternalFileChangeHandling = false;
        }

        private void SuppressExternalFileChangeHandling(bool _)
        {
            _suppressExternalFileChangeHandling = true;
        }

        private void OnConfigChangedExternally(string obj)
        {
            if (_suppressExternalFileChangeHandling)
            {
                return;
            }

            AttemptConfigReload();
        }

        private void AttemptConfigReload(int attempt = 0)
        {
            try
            {
                _config.Reload();
            }
            catch (IOException ex)
            {
                if (++attempt > MAX_RELOAD_ATTEMPTS)
                {
                    _logger.LogError(
                        $"Giving up on config reload after {MAX_RELOAD_ATTEMPTS} attempts",
                        ex);
                }
                else
                {
                    Thread.Sleep(attempt * 1000);
                    AttemptConfigReload(attempt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Non-io-related exception whilst attempting config reload",
                    ex);
            }
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

            _trayIcon.AddMouseClickHandler(MouseClicks.Single,
                MouseButtons.Middle,
                ()
                    =>
                {
                    var reEnumerate = _config.GetSetting("general",
                        "middle-click-hardware-scan",
                        true);
                    if (!reEnumerate)
                        return;

                    _trayIcon.ShowBalloonTipFor(5000,
                        "Notice",
                        "Re-enumerating devices... please hold...",
                        ToolTipIcon.Info);
                    _deviceReenumerator.ReEnumerate();
                    _trayIcon.ShowBalloonTipFor(1000,
                        "Notice",
                        "Device re-enumeration complete!",
                        ToolTipIcon.Info);
                });

            return restoreMenuItem;
        }

        private void AddOpenConfigMenuItem()
        {
            _trayIcon.AddMenuItem("Edit config...",
                () =>
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
                        toRemove.ForEach(item
                            => item.Parent.MenuItems.Remove(item));
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