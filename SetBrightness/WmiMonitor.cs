using System;
using System.Management;
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

        private readonly string _instanceId;

        private WmiMonitorBrightnessClass _wmiMonitorBrightness;

        public WmiMonitor(string instanceId)
        {
            Type = MonitorType.WmiMonitor;
            _instanceId = instanceId;
            SupportContrast = false;
        }

        private bool WmiOperation(string className, Action<ManagementObject> action)
        {
            var succeed = false;
            using (var searcher = GetWmiSearcher(className))
            using (var instances = searcher.Get())
            {
                foreach (var instance in instances)
                {
                    if (!RightDevice(instance))
                    {
                        continue;
                    }

                    succeed = true;
                    action.Invoke((ManagementObject) instance);
                }
            }

            return succeed;
        }

        public override void SetBrightness(int brightness)
        {
            var succeed = WmiOperation("WmiMonitorBrightnessMethods",
                instance => instance.InvokeMethod("WmiSetBrightness", new object[] {(uint) 2, (byte) brightness}));

            if (!succeed)
            {
                throw new WmiMonitorInvalidException();
            }
        }

        public static ManagementObjectSearcher GetWmiSearcher(string queryStr)
        {
            var scope = new ManagementScope("root\\WMI");
            var query = new SelectQuery(queryStr);

            return new ManagementObjectSearcher(scope, query);
        }

        public override int GetBrightness()
        {
            _wmiMonitorBrightness = GetBrightnessInfo();
            return _wmiMonitorBrightness.CurrentBrightness;
        }

        private WmiMonitorBrightnessClass GetBrightnessInfo()
        {
            var succeed = WmiOperation("WmiMonitorBrightness", instance =>
            {
                _wmiMonitorBrightness.Level = (byte[]) instance["Level"];
                _wmiMonitorBrightness.Active = (bool) instance["Active"];
                _wmiMonitorBrightness.Levels = (uint) instance["Levels"];
                _wmiMonitorBrightness.InstanceName = (string) instance["InstanceName"];
                _wmiMonitorBrightness.CurrentBrightness = (byte) instance["CurrentBrightness"];
            });

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
            return monitor.Type == Type && ((WmiMonitor) monitor)._instanceId.Equals(_instanceId);
        }

        public override bool IsValide()
        {
            return WmiOperation("WmiMonitorBrightness", a => { });
        }

        private bool RightDevice(ManagementBaseObject instance)
        {
            // LOL I don't care the real sequence
            var instanceName = (string) instance["InstanceName"];
            return _instanceId.Contains(instanceName) || instanceName.Contains(_instanceId);
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