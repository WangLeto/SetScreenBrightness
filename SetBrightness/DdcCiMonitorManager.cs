using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SetBrightness
{
    // DDC/CI：https://docs.microsoft.com/en-us/windows/desktop/Monitor/monitor-configuration
    class DdcCiMonitorManager
    {
        [DllImport("User32.dll")]
        private static extern bool EnumDisplayMonitors(
            IntPtr hdc,
            IntPtr lprcClip,
            MonitorEnumProc lpfnEnum,
            IntPtr dwData);

        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool MonitorEnumProc(
            IntPtr hMonitor,
            IntPtr hdcMonitor,
            ref Rect lprcMonitor,
            IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct Monitorinfoex
        {
            public uint cbSize;
            public Rect rcMonitor;
            public Rect rcWork;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        private static readonly List<DdcCiMonitor> DdcCiMonitors = new List<DdcCiMonitor>();

        /// <summary>
        /// 使用 ddc/ic 协议管理的显示器句柄
        /// </summary>
        /// <returns></returns>
        public static List<DdcCiMonitor> GetMonitorHandles()
        {
            if (!EnumDisplayMonitors(
                IntPtr.Zero,
                IntPtr.Zero,
                MonitorEnumRroc,
                IntPtr.Zero))
            {
                Debug.WriteLine("EnumDisplayMonitors Fails");
            }

            return DdcCiMonitors;
        }

        private static bool MonitorEnumRroc(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
        {
            // https://docs.microsoft.com/en-us/windows/desktop/Monitor/using-the-high-level-monitor-configuration-functions#enumerating-physical-monitors
            // an HMONITOR handle can be associated with more than one physical monitor.
            // To configure the settings on a monitor, the application must get a unique handle to the physical monitor
            // by calling GetPhysicalMonitorsFromHMONITOR.

            // hMonitor 表示逻辑显示器句柄，一个 hMonitor 多个 physical displays：显示设置中的复制显示器

            // todo
            DdcCiMonitors.Add(new DdcCiMonitor(hMonitor));
            return true;
        }

        public static IntPtr[] GetPhysicalMonitorHandle(IntPtr hMonitor)
        {
            return new IntPtr[0];
        }
    }
}