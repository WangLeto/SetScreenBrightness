using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace SetBrightness
{
    class WmiMonitor : Monitor
    {
        private readonly string _instanceId;

        public WmiMonitor(string instanceId)
        {
            _instanceId = instanceId;
            SupportContrast = false;
        }

        public override void SetBrightness(int brightness)
        {
            using (ManagementObjectSearcher searcher = GetBrightnessSearcher())
            using (ManagementObjectCollection instances = searcher.Get())
            {
                foreach (var instance in instances)
                {
                    // todo 设备验证
                    //  var instanceName = instance["InstanceName"];
                    //  if (!_instanceId.Contains((string) instanceName))
                    //  {
                    //      continue;
                    //  }

                    ((ManagementObject) instance).InvokeMethod("WmiSetBrightness", new object[]
                    {
                        (uint) 2, (byte) brightness
                    });
                    instance.Dispose();
                }
            }
        }
        
        private ManagementObjectSearcher GetBrightnessSearcher()
        {
            var scope = new ManagementScope("root\\WMI");
            var query = new SelectQuery("WmiMonitorBrightnessMethods");

            return new ManagementObjectSearcher(scope, query);
        }

        public override int GetBrightness(int brightness)
        {
            using (var searcher = GetBrightnessSearcher())
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

        private bool RightDevice(ManagementBaseObject instance)
        {
            // InstanceName         DISPLAY\SDC4C48\4&2e490a7&0&UID265988_0
            // deviceInstanceId     DISPLAY\SDC4C48\4&2e490a7&0&UID265988
            string instanceName = (string) instance["InstanceName"];
            return _instanceId.Contains(instanceName);
        }
    }
}