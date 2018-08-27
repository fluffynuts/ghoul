using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using Ghoul.Native;
using Ghoul.Ui;
using Ghoul.Utils;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.Utils;

namespace Ghoul.Tests
{
    [TestFixture]
    public class DiscoveryTests
    {
        [Test]
        [Explicit]
        public void FindSlack()
        {
            var moo = new DesktopWindowUtil();
            var windows = moo.ListWindows();
            windows.ForEach(
                w =>
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

        [Test]
        [Explicit]
        public void EnumDisplayDevices()
        {
            // Arrange
            var sut = new EnumDisplayDevicesTest();
            // Pre-assert
            // Act
            sut.Display();
            // Assert
        }

        public class EnumDisplayDevicesTest
        {
            [DllImport("user32.dll")]
            static extern bool EnumDisplayDevices(
                string lpDevice,
                uint iDevNum,
                ref DISPLAY_DEVICE lpDisplayDevice,
                uint dwFlags);

            [Flags()]
            public enum DisplayDeviceStateFlags : int
            {
                /// <summary>The device is part of the desktop.</summary>
                AttachedToDesktop = 0x1,
                MultiDriver = 0x2,

                /// <summary>The device is part of the desktop.</summary>
                PrimaryDevice = 0x4,

                /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
                MirroringDriver = 0x8,

                /// <summary>The device is VGA compatible.</summary>
                VGACompatible = 0x10,

                /// <summary>The device is removable; it cannot be the primary display.</summary>
                Removable = 0x20,

                /// <summary>The device has more display modes than its output devices support.</summary>
                ModesPruned = 0x8000000,
                Remote = 0x4000000,
                Disconnect = 0x2000000
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct DISPLAY_DEVICE
            {
                [MarshalAs(UnmanagedType.U4)] public int cb;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string DeviceName;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                public string DeviceString;

                [MarshalAs(UnmanagedType.U4)] public DisplayDeviceStateFlags StateFlags;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                public string DeviceID;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                public string DeviceKey;
            }

            public void Display()
            {
                DISPLAY_DEVICE d = new DISPLAY_DEVICE();
                d.cb = Marshal.SizeOf(d);
                try
                {
                    for (uint id = 0; EnumDisplayDevices(null, id, ref d, 0); id++)
                    {
                        if (d.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                        {
                            Console.WriteLine(
                                $@"{id}, {d.DeviceName}, {d.DeviceString}, {d.StateFlags}, {d.DeviceID}, {d.DeviceKey}"
                            );
                            d.cb = Marshal.SizeOf(d);
                            EnumDisplayDevices(d.DeviceName, 0, ref d, 0);
                            Console.WriteLine(
                                $"{d.DeviceName}, {d.DeviceString}"
                            );
                        }

                        d.cb = Marshal.SizeOf(d);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.ToString()}");
                }
            }
        }

        [Test]
        [Explicit]
        public void ReenumerateDevices()
        {
            var config = Substitute.For<IINIFile>();
            var general = new Dictionary<string, string>() {["DeviceEnumeration"] = "true"};
            config["general"].Returns(general);
            var sut = new DeviceReenumerator(Substitute.For<ILogger>(), config, Substitute.For<IConfigLocator>());
            sut.ReEnumerate();
        }
    }
}