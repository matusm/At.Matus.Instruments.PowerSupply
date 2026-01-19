using At.Matus.Instruments.PowerSupply.Abstractions;
using System.Threading;

namespace At.Matus.Instruments.PowerSupply.Extensions
{
    public static class Ramp
    {
        public static void RampUpCurrent(this IPowerSupply powerSupply, double targetCurrent, double rampTimeSec)
        {
            if (targetCurrent > powerSupply.MaxCurrent)
                targetCurrent = powerSupply.MaxCurrent;
            double stepSize = StepSizeForRamp(targetCurrent, rampTimeSec, _timePerStepSecCUp);
            stepSize = stepSize < 0.001 ? 0.001 : stepSize;
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

        public static void RampUpVoltage(this IPowerSupply powerSupply, double targetVoltage, double rampTimeSec)
        {
            if (targetVoltage > powerSupply.MaxVoltage)
                targetVoltage = powerSupply.MaxVoltage;
            double stepSize = StepSizeForRamp(targetVoltage, rampTimeSec, _timePerStepSecVUp);
            stepSize = stepSize < 0.01 ? 0.01 : stepSize;
            powerSupply.SetVoltage(0);
            powerSupply.SetCurrent(powerSupply.MaxCurrent);
            powerSupply.TurnOn();
            for (double runVoltage = 0; runVoltage <= targetVoltage; runVoltage += stepSize)
            {
                powerSupply.SetVoltage(runVoltage);
                Thread.Sleep(1);
                if (powerSupply.GetVoltage() >= targetVoltage)
                    break;
            }
            powerSupply.SetVoltage(targetVoltage);
        }

        public static void RampDown(this IPowerSupply powerSupply, double rampTimeSec)
        {
            var stepSize = StepSizeForRamp(powerSupply.GetCurrent(), rampTimeSec, _timePerStepSecCDown);
            stepSize = stepSize < 0.001 ? 0.001 : stepSize;
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

        private const double _timePerStepSecCUp = 0.15752;
        private const double _timePerStepSecCDown = 0.1414;
        private const double _timePerStepSecVUp = 0.1580;

        private static double StepSizeForRamp(double targetValue, double rampTimeSec, double _parameter)
        {
            int nSteps = (int)((rampTimeSec - 0.3) / _parameter);
            double stepSize = targetValue / (nSteps + 2);
            return stepSize;
        }
    }
}
