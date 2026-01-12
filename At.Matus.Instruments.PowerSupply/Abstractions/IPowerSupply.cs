namespace At.Matus.Instruments.PowerSupply.Abstractions
{
    public interface IPowerSupply
    {
        string InstrumentManufacturer { get; }
        string InstrumentType { get; }
        string InstrumentFirmwareVersion { get; }
        string InstrumentSerialNumber { get; }

        void TurnOn();
        void TurnOff();
        void SetVoltage(double voltage);
        void SetCurrent(double current);
        double GetVoltage();
        double GetCurrent();
        double GetSetVoltage();
        double GetSetCurrent();
    }
}
