using At.Matus.Instruments.PowerSupply.Abstractions;
using At.Matus.Instruments.PowerSupply.Domain;
using System;
using System.Reflection;
using System.Threading;

namespace TestPS
{
    internal class Program
    {
        private static RsPower ps;

        static void Main(string[] args)
        {
            ps = new RsPower("COM6");

            ps.TurnOff();
            ps.SetVoltage(0); 
            ps.SetCurrent(0);

            Console.WriteLine($"Type:            {ps.InstrumentType}");
            Console.WriteLine($"SerialNumber:    {ps.InstrumentSerialNumber}");
            Console.WriteLine($"FirmwareVersion: {ps.InstrumentFirmwareVersion}");
            Console.WriteLine($"Status:          {ps.GetStatus()}");

            LogStatus();

            //ps.TurnOn();
            //ps.SetCurrent(4.100);
            //ps.SetVoltage(ps.MaxVoltage);
            //LogStatus();
            //Thread.Sleep(2000);
            //LogStatus();
            //LogValues(100);

            // 10 seconds ramp
            RampUpCurrent(4.100, 10);
            LogStatus();
            RampDownCurrent(10);
            LogStatus();

            // 30 seconds ramp
            RampUpCurrent(4.100, 30);
            LogStatus();
            RampDownCurrent(30);
            LogStatus();

            // 60 seconds ramp
            RampUpCurrent(4.100, 60);
            RampDownCurrent(60);
            LogStatus();

            // 5 minutes ramp
            RampUpCurrent(4.100, 300);
            LogStatus();
            RampDownCurrent(300);
            LogStatus();

            //RampUpCurrentMC2(4.100, 0.01);
            //Console.WriteLine($"MC2 completed  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
            //Console.WriteLine($"Status:        {ps.GetStatus()}");
            //LogStatus();
            //Thread.Sleep(2000);
            //LogStatus();
            //LogValues(100);

            ps.TurnOff();
            ps.SetVoltage(0);
            ps.SetCurrent(0);
            Console.WriteLine("Shut down");
            LogStatus();
            Thread.Sleep(2000);
        }

        //==============================================================

        public static void LogStatus()
        {
            Console.WriteLine($"Status[{ps.GetStatus()}] {ps.Mode} {ps.OutputState} {ps.OcpState}");
        }

        //==============================================================

