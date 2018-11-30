using System;
using System.Runtime.InteropServices;
using PhysicalMonitorHandle = System.IntPtr;

namespace SetBrightness
{
    class DdcCiMonitor : Monitor
    {
        // refer MCCS vcp codes: https://milek7.pl/ddcbacklight/mccs.pdf

        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetVCPFeature(
            PhysicalMonitorHandle hMonitor,
            byte bVCPCode,
            uint dwNewValue);

        private static byte LuminanceCode = 0x10;
        private static byte Contrastode = 0x12;

        // https://docs.microsoft.com/en-us/windows/desktop/api/highlevelmonitorconfigurationapi/nf-highlevelmonitorconfigurationapi-getmonitorcapabilities
        // 是否支持亮度、对比度方法 ↑

        // https://docs.microsoft.com/en-us/windows/desktop/Monitor/using-the-high-level-monitor-configuration-functions#supported-functions
        // A monitor might not support all of the monitor configuration functions. To find out which functions a monitor supports, call GetMonitorCapabilities.

        // Todo
        private bool _isLowLevel;

        private IntPtr _physicalMonitorHandle;

        public DdcCiMonitor(IntPtr physicalMonitorHandle)
        {
            _physicalMonitorHandle = physicalMonitorHandle;
        }


        public override void SetBrightness(int brightness)
        {
            var succeed = SetVCPFeature(_physicalMonitorHandle, LuminanceCode, (byte)brightness);
            Console.WriteLine(succeed);
        }

        public override int GetBrightness(int brightness)
        {
            throw new NotImplementedException();
        }

        public int CurrentBrightness
        {
            // TODO
            get { return 1; }
        }
    }
}