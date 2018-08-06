using Ghoul.Utils;
using NUnit.Framework;
using static PeanutButter.RandomGenerators.RandomValueGen;
using static NExpect.Expectations;
using NExpect;
using PeanutButter.INIFile;

namespace Ghoul.Tests
{
    [TestFixture]
    public class TestConfigExtensions
    {
        [TestFixture]
        public class EnumerateLayouts
        {
            [Test]
            public void WhenEmptyConfig_ShouldProduceEmptyCollection()
            {
                // Arrange
                var config = new INIFile();
                // Pre-assert
                // Act
                var result = config.EnumerateLayouts();
                // Assert
                Expect(result).To.Be.Empty();
            }

            [Test]
            public void WhenHaveOneSavedWindowForOneLayout_ShouldProduceOneLayout()
            {
                // Arrange
                var config = new INIFile();
                var layout = GetRandomString(10);
                var processName = GetRandomString(10);
                config.AddSection($"app-layout: {layout} : {processName}");
                // Pre-assert
                // Act
                var result = config.EnumerateLayouts();
                // Assert
                Expect(result).To.Equal(new[] { layout });
            }

            [Test]
            public void WhenHaveTwoSavedWindowsForOneLayout_ShouldProduceOneLayout()
            {
                // Arrange
                var config = new INIFile();
                var layout = GetRandomString(10);
                var processName1 = GetRandomString(10);
                var processName2 = GetAnother(processName1);
                config.AddSection($"app-layout: {layout} : {processName1}");
                config.AddSection($"app-layout: {layout} : {processName2}");
                // Pre-assert
                // Act
                var result = config.EnumerateLayouts();
                // Assert
                Expect(result).To.Equal(new[] { layout });
            }            
            
            [Test]
            public void WhenHaveTwoSavedWindowsForTwoLayouts_ShouldProduceTwoLayouts()
            {
                // Arrange
                var config = new INIFile();
                var layout1 = GetRandomString(10);
                var layout2 = GetAnother(layout1);
                var processName1 = GetRandomString(10);
                var processName2 = GetAnother(processName1);
                config.AddSection($"app-layout: {layout1} : {processName1}");
                config.AddSection($"app-layout: {layout2} : {processName2}");
                // Pre-assert
                // Act
                var result = config.EnumerateLayouts();
                // Assert
                Expect(result).To.Equal(new[] { layout1, layout2 });
            }
        }
    }
}