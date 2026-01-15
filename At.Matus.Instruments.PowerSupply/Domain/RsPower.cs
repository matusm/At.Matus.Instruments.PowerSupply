using At.Matus.Instruments.PowerSupply.Abstractions;
using System.IO.Ports;
using System.Globalization;
using System.Threading;

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
        public Mode Mode { get { UpdateStatus(); return _mode; } }
        public OutputState OutputState { get { UpdateStatus(); return _outputState; } }
        public OcpState OcpState { get { UpdateStatus(); return _ocpState; } }

        public string InstrumentManufacturer => "RS";
        public string InstrumentType { get; private set; } = string.Empty;
        public string InstrumentFirmwareVersion { get; private set; } = string.Empty;
        public string InstrumentSerialNumber { get; private set; } = string.Empty;

        public double MaxVoltage => 30.0;
        public double MaxCurrent => 5.0;

        public void TurnOff() => Query("OUT0");

        public void TurnOn() => Query("OUT1");

        public void SetVoltage(double voltage)
        {
            if(voltage < 0 || voltage > MaxVoltage)
                throw new System.ArgumentOutOfRangeException(nameof(voltage), $"Voltage must be between 0 and {MaxVoltage:F2} V.");
            Query($"VSET1:{voltage.ToString("F2", CultureInfo.InvariantCulture)}");
        }

        public void SetCurrent(double current)
        {
            if(current < 0 || current > MaxCurrent)
                throw new System.ArgumentOutOfRangeException(nameof(current), $"Current must be between 0 and {MaxCurrent:F3} A.");
            Query($"ISET1:{current.ToString("F3", CultureInfo.InvariantCulture)}");
            Thread.Sleep(50); // Allow time for the setting to take effect, especially important for current settings (see manual)
        }

        public double GetSetVoltage() => QueryDouble("VSET1?");

        public double GetSetCurrent() => QueryDouble("ISET1?");

        public double GetVoltage() => QueryDouble("VOUT1?");

        public double GetCurrent() => QueryDouble("IOUT1?");

        public void OcpOff() => Query("OCP0");

        public void OcpOn() => Query("OCP1");

        public string GetStatus() => ToFriendlyString(_GetStatus());
    }
}