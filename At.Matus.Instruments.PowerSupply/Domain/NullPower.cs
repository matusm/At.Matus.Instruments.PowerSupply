using At.Matus.Instruments.PowerSupply.Abstractions;

namespace At.Matus.Instruments.PowerSupply.Domain
{
    public class NullPower : IPowerSupply
    {
        public string InstrumentManufacturer => string.Empty;
        public string InstrumentType => string.Empty;
        public string InstrumentFirmwareVersion => string.Empty;
        public string InstrumentSerialNumber => string.Empty;

        public double MaxVoltage => 0;
        public double MaxCurrent => 0;

        public void TurnOn() { }
        public void TurnOff() { }
        public void SetVoltage(double voltage) { }
        public void SetCurrent(double current) { }
        public double GetVoltage() => 0;
        public double GetCurrent() => 0;
        public double GetSetVoltage() => 0;
        public double GetSetCurrent() => 0;
    }
}
