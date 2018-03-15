using System.Diagnostics;
using System.Linq;
using Ghoul.Native;
using NUnit.Framework;
using PeanutButter.RandomGenerators;

namespace Ghoul.Tests.Builders
{
    // ReSharper disable once UnusedMember.Global
    public class ProcessWindowBuilder : GenericBuilder<ProcessWindowBuilder, ProcessWindow>
    {
        public override ProcessWindow ConstructEntity()
        {
            var process = Process.GetProcesses()
                .Select(Filter)
                .FirstOrDefault(p => p != null);
            Assert.That(process, Is.Not.Null, () => "can't find any processes");
            return new ProcessWindow(process.MainWindowHandle);
        }

        private Process Filter(
            Process process)
        {
            try
            {
                return string.IsNullOrWhiteSpace(process.ProcessName) ||
                       string.IsNullOrWhiteSpace(process.MainModule.FileName) ||
                       string.IsNullOrWhiteSpace(process.MainWindowTitle)
                    ? null
                    : process;
            }
            catch
            {
                return null;
            }
        }
    }
}