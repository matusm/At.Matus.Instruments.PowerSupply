using At.Matus.Instruments.PowerSupply.Abstractions;

namespace At.Matus.Instruments.PowerSupply.Domain
{
    public class ManualPower : IPowerSupply
    {
        public string InstrumentManufacturer => "At.Matus";
        public string InstrumentType => "Mockup";
        public string InstrumentFirmwareVersion => "1.0.0";
        public string InstrumentSerialNumber => "000";

        public double MaxVoltage => 30.0;
        public double MaxCurrent => 5.0;

        public void TurnOn() => _isOn = true;

        public void TurnOff() => _isOn = false;

        public void SetVoltage(double voltage)
        {
            _setVoltage = voltage;
            _actualVoltage = voltage;
        }

        public void SetCurrent(double current)
        {
            _setCurrent = current;
            _actualCurrent = current;
        }

        public double GetVoltage() => _actualVoltage;

        public double GetSetVoltage() => _setVoltage;

        public double GetCurrent() => _actualCurrent;

        public double GetSetCurrent() => _setCurrent;

        private double _actualVoltage = 0.0;
        private double _actualCurrent = 0.0;
        private double _setVoltage = 0.0;
        private double _setCurrent = 0.0;
        private bool _isOn = false;
    }
}
