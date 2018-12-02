using System;
using System.Management;
using System.Runtime.InteropServices;

namespace SetBrightness
{
    class WmiMonitor : Monitor
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WmiMonitorBrightnessClass
        {
            public bool Active;
            public byte CurrentBrightness;
            public string InstanceName;
            public byte[] Level;
            public uint Levels;
        }

        private readonly string _instanceId;

        public WmiMonitor(string instanceId)
        {
            _instanceId = instanceId;
            SupportContrast = false;
        }

        public override void SetBrightness(int brightness)
        {
            using (ManagementObjectSearcher searcher = GetBrightnessSearcher("WmiMonitorBrightnessMethods"))
            using (ManagementObjectCollection instances = searcher.Get())
            {
                foreach (var instance in instances)
                {
                    if (!RightDevice(instance))
                    {
                        continue;
                    }

                    ((ManagementObject) instance).InvokeMethod("WmiSetBrightness", new object[]
                    {
                        (uint) 2, (byte) brightness
                    });
                }
            }
        }

        private ManagementObjectSearcher GetBrightnessSearcher(string queryStr)
        {
            var scope = new ManagementScope("root\\WMI");
            var query = new SelectQuery(queryStr);

            return new ManagementObjectSearcher(scope, query);
        }

        public override int GetBrightness(int brightness)
        {
            using (var searcher = GetBrightnessSearcher("WmiMonitorBrightnessMethods"))
            using (var instances = searcher.Get())
            {
                foreach (var instance in instances)
                {
                    if (RightDevice(instance))
                    {
                        return (int) instance["CurrentBrightness"];
                    }
                }
            }

            return -1;
        }

        public WmiMonitorBrightnessClass GetBrightnessInfo()
        {
            WmiMonitorBrightnessClass @class = new WmiMonitorBrightnessClass();
            using (var searcher = GetBrightnessSearcher("WmiMonitorBrightness"))
            using (var instances = searcher.Get())
            {
                foreach (var instance in instances)
                {
                    if (!RightDevice(instance))
                    {
                        continue;
                    }

                    @class.Active = (bool) instance["Active"];
                    @class.CurrentBrightness = (byte) instance["CurrentBrightness"];
                    @class.InstanceName = (string) instance["InstanceName"];
                    @class.Level = (byte[]) instance["Level"];
                    @class.Levels = (uint) instance["Levels"];
                }
            }

            return @class;
        }

        public override void SetContrast(int contrast)
        {
        }

        public override int GetContrast(int contrast)
        {
            return -1;
        }

        private bool RightDevice(ManagementBaseObject instance)
        {
            // InstanceName         DISPLAY\SDC4C48\4&2e490a7&0&UID265988_0
            // deviceInstanceId     DISPLAY\SDC4C48\4&2e490a7&0&UID265988
            string instanceName = (string) instance["InstanceName"];
            return _instanceId.Contains(instanceName);
        }
    }
}