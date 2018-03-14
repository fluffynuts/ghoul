using System;
using System.Collections.Generic;
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

        [TestCase("")]
        [TestCase("\t")]
        [TestCase("moo:cow")]
        public void ShouldThrowWhenInvalidDehydratedSource_(string source)
        {
            // Arrange
            // Pre-assert
            // Act
            Expect(() => new WindowPosition(source))
                .To.Throw<ArgumentException>();
            // Assert
        }

        public static IEnumerable<(string config, Func<WindowPosition, bool> validator)> TestCaseGenerator()
        {
            var (rand1, rand2, rand3) = (GetRandomInt(), GetRandomInt(), GetRandomInt());
            yield return ($"Top: {rand1}", wp => wp.Top == rand1);
            yield return ($"width:{rand1}", wp => wp.Width == rand1);
            yield return ($"lEFt:\t{rand1}, foo: {rand2}, width: {rand3}", wp => wp.Left == rand1 && wp.Width == rand3);
        }

        [TestCaseSource(nameof(TestCaseGenerator))]
        public void ShouldAllowPartialRehydration(
            (string config, Func<WindowPosition, bool> validator) testCase
        )
        {
            // Arrange
            // Pre-assert
            // Act
            var sut = new WindowPosition(testCase.config);
            var result = testCase.validator(sut);
            // Assert
            Expect(result).To.Be.True();
        }
    }
}