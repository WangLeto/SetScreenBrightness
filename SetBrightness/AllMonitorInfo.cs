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
                foreach (var instance in instances)
                {
                    foreach (var property in instance.Properties)
                    {
                        Debug.WriteLine(property.Name + "\t\t" + property.Value);
                    }

                    var deviceId = (string) instance["DeviceID"];
                    var description = (string) instance["Description"];
                    Debug.WriteLine("deviceId:" + deviceId + "\tdes:" + description);
                    list.Add(new MonitorDescription {DeviceId = deviceId, Description = description});
                }
            }

            EnumerateMonitorDevices();

            return list;
        }

        public struct MonitorDescription
        {
            public string DeviceId;
            public string Description;
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

                    Debug.WriteLine("///////////////////////////\r\n" +
                                    "deviceInstanceId：" + deviceInstanceId + "\r\ndescription：" + monitor.DeviceString +
                                    "\r\nName：" + monitor.DeviceName +
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