using System.Runtime.InteropServices;

namespace SetBrightness
{
    internal class WmiMonitor : Monitor
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WmiMonitorBrightnessClass
        {
            public bool Active;
            public byte CurrentBrightness;
            public string InstanceName;
            public byte[] Level;
            public uint Levels;
        }

        private WmiMonitorBrightnessClass _wmiMonitorBrightness;

        public WmiMonitor(string instanceId, string name)
        {
            Type = MonitorType.WmiMonitor;
            Id = instanceId;
            Name = name;
            SupportContrast = false;
        }

        public override void SetBrightness(int brightness)
        {
            var succeed = WmiMonitorManager.WmiOperation("WmiMonitorBrightnessMethods",
                instance => instance.InvokeMethod("WmiSetBrightness", new object[] {(uint) 2, (byte) brightness}), Id);

            if (!succeed)
            {
                throw new WmiMonitorInvalidException();
            }
        }

        public override int GetBrightness()
        {
            _wmiMonitorBrightness = GetBrightnessInfo();
            return _wmiMonitorBrightness.CurrentBrightness;
        }

        private WmiMonitorBrightnessClass GetBrightnessInfo()
        {
            var succeed = WmiMonitorManager.WmiOperation("WmiMonitorBrightness", instance =>
            {
                _wmiMonitorBrightness.Level = (byte[]) instance["Level"];
                _wmiMonitorBrightness.Active = (bool) instance["Active"];
                _wmiMonitorBrightness.Levels = (uint) instance["Levels"];
                _wmiMonitorBrightness.InstanceName = (string) instance["InstanceName"];
                _wmiMonitorBrightness.CurrentBrightness = (byte) instance["CurrentBrightness"];
            }, Id);

            if (!succeed)
            {
                throw new WmiMonitorInvalidException();
            }

            return _wmiMonitorBrightness;
        }

        public override void SetContrast(int contrast)
        {
        }

        public override int GetContrast()
        {
            return 0;
        }

        public override bool IsSameMonitor(Monitor monitor)
        {
            return monitor.Type == Type && ((WmiMonitor) monitor).Id.Equals(Id);
        }

        public override bool IsValide()
        {
            return WmiMonitorManager.WmiOperation("WmiMonitorBrightness", a => { }, Id);
        }
    }

    public class WmiMonitorInvalidException : InvalidMonitorException
    {
        public override string ToString()
        {
            return "WmiMonitorInvalidException";
        }
    }
}