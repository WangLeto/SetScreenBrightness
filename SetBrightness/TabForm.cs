using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using SetBrightness.Properties;
using Timer = System.Timers.Timer;

namespace SetBrightness
{
    public partial class TabForm : Form
    {
        private const string PageControlName = nameof(TabPageTemplate);
        private readonly CheckManager _checkManager;

        private readonly Timer _timer = new Timer(150);
        private bool _canChangeVisible = true;

        private readonly MouseHook _mouseHook = new MouseHook();
        private readonly MonitorsManager _monitorsManager;

        public TabForm()
        {
            InitializeComponent();
            _monitorsManager = new MonitorsManager(AddPage, ClearPages);

            _checkManager = new CheckManager(this, contextMenuStrip);

            _timer.Elapsed += (sender, args) =>
            {
                _canChangeVisible = true;
                _timer.Stop();
            };

            ChangeWindowMessageFilter(WmCopydata, 1);

            _mouseHook.MouseWheel += _mouseHook_MouseWheel;
            _mouseHook.Install();
            Application.ApplicationExit += Application_ApplicationExit;
            _monitorsManager.RefreshMonitors();
        }

        private void AddPage(Monitor monitor)
        {
            var page = new TabPage(monitor.Name);
            page.Controls.Add(new TabPageTemplate(monitor, PageControlName, _monitorsManager));
            tabControl.TabPages.Add(page);
        }

        private void ClearPages()
        {
            tabControl.TabPages.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.FromArgb(214, 214, 214),
                ButtonBorderStyle.Solid);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Environment.Exit(0);
        }

        #region prevent contextmenustrip close on checking

        private bool _preventContextMenuStripClose;

