using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DryIoc;
using PeanutButter.INIFile;
using PeanutButter.Utils;
using PeanutButter.TinyEventAggregator;
using PeanutButter.TrayIcon;

namespace Ghoul
{
    public static class Bootstrapper
    {
        public static IContainer Bootstrap(ITrayIcon trayIcon)
        {
            var container = new Container();
            var allTypes = typeof(Bootstrapper).Assembly.GetTypes();
            allTypes
                .Where(t => t.IsInterface)
                .ForEach(
                    serviceType =>
                    {
                        if (serviceType?.Namespace?.StartsWith("DryIoc") ?? true)
                            return;
                        var implementation = allTypes.FirstOrDefault(
                            t => t.ImplementsServiceType(serviceType)
                        );
                        if (implementation == null)
                            return;
                        container.Register(serviceType, implementation, Reuse.Singleton);
                    });
            container.RegisterDelegate<IINIFile>(LoadConfig, Reuse.Singleton);
            // TODO: replace with usage of IEventAggregator
            container.RegisterDelegate(resolver => EventAggregator.Instance, Reuse.Singleton);
            container.RegisterDelegate(r => trayIcon, Reuse.Singleton);
            return container;
        }

        private static INIFile LoadConfig(IResolver resolver)
        {
            var configLocation = FindConfig();
            var result = new INIFile(configLocation);
            result.Persist(); // ensure config exists
            return result;
        }

        private static string FindConfig()
        {
            var programPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            // TODO: allow portable / user mode?
            var programFolder = Path.GetDirectoryName(programPath) ?? Directory.GetCurrentDirectory();
            return Path.Combine(programFolder, "ghoul.ini");
        }
    }
}