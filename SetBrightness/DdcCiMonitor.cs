using System;
using System.Runtime.InteropServices;
using PhysicalMonitorHandle = System.IntPtr;

namespace SetBrightness
{
    internal partial class DdcCiMonitor : Monitor
    {
        // refer MCCS vcp codes: https://milek7.pl/ddcbacklight/mccs.pdf

        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool SetVCPFeature(PhysicalMonitorHandle hMonitor, byte bVcpCode, uint dwNewValue);

        [DllImport("Dxva2.dll")]
        private static extern bool GetMonitorCapabilities(PhysicalMonitorHandle hMonitor,
            out PdwMonitorCapabilitiesFlag pdwMonitorCapabilities,
            out PdwSupportedColorTemperaturesFlag pdwSupportedColorTemperatures);

        [DllImport("Dxva2.dll")]
        private static extern bool GetVCPFeatureAndVCPFeatureReply(PhysicalMonitorHandle hMonitor, byte bVcpCode,
            out LpmcVcpCodeType pvct, out uint pdwCurrentValue, out uint pdwMaximumValue);

        private enum LpmcVcpCodeType
        {
            McMomentary,
            McSetParameter
        }

        private const byte VcpLuminanceCode = 0x10;
        private const byte VcpContrastCode = 0x12;

        private bool _isLowLevel;

        private readonly PhysicalMonitorHandle _physicalMonitorHandle;

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
                if (!highFlag.HasFlag(PdwMonitorCapabilitiesFlag.McCapsContrast))
                {
                    return;
                }

                SupportContrast = true;
            }
            else
            {
                CanUse = TestLowLevelCapabilities();
            }
        }

        private bool TestLowLevelCapabilities()
        {
            // todo test low level
            return false;
        }

        private delegate bool NativeHighLevelGet(PhysicalMonitorHandle hMonitor,
            ref short min, ref short current, ref short max);

        [DllImport("dxva2.dll")]
        private static extern bool GetMonitorBrightness(PhysicalMonitorHandle hMonitor,
            ref short pdwMinimumBrightness, ref short pdwCurrentBrightness, ref short pdwMaximumBrightness);

        [DllImport("dxva2.dll")]
        private static extern bool SetMonitorBrightness(PhysicalMonitorHandle hMonitor, short brightness);

        [DllImport("dxva2.dll")]
        private static extern bool GetMonitorContrast(PhysicalMonitorHandle hMonitor, ref short pdwMinimumContrast,
            ref short pdwCurrentContrast, ref short pdwMaximumContrast);

        [DllImport("dxva2.dll")]
        private static extern bool SetMonitorContrast(PhysicalMonitorHandle hMonitor, short brightness);

        private static void RestrictValue(ref int value)
        {
            value = Math.Max(0, value);
            value = Math.Min(100, value);
        }

        public override void SetBrightness(int brightness)
        {
            RestrictValue(ref brightness);
            if (_isLowLevel)
            {
                SetVCPFeature(_physicalMonitorHandle, VcpLuminanceCode, (byte) brightness);
            }
            else
            {
                SetMonitorBrightness(_physicalMonitorHandle, (short) brightness);
            }
        }

        public override void SetContrast(int contrast)
        {
            RestrictValue(ref contrast);
            if (_isLowLevel)
            {
                SetVCPFeature(_physicalMonitorHandle, VcpContrastCode, (byte) contrast);
            }
            else
            {
                SetMonitorContrast(_physicalMonitorHandle, (short) contrast);
            }
        }

        public override int GetBrightness()
        {
            return _isLowLevel
                ? LowLevelGetCurrentValue(VcpLuminanceCode)
                : HighLevelGetCurrentValue(GetMonitorBrightness);
        }

        public override int GetContrast()
        {
            return _isLowLevel
                ? LowLevelGetCurrentValue(VcpContrastCode)
                : HighLevelGetCurrentValue(GetMonitorContrast);
        }

        private int LowLevelGetCurrentValue(byte code)
        {
            LpmcVcpCodeType pvct;
            uint currentValue, max;
            GetVCPFeatureAndVCPFeatureReply(_physicalMonitorHandle, code, out pvct, out currentValue, out max);
            return (int) currentValue;
        }

        private int HighLevelGetCurrentValue(NativeHighLevelGet func)
        {
            var values = new short[3];
            if (!func(_physicalMonitorHandle, ref values[0], ref values[1], ref values[2]))
            {
                throw new HighLevelPhysicalHandleInvalidException();
            }

            return values[1];
        }

        [DllImport("Dxva2.dll")]
        private static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

        ~DdcCiMonitor()
        {
            DestroyPhysicalMonitor(_physicalMonitorHandle);
        }
    }

    internal class HighLevelPhysicalHandleInvalidException : Exception
    {
    }
}