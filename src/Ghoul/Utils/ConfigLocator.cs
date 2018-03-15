using System;
using System.IO;
using System.Reflection;

namespace Ghoul.Utils
{
    public interface IConfigLocator
    {
        string FindConfig();
    }

    // ReSharper disable once UnusedMember.Global
    public class ConfigLocator: IConfigLocator
    {
        public string FindConfig()
        {
            var programPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            // TODO: allow portable / user mode?
            var programFolder = Path.GetDirectoryName(programPath) ?? Directory.GetCurrentDirectory();
            return Path.Combine(programFolder, "ghoul.ini");
        }
    }
}