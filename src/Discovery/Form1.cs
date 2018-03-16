using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ghoul.Native;

namespace Discovery
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateResolution()
        {
            var screen = Screen.FromControl(this);
            ResolutionLabel.Text = $"{screen.WorkingArea.Width} x {screen.WorkingArea.Height}";
        }

        private void UpdateScaling()
        {
            var graphics = Graphics.FromHwnd(IntPtr.Zero);
            var desktop = graphics.GetHdc();
            var logical = Win32Api.GetDeviceCaps(desktop, Win32Api.DeviceCapabilities.VerticalResolution);
            var physical = Win32Api.GetDeviceCaps(desktop, Win32Api.DeviceCapabilities.DesktopVerticalResolution);
            ScalingLabel.Text = ((decimal) logical / (decimal) physical).ToString();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            UpdateResolution();
            UpdateScaling();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateInfo();
        }
    }
}
