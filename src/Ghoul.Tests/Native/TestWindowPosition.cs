using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Ghoul.Native;
using NExpect;
using NUnit.Framework;
using PeanutButter.DuckTyping.Extensions;
using PeanutButter.RandomGenerators;
using static NExpect.Expectations;
using static PeanutButter.RandomGenerators.RandomValueGen;

// ReSharper disable MemberCanBePrivate.Global

namespace Ghoul.Tests.Native
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
            Expect(result).To.Intersection.Equal(source.DuckAs<IRectangle>());
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

    public class WindowPlacementBuilder : GenericBuilder<WindowPlacementBuilder, Win32Api.WindowPlacement>
    {
        public override WindowPlacementBuilder WithRandomProps()
        {
            return base.WithRandomProps()
                .WithProp(o => o.Length = Marshal.SizeOf(o))
                // TODO: remove all of this when PeanutButter.RandomGenerators is updated to >= 1.2.224
                .WithProp(
                    o => o.NormalPosition = new Rectangle()
                    {
                        Width = GetRandomInt(),
                        Height = GetRandomInt(),
                        Size = new Size()
                        {
                            Height = GetRandomInt(),
                            Width = GetRandomInt()
                        },
                        Location = new Point()
                        {
                            Y = GetRandomInt(),
                            X = GetRandomInt()
                        },
                        X = GetRandomInt(),
                        Y = GetRandomInt()
                    });
        }
    }
}