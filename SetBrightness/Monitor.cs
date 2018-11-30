using System;

namespace SetBrightness
{
    abstract class Monitor
    {
        public int Brightness;
        public int Contrast;
        public bool SupportContrast;
        public abstract void SetBrightness(int brightness);
        public abstract int GetBrightness(int brightness);
    }
}