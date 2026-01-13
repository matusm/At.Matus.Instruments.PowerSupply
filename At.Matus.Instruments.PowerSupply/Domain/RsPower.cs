using At.Matus.Instruments.PowerSupply.Abstractions;
using System.IO.Ports;
using System.Globalization;

namespace At.Matus.Instruments.PowerSupply.Domain
{
    public partial class RsPower : IPowerSupply
    {
        public RsPower(string port)
        {
            _serialPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                NewLine = "\n",
                ReadTimeout = 100,
                WriteTimeout = 100
            };
            _serialPort.Open();
            UpdateInstrumentId();
        }

        public string DevicePort => _serialPort.PortName;
        public string InstrumentManufacturer => "RS";
        public string InstrumentType { get; private set; } = string.Empty;
        public string InstrumentFirmwareVersion { get; private set; } = string.Empty;
        public string InstrumentSerialNumber { get; private set; } = string.Empty;

        public void TurnOff() => Query("OUT0");

        public void TurnOn() => Query("OUT1");

        public void SetVoltage(double voltage) => Query($"VSET1:{voltage.ToString("F2", CultureInfo.InvariantCulture)}");

        public void SetCurrent(double current) => Query($"ISET1:{current.ToString("F3", CultureInfo.InvariantCulture)}");

        public double GetSetVoltage() => QueryDouble("VSET1?");

        public double GetSetCurrent() => QueryDouble("ISET1?");

        public double GetVoltage() => QueryDouble("VOUT1?");

        public double GetCurrent() => QueryDouble("IOUT1?");

        public void OcpOff() => Query("OCP0");

        public void OcpOn() => Query("OCP1");

        public string GetStatus() => ToFriendlyString(_GetStatus());

    }
}