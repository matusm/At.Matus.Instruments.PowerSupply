using At.Matus.Instruments.PowerSupply.Abstractions;
using At.Matus.Instruments.PowerSupply.Domain;
using At.Matus.Instruments.PowerSupply.Extensions;
using System;
using System.Reflection;
using System.Threading;

namespace TestPS
{
    internal class Program
    {
        private static RsPower powerSupply;

        static void Main(string[] args)
        {
            powerSupply = new RsPower("COM6");

            powerSupply.TurnOff();
            powerSupply.SetVoltage(0); 
            powerSupply.SetCurrent(0);

            Console.WriteLine($"Type:            {powerSupply.InstrumentType}");
            Console.WriteLine($"SerialNumber:    {powerSupply.InstrumentSerialNumber}");
            Console.WriteLine($"FirmwareVersion: {powerSupply.InstrumentFirmwareVersion}");
            Console.WriteLine($"Status:          {powerSupply.GetStatus()}");

            LogStatus();

            //ps.TurnOn();
            //ps.SetCurrent(4.100);
            //ps.SetVoltage(ps.MaxVoltage);
            //LogStatus();
            //Thread.Sleep(2000);
            //LogStatus();
            //LogValues(100);

            // 10 seconds ramp
            powerSupply.RampUpCurrent(4.100, 10);
            LogStatus();
            powerSupply.RampDownCurrent(10);
            LogStatus();

            // 60 seconds ramp
            powerSupply.RampUpCurrent(4.100, 60);
            LogStatus();
            powerSupply.RampDownCurrent(60);
            LogStatus();

            // 5 minutes ramp
            //powerSupply.RampUpCurrent(4.100, 300);
            //LogStatus();
            //powerSupply.RampDownCurrent(300);
            //LogStatus();

            powerSupply.TurnOff();
            powerSupply.SetVoltage(0);
            powerSupply.SetCurrent(0);
            Console.WriteLine("Shut down");
            LogStatus();
            Thread.Sleep(2000);
        }

        //==============================================================

        public static void LogStatus()
        {
            Console.WriteLine($"Status[{powerSupply.GetStatus()}] {powerSupply.Mode} {powerSupply.OutputState} {powerSupply.OcpState}");
        }

        //==============================================================

        public static void LogValues(int numbers)
        {
            Console.WriteLine();
            for (int i = 0; i < numbers; i++)
            {
                Console.WriteLine($"{i,3}: {powerSupply.GetVoltage():F2} V  {powerSupply.GetCurrent():F3} A");
                Thread.Sleep(1000);
            }
            Console.WriteLine();
        }


        //==============================================================

        public static void RampUpVoltageM1(double targetVoltage)
        {
            // starting with 0 A and gradually increasing current until the voltage reaches the set value
            // this will fail if the load is not connected
            // Clamp voltage to max voltage
            if (targetVoltage > powerSupply.MaxVoltage)
                targetVoltage = powerSupply.MaxVoltage;
            powerSupply.TurnOff();
            powerSupply.SetCurrent(0);
            powerSupply.SetVoltage(targetVoltage);
            powerSupply.TurnOn();
            DateTime start = DateTime.Now;
            TimeSpan elapsed = TimeSpan.Zero;
            int index = 0;
            for (double runCurrent = 0; runCurrent <= powerSupply.MaxCurrent; runCurrent += 0.005)
            {
                index++;
                powerSupply.SetCurrent(runCurrent);
                Thread.Sleep(1);
                double u = powerSupply.GetVoltage();
                elapsed = DateTime.Now - start;
                if (u >= targetVoltage)
                    break;
            }
            Console.WriteLine($"M1  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {powerSupply.GetVoltage():F2} V  {powerSupply.GetCurrent():F3} A");
        }


        //==============================================================

        public static void RampUpVoltageM2(double targetVoltage)
        {
            // Starting with 5 A and gradually increasing voltage until the voltage reaches the set value
            // Clamp voltage to max voltage
            if(targetVoltage > powerSupply.MaxVoltage)
                targetVoltage = powerSupply.MaxVoltage;
            powerSupply.TurnOff();
            powerSupply.SetVoltage(0);
            powerSupply.SetCurrent(powerSupply.MaxCurrent);
            powerSupply.TurnOn();
            DateTime start = DateTime.Now;
            TimeSpan elapsed = TimeSpan.Zero;
            int index = 0;
            for (double runVoltage = 0; runVoltage <= targetVoltage; runVoltage += 0.01)
            {
                index++;
                powerSupply.SetVoltage(runVoltage);
                Thread.Sleep(1);
                double u = powerSupply.GetVoltage();
                elapsed = DateTime.Now - start;
                if (u >= targetVoltage)
                    break;
            }
            Console.WriteLine($"M2  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {powerSupply.GetVoltage():F2} V  {powerSupply.GetCurrent():F3} A");
        }

        //==============================================================

    }
}
