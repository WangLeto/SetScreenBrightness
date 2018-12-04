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

        public TabPageTemplate(Monitor monitor, string name, MonitorsManager monitorsManager)
        {
            InitializeComponent();
            _monitor = monitor;
            _monitorsManager = monitorsManager;
            Name = name;
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

            brightTrackbar.ValueChanged += (sender, args) => Brightness = brightTrackbar.Value;
            contrastTrackbar.ValueChanged += (sender, args) => Contrast = contrastTrackbar.Value;
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
            get
            {
                try
                {
                    return _monitor.GetBrightness();
                }
                catch (InvalidMonitorException e)
                {
                    _monitorsManager.RefreshMonitors();
                    Debug.WriteLine(e);
                }

                return 0;
            }
            set
            {
                // windows form control thread-safe call
                var thread = new Thread(new SetTrackBarThreadSafe(value, this, brightTrackbar).Set);
                thread.Start();

                _monitor.SetBrightness(value);
            }
        }

        private class SetTrackBarThreadSafe
        {
            private readonly int _value;
            private readonly TabPageTemplate _tabPageTemplate;
            private readonly TrackBar _trackBar;

            public SetTrackBarThreadSafe(int value, TabPageTemplate tabPageTemplate, TrackBar trackBar)
            {
                _value = value;
                _tabPageTemplate = tabPageTemplate;
                _trackBar = trackBar;
            }

            private delegate void SetDelegate();

            public void Set()
            {
                if (_trackBar.InvokeRequired)
                {
                    var @delegate = new SetDelegate(Set);
                    _tabPageTemplate.Invoke(@delegate);
                }
                else
                {
                    if (_trackBar.Value == _value)
                    {
                        return;
                    }

                    _trackBar.Value = _value;
                }
            }
        }

        // todo? read brightness bound
        public int BrightnessMax => 100;

        public int BrightnessMin => 0;

        private int Contrast
        {
            get
            {
                try
                {
                    return _monitor.GetContrast();
                }
                catch (InvalidMonitorException e)
                {
                    _monitorsManager.RefreshMonitors();
                    Debug.WriteLine(e);
                }

                return 0;
            }
            set
            {
                var thread = new Thread(new SetTrackBarThreadSafe(value, this, contrastTrackbar).Set);
                thread.Start();

                _monitor.SetContrast(value);
            }
        }

        public void UpdateTrackBars()
        {
            brightTrackbar.Value = Brightness;
            contrastTrackbar.Value = Contrast;
        }
    }
}