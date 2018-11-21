using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using Timer = System.Timers.Timer;

namespace SetBrightness
{
    public partial class Form1 : Form
    {
        private readonly BrightnessControl _brightnessControl;
        private int _workingDisplay;
        private bool _mouseDown;
        private readonly Timer _timer = new Timer(200);
        private bool _canChangeVisible = true;
        private readonly MouseHook _mouseHook = new MouseHook();
        public static string WindowName = "亮度调节";

        public Form1()
        {
            InitializeComponent();
            ChangeWindowMessageFilter(WmCopydata, 1);

            var hWnd = Handle;
            _brightnessControl = new BrightnessControl(hWnd);
            BindSlider();

            label1.DataBindings.Add("Text", trackBar1, "Value");

            Visible = false;

            Text = WindowName;
            notifyIcon1.Text = WindowName;
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
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
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
                if (trackBar1.Value + wheelChange <= trackBar1.Maximum)
                {
                    trackBar1.Value += wheelChange;
                }
                else
                {
                    trackBar1.Value = trackBar1.Maximum;
                }
            }
            else
            {
                if (trackBar1.Value - wheelChange >= trackBar1.Minimum)
                {
                    trackBar1.Value -= wheelChange;
                }
                else
                {
                    trackBar1.Value = trackBar1.Minimum;
                }
            }

            goOn = false;
        }

        private void BindSlider()
        {
            var count = _brightnessControl.GetMonitors();
            var mInfo = new BrightnessInfo();
            for (var i = 0; i < count; i++)
            {
                mInfo = _brightnessControl.GetBrightnessCapabilities(i);
                if (mInfo.Current == -1)
                    continue;
                _workingDisplay = i;
                break;
            }

            trackBar1.Value = mInfo.Current;
            trackBar1.ValueChanged += TrackBar1_ValueChanged;
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            _brightnessControl?.SetBrightness((short) trackBar1.Value, _workingDisplay);
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
            notifyIcon1.Visible = false;
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
                BrightnessInfo mInfo;
                try
                {
                    mInfo = _brightnessControl.GetBrightnessCapabilities(_workingDisplay);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    _brightnessControl.DestroyMonitors();
                    _brightnessControl.SetupMonitors();
                    BindSlider();
                    mInfo = _brightnessControl.GetBrightnessCapabilities(_workingDisplay);
                }

                trackBar1.Value = mInfo.Current;

                Relocate();
            }
        }

        private void AutoStart_ToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (!_mouseDown)
            {
                return;
            }

            _mouseDown = false;

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
                notifyIcon1.ShowBalloonTip(3500, "设置成功", "如果移动程序位置，需要重新设置", ToolTipIcon.Info);
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

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }

        private void AutoStart_ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
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

        #region windows api functions

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
                        notifyIcon1.ShowBalloonTip(1500, "运行中", WindowName + "已经在运行", ToolTipIcon.Info);
                        Visible = true;
                    }

                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        #endregion

        #endregion
    }
}