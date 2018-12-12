using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace SetBrightness
{
    internal static class AllMonitorManager
    {
        public static List<Monitor> GetAllMonitors()
        {
            var monitorIds = MonitorName.GetAllMonitorNames();
            var monitors = new List<Monitor>(WmiMonitorManager.ListWmiMonitors());
            foreach (var monitor in monitors)
            {
                foreach (var wmiMonitorId in monitorIds)
                {
                    if (!IdMatch(wmiMonitorId.InstanceName, monitor.Id))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(wmiMonitorId.UserFriendlyName))
                    {
                        monitor.Name = wmiMonitorId.UserFriendlyName;
                    }

                    monitorIds.Remove(wmiMonitorId);
                    break;
                }
            }

            foreach (var monitor in DdcCiMonitorManager.GetMonitorHandles())
            {
                if (monitorIds.Count == 0)
                {
                    continue;
                }

                var monitorId = monitorIds[0];
                if (!string.IsNullOrEmpty(monitorId.UserFriendlyName))
                {
                    monitor.Name = monitorId.UserFriendlyName;
                }

                monitorIds.Remove(monitorId);
                monitors.Add(monitor);
                break;
            }

            return monitors;
        }

        private static bool IdMatch(string id, string id2)
        {
            return id.Contains(id2) || id2.Contains(id);
        }
    }

    internal static class MonitorName
    {
        // get all monitors' name

        public static List<WmiMonitorId> GetAllMonitorNames()
        {
            var monitorIds = new List<WmiMonitorId>();
            // get all monitors' name and instanceName
            WmiOperation(instance =>
            {
                // ManufacturerName + ProductCodeId is contained in deviceInstanceId == InstanceName
                // UserFriendlyName is the brand and serie name, possibly null
                var userFriendlyName = "";
                if (instance["UserFriendlyName"] != null)
                {
                    userFriendlyName = Uint16ArrayToString((ushort[]) instance["UserFriendlyName"]);
                }

                var monitor = new WmiMonitorId()
                    {InstanceName = (string) instance["InstanceName"], UserFriendlyName = userFriendlyName};
                monitorIds.Add(monitor);
            });
            return monitorIds;
        }

        public struct WmiMonitorId
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

        private static void WmiOperation(Action<ManagementObject> action)
        {
            using (var searcher = WmiMonitorManager.GetWmiSearcher("WmiMonitorID"))
            using (var instances = searcher.Get())
            {
                foreach (var instance in instances)
                {
                    action.Invoke((ManagementObject) instance);
                }
            }
        }
    }
}