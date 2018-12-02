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
            tabControl.TabPages.Add("1st");
        }
    }
}
