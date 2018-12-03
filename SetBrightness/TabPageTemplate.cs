using System.Drawing;
using System.Windows.Forms;

namespace SetBrightness
{
    public partial class TabPageTemplate : UserControl
    {
        private readonly Monitor _monitor;

        public TabPageTemplate(Monitor monitor, string name)
        {
            InitializeComponent();
            _monitor = monitor;
            Name = name;
            PreWork();
        }

        private void PreWork()
        {
            if (!_monitor.SupportContrast)
            {
                contrastTrackbar.Enabled = false;

                var right = contrastNameLabel.Right;
                contrastNameLabel.Text += "(不支持)";
                contrastNameLabel.Left = right - contrastNameLabel.Width;
                contrastNameLabel.ForeColor = Color.DarkGray;
                contrastLabel.ForeColor = Color.DarkGray;
            }

            brightLabel.DataBindings.Add("Text", brightTrackbar, "Value");
            contrastLabel.DataBindings.Add("Text", contrastTrackbar, "Value");
        }

        private bool _useContrast;

        public bool UseContrast
        {
            get { return _useContrast; }
            set
            {
                if (!_monitor.SupportContrast)
                {
                    value = false;
                }

                contrastTrackbar.Enabled = value;
                _useContrast = value;
            }
        }

        public int Brightness
        {
            get { return _monitor.GetBrightness(); }
            set { _monitor.SetBrightness(value); }
        }

        public int Contrast
        {
            get { return _monitor.GetContrast(); }
            set { _monitor.SetContrast(value); }
        }
    }
}