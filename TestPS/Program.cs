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

            RampUpVoltageDebug(12.00);
            LogValues(50);
            powerSupply.RampDown(30);

            ////ps.TurnOn();
            ////ps.SetCurrent(4.100);
            ////ps.SetVoltage(ps.MaxVoltage);
            ////LogStatus();
            ////Thread.Sleep(2000);
            ////LogStatus();
            ////LogValues(100);

            //// 10 seconds ramp
            //powerSupply.RampUpCurrent(4.100, 10);
            //LogStatus();
            //powerSupply.RampDown(10);
            //LogStatus();

            //// 60 seconds ramp
            //powerSupply.RampUpCurrent(4.100, 60);
            //LogStatus();
            //powerSupply.RampDown(60);
            //LogStatus();

            // 5 minutes ramp
            //powerSupply.RampUpCurrent(4.100, 300);
            //LogStatus();
            //powerSupply.RampDownCurrent(300);
            //LogStatus();

            powerSupply.TurnOff();
            powerSupply.SetVoltage(0);
            powerSupply.SetCurrent(0);
            LogStatus();
        }

        //==============================================================

        public static void RampUpVoltageDebug(double targetVoltage)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Ramp voltage up ...");
            powerSupply.RampUpVoltage(targetVoltage, 60);
            var elapsed = DateTime.Now - start;
            Console.WriteLine($"RampUpV  -  {elapsed.TotalSeconds,4:F1} s  {powerSupply.GetVoltage():F2} V  {powerSupply.GetCurrent():F3} A");
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


    }
}
