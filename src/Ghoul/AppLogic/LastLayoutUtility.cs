using System.Windows.Forms;
using Ghoul.AppLogic.Events;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
using PeanutButter.TrayIcon;
using Sections = Ghoul.Utils.Constants.Sections;
using Keys = Ghoul.Utils.Constants.Keys;

namespace Ghoul.AppLogic
{
    public interface ILastLayoutUtility
    {
        void RestoreLastLayout();
    }

    // ReSharper disable once UnusedMember.Global
    public class LastLayoutUtility : ILastLayoutUtility
    {
        private const string NO_LAST_LAYOUT_MESSAGE = "There is no stored last-layout value - please select a layout to restore and subsequent double-clicks will restore that layout";
        private const string NO_LAST_LAYOUT_CAPTION = "Information";

        private readonly IINIFile _config;
        private readonly ILayoutRestorer _layoutRestorer;
        private readonly ITrayIcon _trayIcon;

        public LastLayoutUtility(
            IINIFile config,
            ILayoutRestorer layoutRestorer,
            IEventAggregator eventAggregator,
            ITrayIcon trayIcon
        )
        {
            _config = config;
            _layoutRestorer = layoutRestorer;
            _trayIcon = trayIcon;
            eventAggregator
                .GetEvent<LayoutRestoredEvent>()
                .Subscribe(OnLayoutRestored);
        }

        private void OnLayoutRestored(string layout)
        {
            var existing = _config.GetValue(Sections.GENERAL, Keys.LAST_LAYOUT);
            _config.SetValue(Sections.GENERAL, Keys.LAST_LAYOUT, layout);
            _config.Persist();
            if (existing == layout)
                return;

            _trayIcon.ShowBalloonTipFor(
                    10000, 
                    "Tip",
                    $"The layout '{layout}' can be quickly restored by double-clicking the Ghoul icon",
                    ToolTipIcon.Info
                );
        }

        public void RestoreLastLayout()
        {
            var lastLayout = _config.GetValue(Sections.GENERAL, Keys.LAST_LAYOUT);
            if (lastLayout == null)
            {
                MessageBox.Show(
                    NO_LAST_LAYOUT_MESSAGE,
                    NO_LAST_LAYOUT_CAPTION,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            _layoutRestorer.RestoreLayout(lastLayout);
        }

    }
}