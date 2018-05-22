using System.Linq;
using DryIoc;
using Ghoul.Ui;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;

namespace Ghoul.Utils
{
    public static class Bootstrapper
    {
        public static IContainer Bootstrap()
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
                        try
                        {
                            container.Register(serviceType, implementation, Reuse.Singleton);
                        }
                        catch (ContainerException)
                        {
                            /*
                             * ignore: could be from an interface we don't expect to inject
                             * -> tests should cover all interesting constructables
                             */
                        }
                    });
            container.RegisterDelegate<IINIFile>(LoadConfig, Reuse.Singleton);
            // TODO: replace with usage of IEventAggregator
            container.RegisterDelegate<IEventAggregator>(resolver => EventAggregator.Instance, Reuse.Singleton);
            container.RegisterDelegate<ITrayIcon>(
                r => {
                    var trayIcon = new TrayIcon(r.Resolve<IIconProvider>().MainIcon());
                    // ReSharper disable once LocalizableElement
                    trayIcon.NotifyIcon.Text = "Ghoul - Desktop layout resurrector";
                    return trayIcon;
                },
                Reuse.Singleton);

            container.RegisterDelegate<ITrayIconAnimator>(
                r =>
                {
                    var iconProvider = r.Resolve<IIconProvider>();
                    return new TrayIconAnimator(r.Resolve<ITrayIcon>(), iconProvider.MainIcon(), iconProvider.WaitIcon());
                },
                Reuse.Singleton);
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