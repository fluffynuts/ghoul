using System;
using System.IO;
using System.Runtime.CompilerServices;
using Ghoul.AppLogic.Events;
using Ghoul.Utils;
using PeanutButter.TinyEventAggregator;

namespace Ghoul.Ui
{
    public interface IConfigWatcher
    {
        void StartWatching();
        void StopWatching();
    }

    public class ConfigWatcher : IConfigWatcher
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
            if (e.ChangeType == WatcherChangeTypes.Changed &&
                string.Equals(e.FullPath, _configLocator.FindConfig(), StringComparison.OrdinalIgnoreCase))
            {
                _eventAggregator.GetEvent<ConfigChangedEvent>().Publish(_configLocator.FindConfig());
            }
        }
    }
}
    