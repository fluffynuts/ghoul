using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using static PeanutButter.RandomGenerators.RandomValueGen;
using static NExpect.Expectations;
using NExpect;
using PeanutButter.RandomGenerators;
using PeanutButter.Utils;

namespace Ghoul.Tests
{
    [TestFixture]
    public class TestLayoutConfigurationItem
    {
        [TestFixture]
        public class Construction
        {
            [TestCase(nameof(LayoutConfigurationItem.ProcessPath), "Process.MainModule.FileName")]
            [TestCase(nameof(LayoutConfigurationItem.WindowTitle), nameof(ProcessWindow.WindowTitle))]
            [TestCase(nameof(LayoutConfigurationItem.Position), nameof(ProcessWindow.Position))]
            public void ShouldSet_(string target, string source)
            {
                // Arrange
                var processWindow = GetRandom<ProcessWindow>();
                var expected = processWindow.GetPropertyValue(source);
                // Pre-assert
                // Act
                var sut = Create(processWindow);
                var result = sut.GetPropertyValue(target);
                // Assert
                Expect(result).To.Deep.Equal(expected);
            }

            private LayoutConfigurationItem Create(
                ProcessWindow src)
            {
                return new LayoutConfigurationItem(src);
            }
        }
    }

    public class ProcessWindowBuilder : GenericBuilder<ProcessWindowBuilder, ProcessWindow>
    {
        public override ProcessWindow ConstructEntity()
        {
            var process = Process.GetProcesses()
                .Select(Filter)
                .FirstOrDefault(p => p != null);
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