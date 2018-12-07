using System;

namespace SetBrightness
{
    public abstract class Monitor
    {
        public bool SupportContrast;

        /// <summary>
        /// whether support specific method to manipulate or support brightness
        /// </summary>
        public bool CanUse;

        public string Name = "test";
        public MonitorType Type;

        public abstract void SetBrightness(int brightness);
        public abstract void SetContrast(int contrast);
        // modify: not get real brightness or contrast everytime, just use trackbar.value, to prevent
        // a dead loop. call TabPageTemplate.UpdateValues() to refresh values and update UI
        public abstract int GetBrightness();
        public abstract int GetContrast();
        public abstract bool IsSameMonitor(Monitor monitor);
        public abstract bool IsValide();
    }

    public enum MonitorType
    {
        DdcCiMonitor,
        WmiMonitor
    }

    public class InvalidMonitorException : Exception
    {
    }
}