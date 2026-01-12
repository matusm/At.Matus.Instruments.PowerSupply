using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At.Matus.Instruments.PowerSupply.Abstractions
{
    public interface IPowerSupply
    {
        void SetVoltage(double voltage);
        double GetVoltage();
        void SetCurrent(double current);
        double GetCurrent();
        void TurnOn();
        void TurnOff();
    }
}
