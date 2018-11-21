using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using SetBrightness.Properties;
using Timer = System.Timers.Timer;

namespace SetBrightness
{
    public partial class Form1 : Form
    {
        private readonly BrightnessControl _brightnessControl;
        private int _workingDisplay;
        private bool _autoStartMenuItemMouseDown;
        private readonly Timer _timer = new Timer(200);
        private bool _canChangeVisible = true;
        private readonly MouseHook _mouseHook = new MouseHook();
        public static string WindowName = "亮度调节";
        private bool _useContrastMenuItemMouseDown;
        private const int _high_height = 110;
        private const int _low_height = 66;

        public Form1()
        {
            InitializeComponent();
            ChangeWindowMessageFilter(WmCopydata, 1);

            var hWnd = Handle;
            _brightnessControl = new BrightnessControl(hWnd);
            BindSlider();

            label_brightness.DataBindings.Add("Text", trackBar_brightness, "Value");
            label_contrast.DataBindings.Add("Text", trackBar_contrast, "value");

            Visible = false;
            Height = _low_height;

            Text = WindowName;
            notifyIcon.Text = WindowName;
            _mouseHook.MouseWheel += _mouseHook_MouseWheel;
            _mouseHook.Install();

            Closing += Form1_Closing;

            _timer.Elapsed += (sender, args) =>
            {
                _canChangeVisible = true;
                _timer.Stop();
            };

            Application.ApplicationExit += Application_ApplicationExit;

            CheckRegistry();
            CheckSetting();
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private bool UseContrast
        {
            get { return Settings.Default.use_contrast; }
            set
            {
                Settings.Default.use_contrast = value;
                Settings.Default.Save();
                SwitchContrast(value);
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            _mouseHook.Uninstall();
        }

        private void _mouseHook_MouseWheel(MouseHook.Msllhookstruct mouseStruct, out bool goOn)
        {
            if (!Visible)
            {
                goOn = true;
                return;
            }

            Int16 delta = (short) (mouseStruct.mouseData >> 16);
            var wheelChange = 5;
            if (delta > 0)
            {
                if (trackBar_brightness.Value + wheelChange <= trackBar_brightness.Maximum)
                {
                    trackBar_brightness.Value += wheelChange;
                }
                else
                {
                    trackBar_brightness.Value = trackBar_brightness.Maximum;
                }
            }
            else
            {
                if (trackBar_brightness.Value - wheelChange >= trackBar_brightness.Minimum)
                {
                    trackBar_brightness.Value -= wheelChange;
                }
                else
                {
                    trackBar_brightness.Value = trackBar_brightness.Minimum;
                }
            }

            goOn = false;
        }

        private void BindSlider()
        {
            var count = _brightnessControl.GetMonitors();
            var brightnessInfo = new MonitorInfo();
            var contrastInfo = new MonitorInfo();
            for (var i = 0; i < count; i++)
            {
                brightnessInfo = _brightnessControl.GetMonitorCapabilities(i, true);
                if (brightnessInfo.Current == -1)
                    continue;
                contrastInfo = _brightnessControl.GetMonitorCapabilities(i, false);
                _workingDisplay = i;
                break;
            }

            trackBar_brightness.Value = brightnessInfo.Current;
            trackBar_brightness.ValueChanged += TrackBarBrightness_ValueChanged;
            trackBar_contrast.Value = contrastInfo.Current;
            trackBar_contrast.ValueChanged += TrackBar_contrast_ValueChanged;
        }

        private void TrackBar_contrast_ValueChanged(object sender, EventArgs e)
        {
            _brightnessControl?.SetContrast((short) trackBar_contrast.Value, _workingDisplay);
        }

        private void TrackBarBrightness_ValueChanged(object sender, EventArgs e)
        {
            _brightnessControl?.SetBrightness((short) trackBar_brightness.Value, _workingDisplay);
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (_canChangeVisible && e.Button == MouseButtons.Left)
            {
                Visible = !Visible;
            }
        }

        private void Exit_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            _brightnessControl.DestroyMonitors();
            Application.Exit();
        }

        private void Relocate()
        {
            var screen = Screen.FromHandle(Handle);
            var x = screen.WorkingArea.X + screen.WorkingArea.Width - Width - 100;
            var y = screen.WorkingArea.Y + screen.WorkingArea.Height - Height - 10;
            Location = new Point(x, y);
            Activate();
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                MonitorInfo brightnessInfo;
                MonitorInfo contrastInfo;
                try
                {
                    brightnessInfo = _brightnessControl.GetMonitorCapabilities(_workingDisplay, true);
                    contrastInfo = _brightnessControl.GetMonitorCapabilities(_workingDisplay, false);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    _brightnessControl.DestroyMonitors();
                    _brightnessControl.SetupMonitors();
                    BindSlider();
                    brightnessInfo = _brightnessControl.GetMonitorCapabilities(_workingDisplay, true);
                    contrastInfo = _brightnessControl.GetMonitorCapabilities(_workingDisplay, false);
                }

                trackBar_brightness.Value = brightnessInfo.Current;
                trackBar_contrast.Value = contrastInfo.Current;

                Relocate();
            }
        }