        public static void LogValues(int numbers)
        {
            Console.WriteLine();
            for (int i = 0; i < numbers; i++)
            {
                Console.WriteLine($"{i,3}: {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
                Thread.Sleep(1000);
            }
            Console.WriteLine();
        }

        //==============================================================

        public static void RampUpCurrent(double targetCurrent, double rampTimeSec)
        {
            double stepSize = StepsizeForMC1up(targetCurrent, rampTimeSec);
            RampUpCurrentMC1(targetCurrent, stepSize);
        }

        //==============================================================


        public static void RampDownCurrent(double rampTimeSec)
        {
            var targetCurrent = ps.GetCurrent();
            double stepSize = StepsizeForMC1down(targetCurrent, rampTimeSec);
            RampDownCurrentMC1(stepSize);
        }

        //==============================================================

        public static void RampUpCurrentMC1(double targetCurrent, double stepSize)
        {
            // Starting with 0 A and gradually increasing current until it reaches the set value
            // This will fail if the load is not connected
            // Clamp target current to max current
            if (targetCurrent > ps.MaxCurrent)
                targetCurrent = ps.MaxCurrent;
            // Clamp step size
            if (stepSize < 0.001)
                stepSize = 0.001;
            ps.TurnOff();
            ps.SetCurrent(0);
            ps.SetVoltage(ps.MaxVoltage);
            ps.TurnOn();
            DateTime start = DateTime.Now;
            TimeSpan elapsed = TimeSpan.Zero;
            int index = 0;
            for (double runCurrent = 0; runCurrent <= ps.MaxCurrent; runCurrent += stepSize)
            {
                if (runCurrent >= targetCurrent)
                    break;
                index++;
                ps.SetCurrent(runCurrent);
                Thread.Sleep(1);
                elapsed = DateTime.Now - start;
                if (ps.GetCurrent() >= targetCurrent)
                    break;
            }
            ps.SetCurrent(targetCurrent);
            Console.WriteLine($"MC1 ramp up  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
        }

        //==============================================================

        public static void RampDownCurrentMC1(double stepSize)
        {
            if (ps.OutputState != OutputState.On) return;
            double startingCurrent = ps.GetCurrent();
            // Clamp step size
            if (stepSize < 0.001)
                stepSize = 0.001;
            ps.SetCurrent(startingCurrent);
            ps.SetVoltage(ps.MaxVoltage);
            DateTime start = DateTime.Now;
            TimeSpan elapsed = TimeSpan.Zero;
            int index = 0;
            for (double runCurrent = startingCurrent; runCurrent >= 0; runCurrent -= stepSize)
            {
                if (runCurrent <= 0)
                    break;
                index++;
                ps.SetCurrent(runCurrent);
                Thread.Sleep(1);
                elapsed = DateTime.Now - start;
            }
            ps.SetCurrent(0);
            Console.WriteLine($"MC1 ramp down -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
            ps.TurnOff();
        }

        //==============================================================


        public static double StepsizeForMC1up(double targetCurrent, double rampTimeSec)
        {
            // Determine step size based on target current and total allowed ramp time in seconds
            int nSteps = (int)((rampTimeSec - 0.3) / 0.15752);
            double stepSize = targetCurrent / (nSteps + 2);
            return stepSize;
        }

        //==============================================================


        public static double StepsizeForMC1down(double targetCurrent, double rampTimeSec)
        {
            // Determine step size based on target current and total allowed ramp time in seconds
            int nSteps = (int)((rampTimeSec - 0.3) / 0.1414);
            double stepSize = targetCurrent / (nSteps + 2);
            return stepSize;
        }

        //==============================================================

        public static void RampUpCurrentMC2(double targetCurrent, double stepSize)
        {
            // Starting with 0 A and gradually increasing current until it reaches the set value
            // This will fail if the load is not connected
            // Clamp target current to max current
            if (targetCurrent > ps.MaxCurrent)
                targetCurrent = ps.MaxCurrent;
            // Clamp step size
            if (stepSize < 0.01)
                stepSize = 0.01;
            ps.TurnOff();
            ps.SetCurrent(targetCurrent);
            ps.SetVoltage(0);
            ps.TurnOn();
            DateTime start = DateTime.Now;
            TimeSpan elapsed = TimeSpan.Zero;
            int index = 0;
            for (double runVoltage = 0; runVoltage <= ps.MaxVoltage; runVoltage += stepSize)
            {
                index++;
                ps.SetVoltage(runVoltage);
                Thread.Sleep(1);
                elapsed = DateTime.Now - start;
                if (ps.GetCurrent() >= targetCurrent)
                    break;
            }
            ps.SetCurrent(targetCurrent);
            ps.SetVoltage(ps.MaxVoltage);
            Console.WriteLine($"MC2  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
        }

        //==============================================================

        public static void RampUpVoltageM1(double targetVoltage)
        {
            // starting with 0 A and gradually increasing current until the voltage reaches the set value
            // this will fail if the load is not connected
            // Clamp voltage to max voltage
            if (targetVoltage > ps.MaxVoltage)
                targetVoltage = ps.MaxVoltage;
            ps.TurnOff();
            ps.SetCurrent(0);
            ps.SetVoltage(targetVoltage);
            ps.TurnOn();
            DateTime start = DateTime.Now;
            TimeSpan elapsed = TimeSpan.Zero;
            int index = 0;
            for (double runCurrent = 0; runCurrent <= ps.MaxCurrent; runCurrent += 0.005)
            {
                index++;
                ps.SetCurrent(runCurrent);
                Thread.Sleep(1);
                double u = ps.GetVoltage();
                elapsed = DateTime.Now - start;
                if (u >= targetVoltage)
                    break;
            }
            Console.WriteLine($"M1  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
        }


        //==============================================================

        public static void RampUpVoltageM2(double targetVoltage)
        {
            // Starting with 5 A and gradually increasing voltage until the voltage reaches the set value
            // Clamp voltage to max voltage
            if(targetVoltage > ps.MaxVoltage)
                targetVoltage = ps.MaxVoltage;
            ps.TurnOff();
            ps.SetVoltage(0);
            ps.SetCurrent(ps.MaxCurrent);
            ps.TurnOn();
            DateTime start = DateTime.Now;
            TimeSpan elapsed = TimeSpan.Zero;
            int index = 0;
            for (double runVoltage = 0; runVoltage <= targetVoltage; runVoltage += 0.01)
            {
                index++;
                ps.SetVoltage(runVoltage);
                Thread.Sleep(1);
                double u = ps.GetVoltage();
                elapsed = DateTime.Now - start;
                if (u >= targetVoltage)
                    break;
            }
            Console.WriteLine($"M2  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
        }

        //==============================================================

    }
}
