using At.Matus.Instruments.PowerSupply.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestPS
{
    internal class Program
    {
        private static RsPower ps;

        static void Main(string[] args)
        {
            ps = new RsPower("COM6");
            double U, I;

            ps.TurnOff();
            ps.SetVoltage(0); 
            ps.SetCurrent(0);

            Console.WriteLine($"Type:            {ps.InstrumentType}");
            Console.WriteLine($"SerialNumber:    {ps.InstrumentSerialNumber}");
            Console.WriteLine($"FirmwareVersion: {ps.InstrumentFirmwareVersion}");
            Console.WriteLine($"Status:          {ps.GetStatus()}");

            ps.SetVoltage(12);
            ps.TurnOn();
            Console.WriteLine($"Status: {ps.GetStatus()}");

            RampUp(12);
            Thread.Sleep(2000);
            RampDown();

            ps.TurnOff();
            ps.SetVoltage(0);
            ps.SetCurrent(0);
            Console.WriteLine($"Status: {ps.GetStatus()}");
        }

        public static void RampUp(double voltage)
        {
            ps.SetCurrent(0);
            ps.SetVoltage(voltage);
            ps.TurnOn();
            for (int ma = 0; ma < 5000; ma += 10)
            {
                ps.SetCurrent(ma / 1000.0);
                Thread.Sleep(1);
                if(ps.GetVoltage()>=ps.GetSetVoltage())
                    return;
            }
        }

        public static void RampDown()
        {
            int u = (int)(ps.GetSetVoltage() * 1000);
            for (int mv = u; mv >= 0; mv -= 10)
            {
                ps.SetVoltage(mv / 1000.0);
                Thread.Sleep(1);
            }
            ps.SetCurrent(0);
            ps.SetVoltage(0);
            ps.TurnOff();
        }

    }
}
