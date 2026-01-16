using At.Matus.Instruments.PowerSupply.Abstractions;
using System.Threading;

namespace At.Matus.Instruments.PowerSupply.Extensions
{
    public static class Ramp
    {

        public static void RampUpCurrent(this IPowerSupply powerSupply, double targetCurrent, double rampTimeSec)
        {
            var stepSize = StepSizeForRampCurrentUp(targetCurrent, rampTimeSec);
            if (targetCurrent > powerSupply.MaxCurrent)
                targetCurrent = powerSupply.MaxCurrent;
            powerSupply.TurnOff();
            powerSupply.SetCurrent(0);
            powerSupply.SetVoltage(powerSupply.MaxVoltage);
            powerSupply.TurnOn();
            for (double runCurrent = 0; runCurrent <= powerSupply.MaxCurrent; runCurrent += stepSize)
            {
                if (runCurrent >= targetCurrent)
                    break;
                powerSupply.SetCurrent(runCurrent);
                Thread.Sleep(1);
                if (powerSupply.GetCurrent() >= targetCurrent)
                    break;
            }
            powerSupply.SetCurrent(targetCurrent);
        }

        public static void RampDownCurrent(this IPowerSupply powerSupply, double rampTimeSec)
        {
            var stepSize = StepSizeForRampCurrentDown(powerSupply.GetCurrent(), rampTimeSec);
            double startingCurrent = powerSupply.GetCurrent();
            powerSupply.SetCurrent(startingCurrent);
            powerSupply.SetVoltage(powerSupply.MaxVoltage);
            for (double runCurrent = startingCurrent; runCurrent >= 0; runCurrent -= stepSize)
            {
                if (runCurrent <= 0)
                    break;
                powerSupply.SetCurrent(runCurrent);
                Thread.Sleep(1);
            }
            powerSupply.SetCurrent(0);
            powerSupply.TurnOff();
        }


        private static double StepSizeForRampCurrentUp(double targetCurrent, double rampTimeSec)
        {
            // Determine step size based on target current and total ramp time in seconds
            int nSteps = (int)((rampTimeSec - 0.3) / 0.15752);
            double stepSize = targetCurrent / (nSteps + 2);
            // Clamp step size
            if (stepSize < 0.001)
                stepSize = 0.001;
            return stepSize;
        }

        private static double StepSizeForRampCurrentDown(double targetCurrent, double rampTimeSec)
        {
            // Determine step size based on target current and total ramp time in seconds
            int nSteps = (int)((rampTimeSec - 0.3) / 0.1414);
            double stepSize = targetCurrent / (nSteps + 2);
            // Clamp step size
            if (stepSize < 0.001)
                stepSize = 0.001;
            return stepSize;
        }

    }
}
