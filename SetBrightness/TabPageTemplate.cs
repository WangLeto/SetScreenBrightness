using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetBrightness
{
    public partial class TabPageTemplate : UserControl
    {
        private readonly Monitor _monitor;
        private readonly MonitorsManager _monitorsManager;
        private readonly SetMonitorQueue _setQueue;
        public readonly MonitorType MonitorType;

        public TabPageTemplate(Monitor monitor, string name, MonitorsManager monitorsManager)
        {
            InitializeComponent();
            _monitor = monitor;
            _monitorsManager = monitorsManager;
            Name = name;
            MonitorType = monitor.Type;

            _setQueue = new SetMonitorQueue(_monitor);
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

            // todo use background work threads and make earlier thread cancelable
            brightTrackbar.ValueChanged += (sender, e) =>
            {
                var value = brightTrackbar.Value;
                new Thread(() => _setQueue.Add(value, true)).Start();
            };
            contrastTrackbar.ValueChanged += (sender, e) =>
            {
                var value = contrastTrackbar.Value;
                new Thread(() => _setQueue.Add(value, false)).Start();
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
                new Thread(() => { _setQueue.Add(value, true); }).Start();
            }
        }

        // todo? read brightness bound
        public int BrightnessMax => 100;

        public int BrightnessMin => 0;

        public async void UpdateValues()
        {
            try
            {
                await Task.WhenAll(
                    Task.Run(() => SafeSetTrackBar(brightTrackbar, _monitor.GetBrightness())),
                    Task.Run(() => SafeSetTrackBar(contrastTrackbar, _monitor.GetContrast())));
            }
            catch (InvalidMonitorException e)
            {
                _monitorsManager.RefreshMonitors();
                Debug.WriteLine(e);
            }
        }

        private void SafeSetTrackBar(TrackBar trackBar, int value)
        {
            if (trackBar.InvokeRequired)
            {
                Invoke(new Action<TrackBar, int>(SafeSetTrackBar), trackBar, value);
            }
            else
            {
                trackBar.Value = value;
            }
        }

        // kind like producer/customer model ?
        private class SetMonitorQueue
        {
            private readonly Queue<int> _brightnessQueue = new Queue<int>();
            private readonly Queue<int> _contrastQueue = new Queue<int>();
            private readonly object _brightnessLock = new object();
            private readonly object _contrastLock = new object();
            private readonly Monitor _monitor;

            public SetMonitorQueue(Monitor monitor)
            {
                _monitor = monitor;
            }

            public void Add(int value, bool isBrightness)
            {
                var @lock = isBrightness ? _brightnessLock : _contrastLock;
                var queue = isBrightness ? _brightnessQueue : _contrastQueue;
                var action = isBrightness ? (Action<int>) _monitor.SetBrightness : _monitor.SetContrast;
                if (queue.Count > 0)
                {
                    queue.Clear();
                }

                queue.Enqueue(value);

                lock (@lock)
                {
                    if (queue.Count == 0)
                    {
                        return;
                    }

                    var des = queue.Dequeue();
                    action.Invoke(des);
                }
            }
        }
    }
}