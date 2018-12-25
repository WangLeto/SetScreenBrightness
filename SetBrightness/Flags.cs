using System;

namespace SetBrightness
{
    partial class DdcCiMonitor
    {
        [Flags]
        public enum PdwMonitorCapabilitiesFlag
        {
            McCapsNone = 0x00000000,
            McCapsMonitorTechnologyType = 0x00000001,
            McCapsBrightness = 0x00000002,
            McCapsContrast = 0x00000004,
            McCapsColorTemperature = 0x00000008,
            McCapsRedGreenBlueGain = 0x00000010,
            McCapsRedGreenBlueDrive = 0x00000020,
            McCapsDegauss = 0x00000040,
            McCapsDisplayAreaPosition = 0x00000080,
            McCapsDisplayAreaSize = 0x00000100,
            McCapsRestoreFactoryDefaults = 0x00000400,
            McCapsRestoreFactoryColorDefaults = 0x00000800,
            McRestoreFactoryDefaultsEnablesMonitorSettings = 0x00001000
        }

        [Flags]
        public enum PdwSupportedColorTemperaturesFlag
        {
            McSupportedColorTemperatureNone = 0x00000000,
            McSupportedColorTemperature4000K = 0x00000001,
            McSupportedColorTemperature5000K = 0x00000002,
            McSupportedColorTemperature6500K = 0x00000004,
            McSupportedColorTemperature7500K = 0x00000008,
            McSupportedColorTemperature8200K = 0x00000010,
            McSupportedColorTemperature9300K = 0x00000020,
            McSupportedColorTemperature10000K = 0x00000040,
            McSupportedColorTemperature11500K = 0x00000080
        }
    }
}