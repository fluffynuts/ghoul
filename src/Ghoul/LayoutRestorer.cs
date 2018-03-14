using System;
using System.Linq;
using PeanutButter.INIFile;
using PeanutButter.Utils;
using Keys = Ghoul.Constants.Keys;

namespace Ghoul
{
    internal interface ILayoutRestorer
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

        public LayoutRestorer(
            IINIFile config,
            IApplicationRestarter applicationRestarter,
            ISectionNameHelper sectionNameHelper,
            IDesktopWindowUtil desktopWindowUtil
        )
        {
            _config = config;
            _applicationRestarter = applicationRestarter;
            _sectionNameHelper = sectionNameHelper;
            _desktopWindowUtil = desktopWindowUtil;
        }

        public void RestoreLayout(string name)
        {
            _applicationRestarter.RestartApplicationsForLayout(name);
            RestoreWindowPositionsFor(name);
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
        }

        private string FindBestMatchFor(
            ProcessWindow window,
            string[] appLayoutSections)
        {
            return WindowMatchStrategies.Aggregate(
                null as string,
                (finalMatch, currentStrategy) =>
                {
                    return finalMatch ?? appLayoutSections.Aggregate(
                               null as string,
                               (thisSectionMatch, currentSection) =>
                                   thisSectionMatch ??
                                   (currentStrategy(_config, currentSection, window)
                                       ? currentSection
                                       : null)
                           );
                });
        }

        private static readonly Func<IINIFile, string, ProcessWindow, bool>[] WindowMatchStrategies =
        {
            IsExactMatch
        };

        private static bool IsExactMatch(
            IINIFile config,
            string section,
            ProcessWindow window)
        {
            var (title, executable) = GetTitleAndExecutableFrom(config, section);
            return window.WindowTitle == title &&
                   window.ProcessName == executable;
        }

        private void ApplyLayout(
            ProcessWindow window,
            string section)
        {
            try
            {
                var windowPosition = new WindowPosition(_config.GetValue(section, Keys.POSITION));
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
            IINIFile config,
            string section)
        {
            return (
                config.GetValue(section, Keys.TITLE),
                config.GetValue(section, Keys.EXECUTABLE)
            );
        }
    }
}