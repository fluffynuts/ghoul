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
                        container.Register(serviceType, implementation, Reuse.Singleton);
                    });
            container.RegisterDelegate<IINIFile>(LoadConfig, Reuse.Singleton);
            // TODO: replace with usage of IEventAggregator
            container.RegisterDelegate<IEventAggregator>(resolver => EventAggregator.Instance, Reuse.Singleton);
            container.RegisterDelegate<ITrayIcon>(
                r => {
                    var trayIcon = new TrayIcon(Resources.main_icon);
                    // ReSharper disable once LocalizableElement
                    trayIcon.NotifyIcon.Text = "Ghoul - Desktop layout resurrector";
                    return trayIcon;
                },
                Reuse.Singleton);

            container.RegisterDelegate<TrayIconAnimator>(
                r =>
                    new TrayIconAnimator(r.Resolve<ITrayIcon>() as TrayIcon, Resources.main_icon, Resources.hourglass),
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