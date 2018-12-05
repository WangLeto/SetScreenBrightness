using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SetBrightness
{
    public partial class TabPageTemplate : UserControl
    {
        private readonly Monitor _monitor;
        private readonly MonitorsManager _monitorsManager;
        public readonly MonitorType MonitorType;

        public TabPageTemplate(Monitor monitor, string name, MonitorsManager monitorsManager)
        {
            InitializeComponent();
            _monitor = monitor;
            _monitorsManager = monitorsManager;
            Name = name;
            MonitorType = monitor.Type;
            PreWork();
        }

        private void PreWork()
        {
            if (!_monitor.SupportContrast)
            {
                contrastTrackbar.Enabled = false;

                var right = contrastNameLabel.Right;
                contrastNameLabel.Text += @"(不支持)";
                contrastNameLabel.Left = right - contrastNameLabel.Width;
                contrastNameLabel.ForeColor = Color.DarkGray;
                contrastLabel.ForeColor = Color.DarkGray;
            }

            brightLabel.DataBindings.Add("Text", brightTrackbar, "Value");
            contrastLabel.DataBindings.Add("Text", contrastTrackbar, "Value");

            brightTrackbar.ValueChanged += (sender, e) =>
            {
                var value = brightTrackbar.Value;
                new Thread(() => _monitor.SetBrightness(value)).Start();
            };
            contrastTrackbar.ValueChanged += (sender, e) =>
            {
                var value = contrastTrackbar.Value;
                new Thread(() => _monitor.SetContrast(value)).Start();
            };
        }

        #region use contrast ui modify

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

        #endregion

        public int Brightness
        {
            get { return brightTrackbar.Value; }
            set
            {
                if (brightTrackbar.Value == value)
                {
                    return;
                }

                brightTrackbar.Value = value;
                new Thread(() => { _monitor.SetBrightness(value); }).Start();
            }
        }

        // todo? read brightness bound
        public int BrightnessMax => 100;

        public int BrightnessMin => 0;

        public void UpdateValues()
        {
            try
            {
                brightTrackbar.Value = GetMonitorValueInThread(true);
                contrastTrackbar.Value = GetMonitorValueInThread(false);
            }
            catch (InvalidMonitorException e)
            {
                _monitorsManager.RefreshMonitors();
                Debug.WriteLine(e);
            }
        }

        private int GetMonitorValueInThread(bool isBrightness)
        {
            var value = 0;
            var thread = new Thread(() =>
            {
                value = isBrightness ? _monitor.GetBrightness() : _monitor.GetContrast();
            });
            thread.Start();
            thread.Join();
            return value;
        }
    }
}