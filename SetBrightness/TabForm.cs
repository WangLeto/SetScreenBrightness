using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Timer = System.Timers.Timer;

namespace SetBrightness
{
    public partial class TabForm : Form
    {
        #region declaration

        private const string PageControlName = nameof(TabPageTemplate);
        private readonly CheckManager _checkManager;

        private readonly Timer _timer = new Timer(150);

        private bool _canChangeVisible = true;

        private readonly MouseHook _mouseHook = new MouseHook();
        private readonly MonitorsManager _monitorsManager;
        public const string WindowName = "亮度调节";
        private readonly KeyboardHook _hook = new KeyboardHook();

        #endregion

        public TabForm()
        {
            InitializeComponent();

            Text = notifyIcon.Text = WindowName;
            _monitorsManager = new MonitorsManager(AddPage, CleanPages);

            _checkManager = new CheckManager(contextMenuStrip);
            AdjustHeight(SettingManager.UseContrast);
            if (SettingManager.UseHotKey)
            {
                hotKeyWinAltBToolStripMenuItem.Checked = RegistryHotKey();
            }

            _timer.Elapsed += (sender, args) =>
            {
                _canChangeVisible = true;
                _timer.Stop();
            };

            ChangeWindowMessageFilter(WmCopydata, 1);
            tabControl.ControlAdded += (sender, args) => loadingTipLabel.Hide();

            _mouseHook.MouseWheel += _mouseHook_MouseWheel;
            _mouseHook.MouseLDown += _mouseHook_MouseLDown;
            _mouseHook.MouseRDown += _mouseHook_MouseRDown;
            _mouseHook.Install();
            Application.ApplicationExit += Application_ApplicationExit;
            _monitorsManager.RefreshMonitors();

            _hook.KeyPressed += (sender, args) => { ShowFormFixed(); };
        }

        private void _mouseHook_MouseLDown(MouseHook.Msllhookstruct mouseStruct, out bool goOn)
        {
            goOn = true;
            
            if (!Visible || Bounds.Contains(Cursor.Position))
            {
                return;
            }

            _canChangeVisible = false;
            _timer.Start();
            Visible = false;
        }