        private void AutoStart_ToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (!_autoStartMenuItemMouseDown)
            {
                return;
            }

            _autoStartMenuItemMouseDown = false;

            var tsmi = sender as ToolStripMenuItem;

            EditRegistry(tsmi != null && tsmi.CheckState != CheckState.Checked);
        }

        private void EditRegistry(bool delete)
        {
            var name = Application.ProductName;
            const string registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var key = Registry.CurrentUser.OpenSubKey(registryPath, true);
            if (delete)
            {
                key?.DeleteValue(name, false);
            }
            else
            {
                notifyIcon.ShowBalloonTip(3500, "设置成功", "如果移动程序位置，需要重新设置", ToolTipIcon.Info);
                key?.SetValue(name, Application.ExecutablePath);
            }
        }

        private void CheckRegistry()
        {
            const string registryPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            if (Registry.GetValue(registryPath, Application.ProductName, null) == null)
            {
                AutoStart_ToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                AutoStart_ToolStripMenuItem.CheckState = CheckState.Checked;
            }
        }

        private void CheckSetting()
        {
            Contast_ToolStripMenuItem.CheckState = UseContrast ? CheckState.Checked : CheckState.Unchecked;
            if (UseContrast)
            {
                SwitchContrast(UseContrast);
            }
        }

        private void SwitchContrast(bool on)
        {
            label_contrast.Visible = trackBar_contrast.Visible = on;
            Height = on ? _high_height : _low_height;
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }

        private void AutoStart_ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            _autoStartMenuItemMouseDown = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        #region 样式控制

        protected override CreateParams CreateParams
        {
            get
            {
                const int csDropshadow = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= csDropshadow;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.WhiteSmoke, ButtonBorderStyle.Solid);
        }

        #endregion

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (Visible)
            {
                _canChangeVisible = false;
                _timer.Start();
                Visible = false;
            }
        }

        #region 消息

        private const int WmCopydata = 0x004A;

        [DllImport("user32")]
        public static extern bool ChangeWindowMessageFilter(uint msg, int flags);

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WmCopydata:
                    MessageSender.CopyDataStruct mystr = new MessageSender.CopyDataStruct();
                    Type mytype = mystr.GetType();
                    mystr = (MessageSender.CopyDataStruct) m.GetLParam(mytype);
                    if (mystr.LpData == MessageSender.Msg)
                    {
                        notifyIcon.ShowBalloonTip(1500, "运行中", WindowName + "已经在运行", ToolTipIcon.Info);
                        Visible = true;
                    }

                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        #endregion

        private void Contast_ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            _useContrastMenuItemMouseDown = true;
        }

        private void Contast_ToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (!_useContrastMenuItemMouseDown)
            {
                return;
            }

            _useContrastMenuItemMouseDown = false;
            var tsmi = sender as ToolStripMenuItem;
            if (tsmi != null)
            {
                UseContrast = tsmi.Checked;
            }
        }
    }
}