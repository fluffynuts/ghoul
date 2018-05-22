using Ghoul.AppLogic;
using Ghoul.Native;
using NExpect;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.Utils;
using static NExpect.Expectations;

namespace Ghoul.Tests.AppLogic
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
                var processWindow = RandomValueGen.GetRandom<ProcessWindow>();
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
}