        private void _mouseHook_MouseRDown(MouseHook.Msllhookstruct mouseStruct, out bool goOn)
        {
            if (!Visible || !tabControl.SelectedTab.Bounds.Contains(PointToClient(Cursor.Position)))
            {
                goOn = true;
                return;
            }

            goOn = false;
            ((TabPageTemplate) tabControl.SelectedTab.Controls[PageControlName]).ShowTabPageContextMenuStrip();
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void AddPage(Monitor monitor)
        {
            if (!IsNewMonitor(monitor))
            {
                return;
            }

            var page = new TabPage(
                string.IsNullOrWhiteSpace(monitor.UserDefineName) ? monitor.Name : monitor.UserDefineName);
            var template = new TabPageTemplate(monitor, PageControlName, _monitorsManager, tabControl.TabCount + 1);
            template.ChangeMonitorNameEventHandler += (sender, args) => { UpdatePageName(args.NewName, args.Uuid); };
            template.UpdateValues();
            page.Controls.Add(template);
            tabControl.TabPages.Add(page);
            template.LoadUserName();
        }

        private bool IsNewMonitor(Monitor monitor)
        {
            var pages = tabControl.TabPages;
            foreach (TabPage page in pages)
            {
                if (((TabPageTemplate) page.Controls[PageControlName]).OwnTheMonitor(monitor))
                {
                    return false;
                }
            }

            return true;
        }

        private void CleanPages()
        {
            var pages = tabControl.TabPages;

            foreach (TabPage page in pages)
            {
                if (!((TabPageTemplate) page.Controls[PageControlName]).IsValide())
                {
                    tabControl.TabPages.Remove(page);
                }
            }

            if (tabControl.TabCount == 0)
            {
                loadingTipLabel.Show();
            }
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

        private void hotKeyWinAltBToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
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

        #region adjust heigth depend on whether use contrast

        private const int TallHeight = 137;
        private const int LowHeight = 86;

        private void AdjustHeight(bool useContrast)
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

        private void TabForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            SelectPreferTab();
            UpdateTrackbarValue();
            RelocateForm();
            Activate();
        }

        private void SelectPreferTab()
        {
            var prefer = SettingManager.PreferMonitor;
            if (prefer == "")
            {
                return;
            }

            foreach (TabPage page in tabControl.TabPages)
            {
                if (!((TabPageTemplate) page.Controls[PageControlName]).IsPreferred(prefer))
                {
                    continue;
                }

                tabControl.SelectTab(page);
                return;
            }
        }

        private void RelocateForm(bool useCursorPos = true)
        {
            var screen = Screen.FromHandle(Handle);

            Left = screen.WorkingArea.X + screen.WorkingArea.Width - Width - 40;
            Top = screen.WorkingArea.Y + screen.WorkingArea.Height - Height - 5;
            if (useCursorPos)
            {
                Left = Cursor.Position.X;
                var leftOff = screen.Bounds.X + screen.Bounds.Width - Width - 50;
                Left = Math.Min(leftOff, Left);

                Top = Cursor.Position.Y;
                var topOff = screen.Bounds.Y + screen.Bounds.Height - Height - 20;
                Top = Math.Min(topOff, Top);
            }

            Activate();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTrackbarValue();
        }

        private void UpdateTrackbarValue()
        {
            var selectedTab = tabControl.SelectedTab;
            if (selectedTab == null)
            {
                return;
            }

            var tabPageTemplate = selectedTab.Controls.Find(PageControlName, true);
            if (tabPageTemplate.Length > 0)
            {
                ((TabPageTemplate) tabPageTemplate[0]).UpdateValues();
            }
        }

        #region handle application start twice

        private const int WmCopydata = 0x004A;
        private const int WmDisplaychange = 0x007E;

        [DllImport("user32")]
        private static extern bool ChangeWindowMessageFilter(uint msg, int flags);

        protected override void DefWndProc(ref Message m)
        {
            base.DefWndProc(ref m);
            switch (m.Msg)
            {
                case WmCopydata:
                {
                    var mystr = new MessageSender.CopyDataStruct();
                    var mytype = mystr.GetType();
                    mystr = (MessageSender.CopyDataStruct) m.GetLParam(mytype);
                    if (mystr.LpData == MessageSender.Msg)
                    {
                        notifyIcon.ShowBalloonTip(1500, "运行中", Application.ProductName + "已在运行", ToolTipIcon.Info);
                        ShowFormFixed();
                    }

                    break;
                }
                case WmDisplaychange:
                    Action<bool, NotifyIcon> action = _monitorsManager.RefreshMonitors;
                    DelayTasks.OrderTask(action, 2);
                    DelayTasks.OrderTask(action, 4);
                    break;
            }
        }

        private class DelayTasks
        {
            private static readonly Queue<object[]> TaskQueue = new Queue<object[]>();
            private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

            public static void OrderTask(Action<bool, NotifyIcon> action, int second)
            {
                TaskQueue.Enqueue(new object[] {action, second * 1000});
                Work();
            }

            private static async void Work()
            {
                var taskDelay = TaskQueue.Dequeue();
                await SemaphoreSlim.WaitAsync();
                try
                {
                    var task = (Action<bool, NotifyIcon>) taskDelay[0];
                    var delay = (int) taskDelay[1];
                    await Task.Delay(delay);
                    task.Invoke(false, null);
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
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
            var brightness = tabTemplate.Brightness;
            const int wheelChange = 5;
            tabTemplate.Brightness = delta > 0
                ? Math.Min(brightness + wheelChange, tabTemplate.BrightnessMax)
                : Math.Max(brightness - wheelChange, tabTemplate.BrightnessMin);

            @continue = false;
        }

        #endregion

        private void rescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _monitorsManager.RefreshMonitors(true, notifyIcon);
        }

        private bool RegistryHotKey()
        {
            try
            {
                _hook.RegisterHotKey(SetBrightness.ModifierKeys.Control | SetBrightness.ModifierKeys.Win, Keys.B);
                SettingManager.UseHotKey = true;
                return true;
            }
            catch (InvalidOperationException)
            {
                notifyIcon?.ShowBalloonTip(1000, "错误", "快捷键可能已被占用", ToolTipIcon.Error);
                SettingManager.UseHotKey = false;
                return false;
            }
        }

        // global hook cannot monitor administrator application
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

        private void TabForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Escape))
            {
                Hide();
            }
        }

