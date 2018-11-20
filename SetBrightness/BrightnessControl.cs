using System;

namespace SetBrightness
{
    internal class BrightnessControl
    {
        private readonly IntPtr _hWnd;
        private NativeStructures.PhysicalMonitor[] _pPhysicalMonitorArray;
        private uint _pdwNumberOfPhysicalMonitors;

        public BrightnessControl(IntPtr hWnd)
        {
            _hWnd = hWnd;
            SetupMonitors();
        }

        public void SetupMonitors()
        {
            var hMonitor = NativeCalls.MonitorFromWindow(_hWnd, NativeConstants.MonitorDefaulttoprimary);

            NativeCalls.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref _pdwNumberOfPhysicalMonitors);
            _pPhysicalMonitorArray = new NativeStructures.PhysicalMonitor[_pdwNumberOfPhysicalMonitors];

            NativeCalls.GetPhysicalMonitorsFromHMONITOR(hMonitor, _pdwNumberOfPhysicalMonitors, _pPhysicalMonitorArray);
        }

        public void SetBrightness(short brightness, int monitorNumber)
        {
            NativeCalls.SetMonitorBrightness(_pPhysicalMonitorArray[monitorNumber].hPhysicalMonitor, brightness);
        }

        public BrightnessInfo GetBrightnessCapabilities(int monitorNumber)
        {
            short current = -1, minimum = -1, maximum = -1;
            bool succeed = NativeCalls.GetMonitorBrightness(_pPhysicalMonitorArray[monitorNumber].hPhysicalMonitor, ref minimum,
                ref current, ref maximum);
            if (!succeed)
            {
                throw new GetBrightnessInfoError();
            }
            return new BrightnessInfo {Minimum = minimum, Maximum = maximum, Current = current};
        }

        public void DestroyMonitors()
        {
            NativeCalls.DestroyPhysicalMonitors(_pdwNumberOfPhysicalMonitors, _pPhysicalMonitorArray);
        }

        public uint GetMonitors()
        {
            return _pdwNumberOfPhysicalMonitors;
        }
    }

    public struct BrightnessInfo
    {
        public int Minimum;
        public int Maximum;
        public int Current;
    }

    public class GetBrightnessInfoError : Exception
    {
        public override string ToString()
        {
            return "get brightness info failed";
        }
    }
}