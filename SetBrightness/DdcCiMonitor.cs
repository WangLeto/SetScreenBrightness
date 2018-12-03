using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PhysicalMonitorHandle = System.IntPtr;

namespace SetBrightness
{
    internal partial class DdcCiMonitor : Monitor
    {
        // refer MCCS vcp codes: https://milek7.pl/ddcbacklight/mccs.pdf

        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool SetVCPFeature(
            PhysicalMonitorHandle hMonitor, byte bVcpCode, uint dwNewValue);

        [DllImport("Dxva2.dll")]
        private static extern bool GetMonitorCapabilities(
            PhysicalMonitorHandle hMonitor, out PdwMonitorCapabilitiesFlag pdwMonitorCapabilities,
            out PdwSupportedColorTemperaturesFlag pdwSupportedColorTemperatures);


        [DllImport("dxva2.dll")]
        public static extern bool SetMonitorContrast(IntPtr hMonitor, short brightness);

        [DllImport("dxva2.dll")]
        public static extern bool SetMonitorBrightness(IntPtr hMonitor, short brightness);

        private const byte LuminanceCode = 0x10;
        private const byte ContrastCode = 0x12;

        // https://docs.microsoft.com/en-us/windows/desktop/api/highlevelmonitorconfigurationapi/nf-highlevelmonitorconfigurationapi-getmonitorcapabilities
        // 是否支持亮度、对比度方法 ↑

        // https://docs.microsoft.com/en-us/windows/desktop/Monitor/using-the-high-level-monitor-configuration-functions#supported-functions
        // A monitor might not support all of the monitor configuration functions. To find out which functions a monitor supports, call GetMonitorCapabilities.

        // Todo
        private bool _isLowLevel;

        private readonly IntPtr _physicalMonitorHandle;

        public DdcCiMonitor(IntPtr physicalMonitorHandle)
        {
            _physicalMonitorHandle = physicalMonitorHandle;
            FigureOutInfo();
        }

        private void FigureOutInfo()
        {
            PdwMonitorCapabilitiesFlag highFlag;
            PdwSupportedColorTemperaturesFlag _;
            CanUse = GetMonitorCapabilities(_physicalMonitorHandle, out highFlag, out _);
            if (!CanUse)
            {
                return;
            }

            if (highFlag.HasFlag(PdwMonitorCapabilitiesFlag.McCapsBrightness))
            {
                if (highFlag.HasFlag(PdwMonitorCapabilitiesFlag.McCapsContrast))
                {
                    Debug.WriteLine("支持对比度");
                    SupportContrast = true;
                }

                return;
            }
            else
            {
                // todo: test low level capabilities
                CanUse = false;
            }
        }

        // https://docs.microsoft.com/en-us/windows/desktop/monitor/using-the-high-level-monitor-configuration-functions#continuous-monitor-settings
        // Most of the high-level monitor configuration functions control continuous monitor settings. For example, brightness and contrast are continuous settings.

        public override void SetBrightness(int brightness)
        {
            if (!CanUse)
            {
                Debug.WriteLine("不支持的设备");
                return;
            }

            RestrictValue(ref brightness);
            if (_isLowLevel)
            {
                LowLevelSetBrightness(brightness);
            }
            else
            {
                HighLevelSetBrightness(brightness);
            }
        }

        private void LowLevelSetBrightness(int brightness)
        {
            var succeed = SetVCPFeature(_physicalMonitorHandle, LuminanceCode, (byte) brightness);
            Debug.WriteLine("颜色修改：" + succeed);
        }

        private void HighLevelSetBrightness(int brightness)
        {
            SetMonitorBrightness(_physicalMonitorHandle, (short) brightness);
        }

        private static void RestrictValue(ref int value)
        {
            value = Math.Max(0, value);
            value = Math.Min(100, value);
        }

        public override int GetBrightness()
        {
            throw new NotImplementedException();
        }

        public override void SetContrast(int contrast)
        {
            throw new NotImplementedException();
        }

        public override int GetContrast()
        {
            throw new NotImplementedException();
        }

        [DllImport("Dxva2.dll")]
        public static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

        ~DdcCiMonitor()
        {
            DestroyPhysicalMonitor(_physicalMonitorHandle);
        }
    }
}