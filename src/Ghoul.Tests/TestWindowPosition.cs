using NUnit.Framework;
using static NExpect.Expectations;
using NExpect;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace Ghoul.Tests
{
    [TestFixture]
    public class TestWindowPosition
    {
        [Test]
        public void ShouldBeAbleToRehydrateFromResultOfToString()
        {
            // Arrange
            var source = GetRandom<WindowPosition>();
            var serialized = source.ToString();
            // Pre-assert
            // Act
            var result = new WindowPosition(serialized);
            // Assert
            Expect(result).To.Deep.Equal(source);
        }
    }
}
