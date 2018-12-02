using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using SetBrightness.Properties;
using Timer = System.Timers.Timer;

namespace SetBrightness
{
    // https://docs.microsoft.com/en-us/windows/desktop/Monitor/monitor-configuration
    public partial class Form1 : Form
    {
        private readonly BrightnessControl _brightnessControl;
        private int _displayHandler;

        private bool _autoStartMenuItemMouseDown;
        private bool _canChangeVisible = true;

        private readonly Timer _timer = new Timer(200);
        private readonly MouseHook _mouseHook = new MouseHook();
        public static string WindowName = "亮度调节";
        private bool _useContrastMenuItemMouseDown;
        private const int HighHeight = 110;
        private const int LowHeight = 66;
        private readonly DelayAction _delayAction = new DelayAction();

        public Form1()
        {
            /*new AllMonitorInfo().GetInfos();
            new WmiMonitor("").SetBrightness(50);

            var handles = DdcCiMonitorManager.GetMonitorHandles();
            Console.WriteLine("千辛万苦拿到了句柄数目：" + handles.Count);
            foreach (var ddcCiMonitor in handles)
            {
                ddcCiMonitor.SetBrightness(20);
            }

            Environment.Exit(0);
            /*********/

            InitializeComponent();
            ChangeWindowMessageFilter(WmCopydata, 1);

            var hWnd = Handle;
            _brightnessControl = new BrightnessControl(hWnd);
            UpdateScreenHandler();

            trackBar_brightness.ValueChanged += TrackBarBrightness_ValueChanged;
            trackBar_contrast.ValueChanged += TrackBar_contrast_ValueChanged;
            UpdateTrackBarValue();

            label_brightness.DataBindings.Add("Text", trackBar_brightness, "Value");
            label_contrast.DataBindings.Add("Text", trackBar_contrast, "value");

            Visible = false;
            Height = LowHeight;

            Text = WindowName;
            notifyIcon.Text = WindowName;

            _mouseHook.MouseWheel += _mouseHook_MouseWheel;
            _mouseHook.Install();

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

        private struct MonitorInfos
        {
            public MonitorInfo Brightness;
            public MonitorInfo Contrast;

            public override string ToString()
            {
                return "亮度：" + Brightness.Current + "　对比度：" + Contrast.Current;
            }

            public void GetInfos(BrightnessControl control, int handler)
            {
                Brightness = control.GetMonitorCapabilities(handler, true);
                Contrast = control.GetMonitorCapabilities(handler, false);
            }
        }

        private MonitorInfos MonitorInfosWrap
        {
            get
            {
                var infos = new MonitorInfos();
                try
                {
                    infos.GetInfos(_brightnessControl, _displayHandler);
                }
                catch (GetMonitorInfoFailException)
                {
                    UpdateScreenHandler();
                    infos.GetInfos(_brightnessControl, _displayHandler);
                }

                return infos;
            }
        }

        private void UpdateScreenHandler()
        {
            try
            {
                _displayHandler = _brightnessControl.updateDisplayHandler();
            }
            catch (NoSupportMonitorException)
            {
                MessageBox.Show("查找支持的显示器失败，程序将退出。\r\n\r\nFinding supporting screen failed, exiting.");
                notifyIcon.Visible = false;
                Environment.Exit(1);
            }
        }

        private void UpdateTrackBarValue()
        {
            var infos = MonitorInfosWrap;
            // 更新 trackBar 值
            trackBar_brightness.Value = infos.Brightness.Current;
            trackBar_contrast.Value = infos.Contrast.Current;
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

        private void TrackBar_contrast_ValueChanged(object sender, EventArgs e)
        {
            _brightnessControl?.SetContrast((short) trackBar_contrast.Value, _displayHandler);
        }

        private void TrackBarBrightness_ValueChanged(object sender, EventArgs e)
        {
            _brightnessControl?.SetBrightness((short) trackBar_brightness.Value, _displayHandler);
            SetNotifyIconText();
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

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                UpdateTrackBarValue();
                RelocateForm();
                ClearNotifyIconText();
            }
            else
            {
                SetNotifyIconText();
            }
        }

        private void RelocateForm(bool notUseCursorPos = false)
        {
            var screen = Screen.FromHandle(Handle);

            var left = screen.WorkingArea.X + screen.WorkingArea.Width - Width - 40;
            var left2 = Cursor.Position.X - Width / 2;
            Left = left;
            if (!notUseCursorPos)
            {
                Left = Math.Min(left, left2);
            }

            Top = screen.WorkingArea.Y + screen.WorkingArea.Height - Height - 5;
            Activate();
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
            Height = on ? HighHeight : LowHeight;
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

                        VisibleChanged -= Form1_VisibleChanged;
                        UpdateTrackBarValue();
                        Visible = true;
                        RelocateForm(true);
                        VisibleChanged += Form1_VisibleChanged;
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

        private void notifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            _delayAction.Delay(1000, null, TrottleSetText);
        }

        private void TrottleSetText()
        {
            _delayAction.Throttle(2000, this, SetNotifyIconText);
        }

        private void SetNotifyIconText()
        {
            if (Visible)
            {
                ClearNotifyIconText();
                return;
            }

            notifyIcon.Text = WindowName + "\r\n" + MonitorInfosWrap;
        }

        private void ClearNotifyIconText()
        {
            notifyIcon.Text = "";
        }
    }
}