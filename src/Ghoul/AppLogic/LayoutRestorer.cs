using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ghoul.AppLogic.Events;
using Ghoul.Native;
using Ghoul.Utils;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;
using Keys = Ghoul.Utils.Constants.Keys;

namespace Ghoul.AppLogic
{
    public interface ILayoutRestorer
    {
        void RestoreLayout(string name);
    }

    internal class LayoutRestorer
        : ILayoutRestorer
    {
        private readonly IINIFile _config;
        private readonly IApplicationRestarter _applicationRestarter;
        private readonly ISectionNameHelper _sectionNameHelper;
        private readonly IDesktopWindowUtil _desktopWindowUtil;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITrayIcon _trayIcon;
        private readonly IDeviceReenumerator _deviceReenumerator;

        public LayoutRestorer(
            IINIFile config,
            IApplicationRestarter applicationRestarter,
            ISectionNameHelper sectionNameHelper,
            IDesktopWindowUtil desktopWindowUtil,
            IEventAggregator eventAggregator,
            ITrayIcon trayIcon,
            IDeviceReenumerator deviceReenumerator
        )
        {
            _config = config;
            _applicationRestarter = applicationRestarter;
            _sectionNameHelper = sectionNameHelper;
            _desktopWindowUtil = desktopWindowUtil;
            _eventAggregator = eventAggregator;
            _trayIcon = trayIcon;
            _deviceReenumerator = deviceReenumerator;
        }

        private readonly Queue<string> _restoreQueue = new Queue<string>();

        public void RestoreLayout(string name)
        {
            _restoreQueue.Enqueue(name);
            ProcessRestoreQueue();
        }

        private void ProcessRestoreQueue()
        {
            lock (_restoreQueue)
            {
                while (_restoreQueue.Any())
                {
                    var name = _restoreQueue.Dequeue();
                    _eventAggregator.GetEvent<LayoutRestoreStartedEvent>().Publish(name);
                    _deviceReenumerator.Reenumerate();
                    _applicationRestarter.RestartApplicationsForLayout(name);
                    RestoreWindowPositionsFor(name);
                }
            }
        }

        private void RestoreWindowPositionsFor(string layout)
        {
            var appLayoutSections = _sectionNameHelper.ListAppLayoutSectionsFor(layout);
            _desktopWindowUtil.ListWindows()
                .ForEach(
                    window =>
                    {
                        var section = FindBestMatchFor(window, appLayoutSections);
                        if (section == null)
                            return;
                        ApplyLayout(window, section);
                    });
            _eventAggregator
                .GetEvent<LayoutRestoredEvent>()
                .Publish(layout);
            _trayIcon.ShowBalloonTipFor(
                5000,
                "Layout restored",
                $"Layout '{layout}' was restored",
                ToolTipIcon.Info
            );
        }

        private string FindBestMatchFor(
            ProcessWindow window,
            string[] appLayoutSections)
        {
            var stored = appLayoutSections.Aggregate(
                new Dictionary<string, Dictionary<string, string>>(),
                (acc, section) =>
                {
                    acc[section] = new[]
                        {
                            Keys.POSITION,
                            Keys.EXECUTABLE,
                            Keys.TITLE
                        }.Select(
                            k => new
                            {
                                k,
                                v = _config.GetValue(section, k)
                            })
                        .ToDictionary(o => o.k, o => o.v);
                    return acc;
                });
            return WindowMatchStrategies.Aggregate(
                null as string,
                (finalMatch, currentStrategy) =>
                {
                    return finalMatch ?? appLayoutSections.Aggregate(
                               null as string,
                               (thisSectionMatch, currentSection) =>
                                   thisSectionMatch ??
                                   (currentStrategy(stored[currentSection], window)
                                       ? currentSection
                                       : null)
                           );
                });
        }

        private static readonly Func<Dictionary<string, string>, ProcessWindow, bool>[]
            WindowMatchStrategies =
            {
                IsExactMatch,
                IsCaseInsensitiveMatch,
                IsProcessAndPartialWindowMatch,
                IsProcessMatchOnly
            };

        private static bool IsProcessMatchOnly(
            Dictionary<string, string> config,
            ProcessWindow window)
        {
            return config.TryGetValue(Keys.EXECUTABLE, out var exe) &&
                   window.ProcessName.Equals(exe, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool IsExactMatch(
            Dictionary<string, string> config,
            ProcessWindow window)
        {
            var (title, executable) = GetTitleAndExecutableFrom(config);
            return window.WindowTitle.Equals(title, StringComparison.CurrentCulture) &&
                   window.ProcessName.Equals(executable, StringComparison.CurrentCulture);
        }

        private static bool IsCaseInsensitiveMatch(
            Dictionary<string, string> config,
            ProcessWindow window)
        {
            var (title, executable) = GetTitleAndExecutableFrom(config);
            return window.WindowTitle.Equals(title, StringComparison.CurrentCultureIgnoreCase) &&
                   window.ProcessName.Equals(executable, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool IsProcessAndPartialWindowMatch(
            Dictionary<string, string> config,
            ProcessWindow window)
        {
            var (title, executable) = GetTitleAndExecutableFrom(config);
            title = title.Split('-').Last(); // windows which include a document / project / extra title
            return window.ProcessName.Equals(executable, StringComparison.CurrentCultureIgnoreCase) &&
                   window.WindowTitle.Split('-').Last().IsSimilarTo(title);
        }

        private void ApplyLayout(
            ProcessWindow window,
            string section)
        {
            try
            {
                var windowPosition = new WindowPosition(_config.GetValue(section, Keys.POSITION));
                window.Restore();
                window.MoveTo(windowPosition.Left, windowPosition.Top, windowPosition.Width, windowPosition.Height);
            }
            catch
            {
                // TODO: log the error perhaps? for the moment, just ignore anything
                //  we can't move or dehydrate properly -- one reason might just be
                //  that we're not running elevated and the matched window is.
            }
        }

        private static (string title, string executable) GetTitleAndExecutableFrom(
            Dictionary<string, string> config
        )
        {
            return (
                config.TryGetValue(Keys.TITLE, out var title)
                    ? title
                    : "",
                config.TryGetValue(Keys.EXECUTABLE, out var exe)
                    ? exe
                    : ""
            );
        }
    }
}