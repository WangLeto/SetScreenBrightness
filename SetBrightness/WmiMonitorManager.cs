using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SetBrightness
{
    class WmiMonitorManager
    {
        // get all monitors' name
        public void EnumMonitor()
        {
            // get all monitors' name and instanceName
            WmiOperation("WmiMonitorID", instance =>
            {
                // ManufacturerName + ProductCodeId is contained in deviceInstanceId == InstanceName
                // UserFriendlyName is the brand and serie name, possibly null
                var instanceName = (string) instance["InstanceName"];
                var userFriendlyName = "";
                if (instance["UserFriendlyName"] != null)
                {
                    userFriendlyName = Uint16ArrayToString((ushort[]) instance["UserFriendlyName"]);
                }

                var monitor = new WmiMonitorId()
                    {InstanceName = (string) instance["InstanceName"], UserFriendlyName = userFriendlyName};
            });
        }

        struct WmiMonitorId
        {
            public string InstanceName;
            public string UserFriendlyName;
        }

        private static string Uint16ArrayToString(IEnumerable<ushort> arr)
        {
            var builder = new StringBuilder();
            foreach (var @ushort in arr)
            {
                if (@ushort == 0)
                {
                    break;
                }

                builder.Append((char) @ushort);
            }

            return builder.ToString();
        }

        private static bool WmiOperation(string className, Action<ManagementObject> action)
        {
            var succeed = false;
            using (var searcher = WmiMonitor.GetWmiSearcher("WmiMonitorID"))
            using (var instances = searcher.Get())
            {
                foreach (var instance in instances)
                {
                    succeed = true;
                    action.Invoke((ManagementObject) instance);
                }
            }

            return succeed;
        }
    }
}