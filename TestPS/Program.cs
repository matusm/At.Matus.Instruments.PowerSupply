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

            ps.TurnOn();
            ps.SetCurrent(4.106);
            ps.SetVoltage(ps.MaxVoltage);
            LogStatus();
            Thread.Sleep(2000);
            LogStatus();
            LogValues(100);


            RampUpCurrentMC1(4.106);
            Console.WriteLine($"MC1 completed  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
            Console.WriteLine($"Status:       {ps.GetStatus()}");
            LogStatus();
            Thread.Sleep(2000);
            LogStatus();
            LogValues(100);


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


        public static void LogValues(int numbers)
        {
            Console.WriteLine();
            for (int i = 0; i < numbers; i++)
            {
                Console.WriteLine($"{i,3}: [{ps.GetStatus()}]  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
                Thread.Sleep(1000);
            }
            Console.WriteLine();
        }

        //==============================================================

        public static void RampUpCurrentMC1(double targetCurrent)
        {
            // starting with 0 A and gradually increasing current until the voltage reaches the set value
            // this will fail if the load is not connected
            // Clamp voltage to max voltage
            if (targetCurrent > ps.MaxCurrent)
                targetCurrent = ps.MaxCurrent;
            ps.TurnOff();
            ps.SetCurrent(0);
            ps.SetVoltage(ps.MaxVoltage);
            ps.TurnOn();
            DateTime start = DateTime.Now;
            TimeSpan elapsed = TimeSpan.Zero;
            int index = 0;
            for (double runCurrent = 0; runCurrent <= ps.MaxCurrent; runCurrent += 0.01)
            {
                index++;
                ps.SetCurrent(runCurrent);
                Thread.Sleep(1);
                double i = ps.GetCurrent();
                elapsed = DateTime.Now - start;
                if (i >= targetCurrent)
                    break;
            }
            ps.SetCurrent(targetCurrent);
            Console.WriteLine($"MC1  -  {index,3}: {elapsed.TotalSeconds,4:F1} s  {ps.GetVoltage():F2} V  {ps.GetCurrent():F3} A");
        }

        //==============================================================



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
            // starting with 5 A and gradually increasing voltage until the voltage reaches the set value
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
