using System;
using NUnit.Framework;
using PeanutButter.Utils;

namespace Ghoul.Tests
{
    [TestFixture]
    public class DiscoveryTests
    {
        [Test]
        public void FindSlack()
        {
            var moo = new DesktopWindowUtil();
            var windows = moo.ListWindows();
            windows.ForEach(w =>
            {
                new[]
                {
                    "--- start ---",
                    w.WindowTitle,
                    w.Process.MainModule.FileName,
                    $"{w.Position.Top} {w.Position.Left} {w.Position.Width} {w.Position.Height}",
                    "--- end ---"
                }.ForEach(s => Console.WriteLine(s));
            });
        }
    }
}