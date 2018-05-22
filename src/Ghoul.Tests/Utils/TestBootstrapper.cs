using System;
using System.Text;
using Ghoul.AppLogic;
using Ghoul.Tests.Matchers;
using Ghoul.Utils;
using NExpect;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.TrayIcon;
using static NExpect.Expectations;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace Ghoul.Tests.Utils
{
    [TestFixture]
    public class TestBootstrapper
    {
        [TestCase(typeof(ISectionNameHelper), typeof(SectionNameHelper))]
        [TestCase(typeof(ILayoutRestorer), typeof(LayoutRestorer))]
        [TestCase(typeof(ILayoutSaver), typeof(LayoutSaver))]
        [TestCase(typeof(IApplicationRestarter), typeof(ApplicationRestarter))]
        [TestCase(typeof(ILayoutRestorer), typeof(LayoutRestorer))]
        [TestCase(typeof(IINIFile), typeof(INIFile))]
        [TestCase(typeof(ITrayIcon), typeof(TrayIcon))]
        public void ShouldBeAbleToResolve_(Type serviceType, Type implementationType)
        {
            // Arrange
            var container = Bootstrapper.Bootstrap();
            // Pre-assert
            // Act
            var result = container.Resolve(serviceType, false);
            // Assert
            Expect(result).Not.To.Be.Null();
            Expect(result).To.Be.An.Instance.Of(implementationType);
        }

        [Test]
        public void StringBuilderVsStringJoin()
        {
            // Arrange
            var collection = GetRandomArray<string>(4096);
            var runs = 1024;
            // Pre-assert
            // Act
            var joinTime = Time(
                () => string.Join("", collection),
                runs);

            var builderTime = Time(
                () =>
                {
                    var builder = new StringBuilder();
                    foreach (var s in collection)
                    {
                        builder.Append(s);
                    }

                    return builder.ToString();
                },
                runs);

            var concatenationTime = Time(
                () =>
                {
                    var result = "";
                    foreach (var s in collection)
                    {
                        result += s;
                    }
                    return result;
                },
                runs);

            float foo = 1.23f;
            Math.Max(0, foo);

            // Assert
            Console.WriteLine($"Join:    {joinTime}ms");
            Console.WriteLine($"Builder: {builderTime}ms");
            Console.WriteLine($"Concat:  {concatenationTime}ms");
        }

        private TimeSpan Time(Func<string> action, int runs)
        {
            var start = DateTime.UtcNow;
            for (var i = 0; i < runs; i++)
            {
                action();
            }

            return DateTime.UtcNow - start;
        }
    }
}