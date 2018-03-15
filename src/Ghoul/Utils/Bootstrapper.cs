using System.Linq;
using DryIoc;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;

namespace Ghoul.Utils
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
            container.RegisterDelegate<IEventAggregator>(resolver => EventAggregator.Instance, Reuse.Singleton);
            container.RegisterDelegate<ITrayIcon>(r => trayIcon, Reuse.Singleton);
            return container;
        }

        private static INIFile LoadConfig(IResolver resolver)
        {
            var configLocation = resolver.Resolve<IConfigLocator>().FindConfig();
            var result = new INIFile(configLocation);
            result.Persist(); // ensure config exists
            return result;
        }

    }

}