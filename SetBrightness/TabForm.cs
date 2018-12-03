using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetBrightness
{
    public partial class TabForm : Form
    {
        public TabForm()
        {
            InitializeComponent();
            AddMonitor(new WmiMonitor(""));
        }

        private void AddMonitor(Monitor monitor)
        {
            var page = new TabPage(monitor.Name);
            page.Controls.Add(new TabPageTemplate(monitor));
            tabControl.TabPages.Add(page);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.FromArgb(214, 214, 214),
                ButtonBorderStyle.Solid);
        }
    }
}