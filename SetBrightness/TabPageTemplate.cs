using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetBrightness
{
    public partial class TabPageTemplate : UserControl
    {
        private Monitor _monitor;

        public TabPageTemplate(Monitor monitor)
        {
            InitializeComponent();
            _monitor = monitor;
        }
    }
}