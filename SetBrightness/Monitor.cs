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

        public abstract void SetBrightness(int brightness);
        public abstract int GetBrightness();
        public abstract void SetContrast(int contrast);
        public abstract int GetContrast();
    }
}