using At.Matus.Instruments.PowerSupply.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int delay = 500;
            var ps = new RsPower("COM6");
            double U, I;

            Console.WriteLine($"Type {ps.InstrumentType}");
            Console.WriteLine($"SerialNumber {ps.InstrumentSerialNumber}");
            Console.WriteLine($"FirmwareVersion {ps.InstrumentFirmwareVersion}");


            ps.TurnOff(); ps.SetVoltage(0); ps.SetCurrent(0);
        }
    }
}
