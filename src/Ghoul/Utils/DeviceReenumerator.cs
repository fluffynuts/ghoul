using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ghoul.Ui;
using PeanutButter.INIFile;
using PeanutButter.Utils;
using DryIoc;

namespace Ghoul.Utils
{
    public interface IDeviceReenumerator
    {
        bool ReEnumerate();
    }

    public class DeviceReenumerator
        : IDeviceReenumerator
    {
        private readonly ILogger _logger;
        private readonly IINIFile _config;
        private readonly IConfigLocator _configLocator;

        // refactored from https://stackoverflow.com/questions/2181525/is-there-an-api-call-to-start-a-scan-for-hardware-devices#2836197
        //    causes device re-enumeration, which can make monitors which are plugged in but
        //    not currently available be detected
        private const int CM_LOCATE_DEVNODE_NORMAL = 0x00000000;
        private const int CM_REENUMERATE_NORMAL = 0x00000000;
        private const int CR_SUCCESS = 0x00000000;

        [DllImport("CfgMgr32.dll", SetLastError = true)]
        private static extern int CM_Locate_DevNodeA(ref int pdnDevInst, string pDeviceID, int ulFlags);

        [DllImport("CfgMgr32.dll", SetLastError = true)]
        private static extern int CM_Reenumerate_DevNode(int dnDevInst, int ulFlags);

        public DeviceReenumerator(
            ILogger logger,
            IINIFile config,
            IConfigLocator configLocator)
        {
            _logger = logger;
            _config = config;
            _configLocator = configLocator;
        }

        // TODO: make this async so we can cancel it if it takes too long (and auto-disable the feature)
        public bool ReEnumerate()
        {
            if (ReEnumerationDisabled())
            {
                _logger.LogInfo("Device re-enumeration disabled.");
                return true;
            }

            var finalResult = false;
            var thread = new Thread(() => finalResult = ActuallyReEnumerateDevices());
            thread.Start();
            if (!thread.Join(TimeSpan.FromSeconds(5)))
            {
                MessageBox.Show(
                    null,
                    "Unable to re-enumerate devices timeously. If this persists and re-enumeration isn't important to you, disable this feature in config by setting 'DeviceEnumeration=false' in the [general] section", 
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                try
                {
                    thread.Abort();
                }
                catch
                {
                    /* intentionally left blank */
                }
            }
            return finalResult;
        }

        private bool ActuallyReEnumerateDevices()
        {
            int pdnDevInst = 0;

            _logger.LogInfo("Locating device node for re-enumeration...");
            var first =
                CM_Locate_DevNodeA(ref pdnDevInst, null, CM_LOCATE_DEVNODE_NORMAL) ==
                CR_SUCCESS;
            if (!first)
            {
                return false;
            }

            _logger.LogInfo(
                new[]
                {
                    "Re-enumerating devices.",
                    "If the application hangs during this operation, please disable device enumeration.",
                    $"(set DeviceEnumeration in the [general] section of {_configLocator.FindConfig()} to false)"
                }.JoinWith("\r\n"));

            var second = CM_Reenumerate_DevNode(pdnDevInst, CM_REENUMERATE_NORMAL) ==
                         CR_SUCCESS;
            return second;
        }

        private bool ReEnumerationDisabled()
        {
            return !_config.GetSetting<bool>("general", "DeviceEnumeration", true);
        }
    }

    public static class INIFileExtensions
    {
        public static T GetSetting<T>(
            this IINIFile iniFile,
            string section,
            string setting,
            T defaultValue = default(T))
        {
            if (!iniFile.HasSetting(section, setting))
                return defaultValue;
            var stringValue = iniFile[section][setting];
            return AttemptToConvert<T>(stringValue, defaultValue);
        }

        private static ILogger _logger;

        private static void LogWarning(string str)
        {
            var logger = _logger ?? (_logger = ResolveLogger());
            logger?.LogWarning(str);
        }

        private static ILogger ResolveLogger()
        {
            try
            {
                return Bootstrapper.Bootstrap().Resolve<ILogger>();
            }
            catch
            {
                return null;
            }
        }

        private static T AttemptToConvert<T>(string stringValue, T defaultValue)
        {
            var type = typeof(T);
            if (_converters.TryGetValue(type, out var converter))
            {
                try
                {
                    return (T) converter(stringValue);
                }
                catch
                {
                    LogWarning($"Unable to convert '{stringValue}' to type {type} -- falling back on {defaultValue}");
                    ;
                    return defaultValue;
                }
            }

            LogWarning($"No converter from string to {type} -- falling back on {defaultValue}");
            return defaultValue;
        }

        private static readonly Dictionary<Type, Func<string, object>> _converters =
            new Dictionary<Type, Func<string, object>>()
            {
                [typeof(bool)] = s => s.AsBoolean(),
                [typeof(int)] = s => s.AsInteger(),
                [typeof(string)] = s => s
            };
    }
}