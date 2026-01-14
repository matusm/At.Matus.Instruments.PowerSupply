using At.Matus.Instruments.PowerSupply.Abstractions;
using System;
using System.Threading;

namespace At.Matus.Instruments.PowerSupply.Extensions
{
    public static class Ramp
    {
        // Ramp up voltage to targetVoltage by increasing the set current.
        // The power supply must be in CC mode for this to work properly.

        public static void RampCurrentUp(this IPowerSupply powerSupply, double targetCurrent, double stepSize, int delayMilliseconds)
        {
            double currentCurrent = powerSupply.GetCurrent();
        }

        public static void RampVoltageUp(this IPowerSupply powerSupply, double targetVoltage, double stepSize, int delayMilliseconds)
        {
            double currentVoltage = powerSupply.GetVoltage();
        }


    }
}
