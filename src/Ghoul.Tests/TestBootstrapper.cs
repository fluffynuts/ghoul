using System;
using NUnit.Framework;
using static NExpect.Expectations;
using NExpect;
using NSubstitute;
using PeanutButter.TrayIcon;

namespace Ghoul.Tests
{
    [TestFixture]
    public class TestBootstrapper
    {
        [TestCase(typeof(ISectionNameHelper), typeof(SectionNameHelper))]
        [TestCase(typeof(ILayoutRestorer), typeof(LayoutRestorer))]
        [TestCase(typeof(ILayoutSaver), typeof(LayoutSaver))]
        [TestCase(typeof(IApplicationRestarter), typeof(ApplicationRestarter))]
        public void ShouldBeAbleToResolve_(Type serviceType, Type implementationType)
        {
            // Arrange
            var container = Bootstrapper.Bootstrap(Substitute.For<ITrayIcon>());
            // Pre-assert
            // Act
            var result = container.Resolve(serviceType, false);
            // Assert
            Expect(result).Not.To.Be.Null();
            Expect(result).To.Be.An.Instance.Of(implementationType);
        }
    }
}
