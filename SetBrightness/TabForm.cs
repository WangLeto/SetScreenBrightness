using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using SetBrightness.Properties;

namespace SetBrightness
{
    public partial class TabForm : Form
    {
        private string _monitorName = nameof(TabPageTemplate);
        private readonly CheckManager _checkManager;

        public TabForm()
        {
            InitializeComponent();
            AddMonitor(new WmiMonitor(""));
            _checkManager = new CheckManager(this, contextMenuStrip);
        }

        private void AddMonitor(Monitor monitor)
        {
            var page = new TabPage(monitor.Name);
            page.Controls.Add(new TabPageTemplate(monitor, _monitorName));
            tabControl.TabPages.Add(page);
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
            Close();
        }

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

            CheckManager.CheckUseContrast(useContrast);

            foreach (TabPage page in tabControl.TabPages)
            {
                ((TabPageTemplate) page.Controls[_monitorName]).UseContrast = useContrast;
            }
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
}