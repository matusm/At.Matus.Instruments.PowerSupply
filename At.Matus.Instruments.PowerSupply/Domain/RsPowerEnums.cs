namespace At.Matus.Instruments.PowerSupply.Domain
{
    public enum OutputState
    {
        Unknown,
        Off,
        On
    }

    public enum OcpState
    {
        Unknown,
        Off,
        On
    }

    public enum Mode
    {
        Unknown,
        ConstantCurrent,
        ConstantVoltage
    }

}
