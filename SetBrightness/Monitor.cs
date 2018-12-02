using System;

namespace SetBrightness
{
    abstract class Monitor
    {
        public bool SupportContrast;

        /// <summary>
        /// whether support specific method to manipulate or support brightness
        /// </summary>
        public bool CanUse;

        public abstract void SetBrightness(int brightness);
        public abstract int GetBrightness(int brightness);
        public abstract void SetContrast(int contrast);
        public abstract int GetContrast(int contrast);
    }
}