        /// <summary>
        /// 显示窗口，不使用鼠标位置
        /// </summary>
        private void ShowFormFixed()
        {
            VisibleChanged -= TabForm_VisibleChanged;
            SelectPreferTab();
            UpdateTrackbarValue();
            RelocateForm(false);
            Visible = true;
            Activate();
            VisibleChanged += TabForm_VisibleChanged;
        }

        #region 右键菜单 check

        private void hotKeyWinAltBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            var use = tsmi != null && tsmi.CheckState == CheckState.Checked;
            if (use)
            {
                tsmi.Checked = RegistryHotKey();
            }
            else
            {
                _hook.UninstallHotkeys();
                SettingManager.UseHotKey = false;
                if (tsmi != null) tsmi.Checked = false;
            }
        }

        private void useContrastToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void autoStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            var enableAutoStart = tsmi != null && tsmi.CheckState == CheckState.Checked;
            _checkManager.CheckAutoStart(enableAutoStart);

            if (enableAutoStart)
            {
                notifyIcon.ShowBalloonTip(3500, "设置成功", "如果移动程序位置，需要重新设置", ToolTipIcon.Info);
            }
        }

        #endregion

        private void homepageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/WangLeto/SetScreenBrightness");
        }

        /// <summary>
        /// 用于用户为显示器改名后及时更新
        /// </summary>
        private void UpdatePageName(string name, int uuid)
        {
            foreach (TabPage page in tabControl.TabPages)
            {
                var template = (TabPageTemplate) page.Controls[PageControlName];
                if (template.Uuid == uuid)
                {
                    page.Text = name;
                }
            }
        }
    }

    internal class CheckManager
    {
        private readonly ToolStripMenuItem _autoStartMenuItem;
        private readonly ToolStripMenuItem _useContrastMenuItem;
        private readonly ToolStripMenuItem _hotkeyWinAltBToolStripMenuItem;
        private readonly Regedit _regedit;

        public CheckManager(ToolStrip menuStrip)
        {
            _regedit = new Regedit();
            _autoStartMenuItem = (ToolStripMenuItem) menuStrip.Items["autoStartToolStripMenuItem"];
            _useContrastMenuItem = (ToolStripMenuItem) menuStrip.Items["useContrastToolStripMenuItem"];
            _hotkeyWinAltBToolStripMenuItem = (ToolStripMenuItem) menuStrip.Items["hotKeyWinAltBToolStripMenuItem"];
            LoadState();
        }

        private void LoadState()
        {
            _autoStartMenuItem.Checked = _regedit.IsAutoStart;
            _useContrastMenuItem.Checked = SettingManager.UseContrast;
            _hotkeyWinAltBToolStripMenuItem.Checked = SettingManager.UseHotKey;
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

    public class MonitorsManager
    {
        private readonly Action<Monitor> _addMonitorAction;
        private readonly Action _cleanAction;
        private readonly List<Monitor> _monitors = new List<Monitor>();

        public MonitorsManager(Action<Monitor> addMonitorAction, Action cleanAction)
        {
            _addMonitorAction = addMonitorAction;
            _cleanAction = cleanAction;
        }

        private void MapMonitorsToPages()
        {
            _cleanAction.Invoke();
            foreach (var monitor in _monitors)
            {
                _addMonitorAction(monitor);
            }
        }

        public async void RefreshMonitors(bool showNotify = false, NotifyIcon notifyIcon = null)
        {
            _monitors.Clear();

            await Task.Run(() => _monitors.AddRange(AllMonitorManager.GetAllMonitors()));

            MapMonitorsToPages();
            if (showNotify)
            {
                notifyIcon?.ShowBalloonTip(1500, "完成", "已经重新扫描屏幕", ToolTipIcon.Info);
            }
        }

        public static void SetPreferMonitor(string id)
        {
            SettingManager.PreferMonitor = id;
        }

        public static string PreferMonitorId()
        {
            return SettingManager.PreferMonitor;
        }
    }
}