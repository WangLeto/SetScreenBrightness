using System;
using System.Collections.Generic;
using System.Management;
using System.Text.RegularExpressions;

namespace SetBrightness
{
    internal static class WmiMonitorManager
    {
        public static IEnumerable<Monitor> ListWmiMonitors()
        {
            var monitors = new List<Monitor>();
            WmiOperation("WmiMonitorBrightness",
                instance =>
                {
                    var id = (string) instance["InstanceName"];
                    monitors.Add(new WmiMonitor(id, ReadName(id)));
                }, "");
            return monitors;
        }

        private static readonly Regex Regex = new Regex(@"\\[a-z0-9A-Z]+\\");

        private static string ReadName(string id)
        {
            var matched = Regex.Match(id);
            var name = matched.ToString();
            return name.Substring(1, name.Length - 2);
        }

        public static bool WmiOperation(string className, Action<ManagementObject> action, string id)
        {
            var succeed = false;
            using (var searcher = GetWmiSearcher(className))
            using (var instances = searcher.Get())
            {
                foreach (var instance in instances)
                {
                    if (!MatchDevice(instance, id))
                    {
                        continue;
                    }

                    succeed = true;
                    action.Invoke((ManagementObject) instance);
                }
            }

            return succeed;
        }

        public static ManagementObjectSearcher GetWmiSearcher(string queryStr)
        {
            var scope = new ManagementScope("root\\WMI");
            var query = new SelectQuery(queryStr);

            return new ManagementObjectSearcher(scope, query);
        }


        private static bool MatchDevice(ManagementBaseObject instance, string id)
        {
            // LOL I don't care the real sequence
            var instanceName = (string) instance["InstanceName"];
            return id.Contains(instanceName) || instanceName.Contains(id);
        }
    }
}