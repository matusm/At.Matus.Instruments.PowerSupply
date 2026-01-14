using At.Matus.Instruments.PowerSupply.Abstractions;
using At.Matus.Instruments.PowerSupply.Domain;
using System;
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

            RampUpVoltageM1(12);
            Console.WriteLine($"M1 completed  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
            Console.WriteLine($"Status:       {ps.GetStatus()}");
            Thread.Sleep(2000);

            RampUpVoltageM2(12);
            Console.WriteLine($"M2 completed  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
            Console.WriteLine($"Status:       {ps.GetStatus()}");
            Thread.Sleep(2000);

            ps.TurnOff();
            ps.SetVoltage(0);
            ps.SetCurrent(0);
            Console.WriteLine("Shut down");
            Console.WriteLine($"Status:       {ps.GetStatus()}");
            Thread.Sleep(2000);
        }

        //==============================================================

        public static void RampUpVoltageM1(double targetVoltage)
        {
            // starting with 0 A and gradually increasing current until the voltage reaches the set value
            // this will fail if the load is not connected
            ps.TurnOff();
            ps.SetCurrent(0);
            ps.SetVoltage(targetVoltage);
            ps.TurnOn();
            DateTime start = DateTime.Now;
            int index = 0;
            for (double runCurrent = 0; runCurrent <= ps.MaxCurrent; runCurrent += 0.010)
            {
                index++;
                ps.SetCurrent(runCurrent);
                Thread.Sleep(1);
                double u = ps.GetVoltage();
                double i = ps.GetCurrent();
                TimeSpan elapsed = DateTime.Now - start;
                Console.WriteLine($"M1  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {u:F2} V  {i:F3} A");
                if (u >= targetVoltage)
                    break;
            }
        }

        //==============================================================

        public static void RampUpVoltageM2(double targetVoltage)
        {
            // starting with 5 A and gradually increasing voltage until the voltage reaches the set value
            // Clamp voltage to max voltage
            if(targetVoltage > ps.MaxVoltage)
                targetVoltage = ps.MaxVoltage;
            ps.TurnOff();
            ps.SetCurrent(ps.MaxCurrent);
            ps.SetVoltage(0);
            ps.TurnOn();
            DateTime start = DateTime.Now;
            int index = 0;
            for (double runVoltage = 0; runVoltage <= targetVoltage; runVoltage += 0.010)
            {
                index++;
                ps.SetVoltage(runVoltage);
                Thread.Sleep(1);
                double u = ps.GetVoltage();
                double i = ps.GetCurrent();
                TimeSpan elapsed = DateTime.Now - start;
                Console.WriteLine($"M2  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {u:F2} V  {i:F3} A");
                if (u >= targetVoltage)
                    break;
            }
        }

        //==============================================================

    }
}
