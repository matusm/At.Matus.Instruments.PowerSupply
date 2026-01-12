using At.Matus.Instruments.PowerSupply.Abstractions;
using System;
using System.IO.Ports;
using System.Threading;

namespace At.Matus.Instruments.PowerSupply.Domain
{
    public class RsPower : IPowerSupply
    {
        private readonly SerialPort _serialPort;
        private const int _transmitDelay = 100;

        public RsPower(string port)
        {
            _serialPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One)
            {
                Handshake = Handshake.None,
                NewLine = "\n",
                ReadTimeout = 2000,
                WriteTimeout = 500
            };
            _serialPort.Open();
            UpdateInstrumentId();
        }

        public string DevicePort => _serialPort.PortName;
        public string InstrumentManufacturer => "RS";
        public string InstrumentType { get; private set; } = string.Empty;
        public string InstrumentFirmwareVersion { get; private set; } = string.Empty;
        public string InstrumentSerialNumber { get; private set; } = string.Empty;

        public double GetCurrent() => QueryDouble("IOUT1?");

        public double GetVoltage() => QueryDouble("VOUT1?");

        public void SetCurrent(double current) => Query($"ISET1:{current.ToString("F3")}");

        public void SetVoltage(double voltage) => Query($"VSET1:{voltage.ToString("F2")}");

        public void TurnOff() => Query("OUT0");

        public void TurnOn() => Query("OUT1");

        private string Query(string command)
        {
            Write(command);
            string answer = Read();
            answer = RemoveNewLine(answer);
            return answer;
        }

        private double QueryDouble(string command)
        {
            string answer = Query(command);
            return double.Parse(answer);
        }

        private void UpdateInstrumentId()
        {
            // *IDN? -> "RS-3005P V6.8 SN:48957148"
            string idLine = Query("*IDN?");
            char[] delimiter = { ' ' };
            string[] tokens = idLine.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 3)
            {
                InstrumentType = tokens[0];
                InstrumentFirmwareVersion = tokens[1];
                InstrumentSerialNumber = tokens[2].Replace("SN:", string.Empty);
            }
        }

        private void Write(string command)
        {
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
            _serialPort.WriteLine(command);
            Thread.Sleep(_transmitDelay);
        }

        private string Read()
        {
            string answer = string.Empty;
            try
            {
                answer = _serialPort.ReadLine();
            }
            catch (TimeoutException)
            {
                // return the empty string
            }
            return RemoveNewLine(answer);
        }

        private string RemoveNewLine(string line) => line.Replace("\r", string.Empty).Replace("\n", string.Empty);

    }
}