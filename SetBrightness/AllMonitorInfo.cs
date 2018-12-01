using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SetBrightness
{
    partial class AllMonitorInfo
    {
        public List<MonitorDescription> GetInfos()
        {
            var list = new List<MonitorDescription>();
            using (var obs = new ManagementClass("Win32_DesktopMonitor"))
            using (var instances = obs.GetInstances())
            {
                foreach (ManagementObject instance in instances)
                {
                    foreach (var property in instance.Properties)
                    {
//                        Console.WriteLine(property.Name + "\t\t" + property.Value);
                    }

                    var deviceId = (string) instance["DeviceID"];
                    var description = (string) instance["Description"];
                    list.Add(new MonitorDescription {DeviceId = deviceId, Description = description});
//                    foreach (var property in instance.Properties)
//                    {
//                        Console.WriteLine(property.Name + "\t\t\t" + property.Value);
//                    }
                }
            }

            Console.WriteLine(list.Count + "!!!");
            EnumerateMonitorDevices();

            return list;
        }

        public struct MonitorDescription
        {
            public string DeviceId;
            public string Description;
        }

        public static bool SetBrightness(string deviceInstanceId, int brightness, int timeout = int.MaxValue)
        {
            using (var searcher = GetSearcher("WmiMonitorBrightnessMethods"))
            using (var instances = searcher.Get())
            {
                foreach (ManagementObject instance in instances)
                {
                    var instanceName = (string) instance["InstanceName"];
                    if (instanceName.StartsWith(deviceInstanceId, StringComparison.OrdinalIgnoreCase))
                    {
                        object result = instance.InvokeMethod("WmiSetBrightness",
                            new object[] {(uint) timeout, (byte) brightness});

                        var isSuccess = (result == null); // Return value will be null if succeeded.
                        if (!isSuccess)
                        {
                            var errorCode = (uint) result;
                            isSuccess = (errorCode == 0);
                            if (!isSuccess)
                            {
                                Debug.WriteLine($"Failed to set brightness. ({errorCode})");
                            }
                        }

                        return isSuccess;
                    }
                }

                return false;
            }
        }

        private static ManagementObjectSearcher GetSearcher(string @class)
        {
            return new ManagementObjectSearcher(new ManagementScope(@"root\wmi"), new SelectQuery(@class));
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DisplayDevice
        {
            public uint cb;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public readonly string DeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public readonly string DeviceString;

            public readonly DisplayDeviceFlag StateFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public readonly string DeviceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public readonly string DeviceKey;
        }

        [DllImport("User32.dll", EntryPoint = "EnumDisplayDevicesA")]
        private static extern bool EnumDisplayDevices(
            string lpDevice,
            uint iDevNum,
            ref DisplayDevice lpDisplayDevice,
            uint dwFlags);


        private const uint EddGetDeviceInterfaceName = 0x00000001;


        private static bool TryGetDisplayIndex(string device, out byte index)
        {
            Regex displayPattern = new Regex(@"DISPLAY(?<index>\d{1,2})$", RegexOptions.Compiled);
            var match = displayPattern.Match(device.Trim());
            if (!match.Success)
            {
                index = 0;
                return false;
            }

            index = byte.Parse(match.Groups["index"].Value);
            return true;
        }

        // https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-enumdisplaydevicesa#remarks
        // 获取全部物理显示器设备
        public void EnumerateMonitorDevices()
        {
            var size = (uint) Marshal.SizeOf<DisplayDevice>();
            var display = new DisplayDevice {cb = size};
            var monitor = new DisplayDevice {cb = size};

            for (uint i = 0; EnumDisplayDevices(null, i, ref display, EddGetDeviceInterfaceName); i++)
            {
                if (display.StateFlags.HasFlag(DisplayDeviceFlag.DisplayDeviceMirroringDriver))
                {
                    continue;
                }

                byte displayIndex;
                if (!TryGetDisplayIndex(display.DeviceName, out displayIndex))
                {
                    continue;
                }

                for (uint j = 0;
                    EnumDisplayDevices(display.DeviceName, j, ref monitor, EddGetDeviceInterfaceName);
                    j++)
                {
                    var deviceInstanceId = GetDeviceInstanceId(monitor.DeviceID);
                    var isActive = monitor.StateFlags.HasFlag(DisplayDeviceFlag.DisplayDeviceActive);
                    if (!isActive)
                    {
                        continue;
                    }

                    Debug.WriteLine(
                        "deviceInstanceId：" + deviceInstanceId + "\r\ndescription：" + monitor.DeviceString +
                        "\r\ndisplayIndex：" + displayIndex);
                }
            }
        }

        private static string GetDeviceInstanceId(string deviceId)
        {
            var index = deviceId.IndexOf("DISPLAY", StringComparison.Ordinal);
            if (index < 0)
                return null;

            var fields = deviceId.Substring(index).Split('#');
            if (fields.Length < 3)
                return null;

            return string.Join(@"\", fields.Take(3));
        }
    }
}