        private void autoStartToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            _preventContextMenuStripClose = true;
        }

        private void useContrastToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            _preventContextMenuStripClose = true;
        }

        private void contextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (!_preventContextMenuStripClose)
            {
                return;
            }

            e.Cancel = true;
            _preventContextMenuStripClose = false;
        }

        #endregion

        public void autoStartToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            var enableAutoStart = tsmi != null && tsmi.CheckState == CheckState.Checked;
            _checkManager.CheckAutoStart(enableAutoStart);

            if (enableAutoStart)
            {
                notifyIcon.ShowBalloonTip(3500, "设置成功", "如果移动程序位置，需要重新设置", ToolTipIcon.Info);
            }
        }

        public void useContrastToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            var useContrast = tsmi != null && tsmi.CheckState == CheckState.Checked;

            // edit setting file
            CheckManager.CheckUseContrast(useContrast);

            // set form height
            AdjustHeight(useContrast);

            // set tabpage enable
            foreach (TabPage page in tabControl.TabPages)
            {
                ((TabPageTemplate) page.Controls[PageControlName]).UseContrast = useContrast;
            }
        }

        #region adjust heigth depend on whether use contrast

        private const int TallHeight = 137;
        private const int LowHeight = 86;

        public void AdjustHeight(bool useContrast)
        {
            var height = useContrast ? TallHeight : LowHeight;
            Height = height;
            tabControl.Height = height - 1;
        }

        #endregion

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (_canChangeVisible && e.Button == MouseButtons.Left)
            {
                Visible = !Visible;
            }
        }

        private void TabForm_Deactivate(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            _canChangeVisible = false;
            _timer.Start();
            Visible = false;
        }

        private void TabForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            UpdateTrackbarValue();
            RelocateForm();
        }

        private void RelocateForm(bool useCursorPos = true)
        {
            var screen = Screen.FromHandle(Handle);

            var left = screen.WorkingArea.X + screen.WorkingArea.Width - Width - 40;
            var leftByCursor = Cursor.Position.X - Width / 2;
            Left = left;
            if (!useCursorPos)
            {
                Left = Math.Min(left, leftByCursor);
            }

            Top = screen.WorkingArea.Y + screen.WorkingArea.Height - Height - 5;
            Activate();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTrackbarValue();
        }

        private void UpdateTrackbarValue()
        {
            // todo ddc/ci monitor has got some terrible delay
            ((TabPageTemplate) tabControl.SelectedTab?.Controls[PageControlName])?.UpdateTrackBars();
        }

        #region handle application start twice

        private const int WmCopydata = 0x004A;

        [DllImport("user32")]
        private static extern bool ChangeWindowMessageFilter(uint msg, int flags);

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WmCopydata:
                    var mystr = new MessageSender.CopyDataStruct();
                    var mytype = mystr.GetType();
                    mystr = (MessageSender.CopyDataStruct) m.GetLParam(mytype);
                    if (mystr.LpData == MessageSender.Msg)
                    {
                        notifyIcon.ShowBalloonTip(1500, "运行中", Application.ProductName + "已在运行", ToolTipIcon.Info);

                        UpdateTrackbarValue();
                        RelocateForm(false);

                        VisibleChanged -= TabForm_VisibleChanged;
                        Visible = true;
                        VisibleChanged += TabForm_VisibleChanged;
                    }

                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        #endregion

        #region global mouse wheel hook, directly set brightness

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            _mouseHook.Uninstall();
        }

        private void _mouseHook_MouseWheel(MouseHook.Msllhookstruct mouseStruct, out bool @continue)
        {
            if (!Visible)
            {
                @continue = true;
                return;
            }

            var delta = (short) (mouseStruct.mouseData >> 16);
            var tabTemplate = (TabPageTemplate) tabControl.SelectedTab.Controls[PageControlName];

            // call COM in global hook lead to disconnected context
            var thread = new Thread(new ThreadWrap(delta, tabTemplate).ThreadChild);
            thread.Start();

            @continue = false;
        }

        private class ThreadWrap
        {
            private const int WheelChange = 5;
            private readonly int _delta;
            private readonly TabPageTemplate _tabTemplate;

            public ThreadWrap(int delta, TabPageTemplate template)
            {
                _delta = delta;
                _tabTemplate = template;
            }

            public void ThreadChild()
            {
                var brightness = _tabTemplate.Brightness;
                var des = _delta > 0
                    ? Math.Min(brightness + WheelChange, _tabTemplate.BrightnessMax)
                    : Math.Max(brightness - WheelChange, _tabTemplate.BrightnessMin);
                _tabTemplate.Brightness = des;
            }
        }

        #endregion

        private void rescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _monitorsManager.RefreshMonitors();
        }
    }

    internal class CheckManager
    {
        private readonly TabForm _form;
        private readonly ToolStripMenuItem _autoStartMenuItem;
        private readonly ToolStripMenuItem _useContrastMenuItem;
        private readonly Regedit _regedit;

        public CheckManager(TabForm tabForm, ToolStrip menuStrip)
        {
            _form = tabForm;
            _regedit = new Regedit();
            _autoStartMenuItem = (ToolStripMenuItem) menuStrip.Items["autoStartToolStripMenuItem"];
            _useContrastMenuItem = (ToolStripMenuItem) menuStrip.Items["useContrastToolStripMenuItem"];
            LoadState();
        }

        private void LoadState()
        {
            _autoStartMenuItem.CheckStateChanged -= _form.autoStartToolStripMenuItem_CheckStateChanged;
            _useContrastMenuItem.CheckStateChanged -= _form.useContrastToolStripMenuItem_CheckStateChanged;

            _autoStartMenuItem.Checked = _regedit.IsAutoStart;
            _useContrastMenuItem.Checked = SettingManager.UseContrast;
            _form.AdjustHeight(SettingManager.UseContrast);

            _autoStartMenuItem.CheckStateChanged += _form.autoStartToolStripMenuItem_CheckStateChanged;
            _useContrastMenuItem.CheckStateChanged += _form.useContrastToolStripMenuItem_CheckStateChanged;
        }

        public void CheckAutoStart(bool enableAutoStart)
        {
            _regedit.IsAutoStart = enableAutoStart;
        }

        public static void CheckUseContrast(bool useContrast)
        {
            SettingManager.UseContrast = useContrast;
        }
    }

    internal class Regedit
    {
        private const string RegistryPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string SubPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private readonly RegistryKey _key = Registry.CurrentUser.OpenSubKey(SubPath, true);
        private readonly string _name = Application.ProductName;
        private bool _isAutoStart;

        public Regedit()
        {
            _isAutoStart = Registry.GetValue(RegistryPath, _name, null) != null;
        }

        public bool IsAutoStart
        {
            get { return _isAutoStart; }
            set
            {
                _isAutoStart = value;
                if (value)
                {
                    _key?.SetValue(_name, Application.ExecutablePath);
                }
                else
                {
                    _key?.DeleteValue(_name, false);
                }
            }
        }
    }

    internal static class SettingManager
    {
        public static bool UseContrast
        {
            get { return Settings.Default.use_contrast; }
            set
            {
                Settings.Default.use_contrast = value;
                Settings.Default.Save();
            }
        }
    }

    public class MonitorsManager
    {
        public delegate void AddMonitorDelegate(Monitor monitor);

        public delegate void ClearPagesDelegate();

        private readonly AddMonitorDelegate _addMonitorDelegate;
        private readonly ClearPagesDelegate _clearPagesDelegate;
        private readonly List<Monitor> _monitors = new List<Monitor>();

        public MonitorsManager(AddMonitorDelegate addMonitorDelegate, ClearPagesDelegate clearPagesDelegate)
        {
            _addMonitorDelegate = addMonitorDelegate;
            _clearPagesDelegate = clearPagesDelegate;
        }

        private void MapMonitorsToPages()
        {
            foreach (var monitor in _monitors)
            {
                _addMonitorDelegate(monitor);
            }
        }

        public void RefreshMonitors()
        {
            _clearPagesDelegate();
            _monitors.Clear();

            // todo mouse werid stuck
            IEnumerable<DdcCiMonitor> handles = null;
            var thread = new Thread(() => { handles = DdcCiMonitorManager.GetMonitorHandles(); });
            thread.Start();
            thread.Join();
            _monitors.AddRange(handles);

            // todo use a manager class
            _monitors.Add(new WmiMonitor(@"DISPLAY\SDC4C48\4&2e490a7&0&UID265988_0"));

            MapMonitorsToPages();
        }
    }
}