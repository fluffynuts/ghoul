using System;
using Ghoul.AppLogic;
using Ghoul.Tests.Matchers;
using Ghoul.Utils;
using NExpect;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.TrayIcon;

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
            Expectations.Expect(result).Not.To.Be.Null();
            Expectations.Expect(result).To.Be.An.Instance.Of(implementationType);
        }
    }
}
