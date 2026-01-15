using System;
using System.IO.Ports;
using System.Threading;
using System.Globalization;

namespace At.Matus.Instruments.PowerSupply.Domain
{
    public partial class RsPower
    {
        private readonly SerialPort _serialPort;
        private const int _transmitDelay = 10;

        private OutputState _outputState = OutputState.Unknown;
        private OcpState _ocpState = OcpState.Unknown;
        private Mode _mode = Mode.Unknown;

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
            return double.Parse(answer, CultureInfo.InvariantCulture);
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
            return answer;
        }

        private string RemoveNewLine(string line) => line.Replace("\r", string.Empty).Replace("\n", string.Empty);

        private void UpdateStatus()
        {
            byte b = _GetStatus();
            UpdateStatusFromBits(ToBits(b));
        }

        private byte _GetStatus()
        {
            var str = Query("STATUS?");
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
            if (bytes.Length > 0)
                return bytes[0];
            return 0x00;
        }

        private bool[] ToBits(byte b)
        {
            bool[] result = new bool[8];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (b & (1 << i)) != 0;
            }
            return result;
        }

        private string ToFriendlyString(byte b) => Convert.ToString(b, 2).PadLeft(8, '0');

        private void UpdateStatusFromBits(bool[] bits)
        {
            _outputState = OutputState.Unknown;
            _ocpState = OcpState.Unknown;
            _mode = Mode.Unknown;
            if (bits.Length != 8)
                return;
            _ocpState = bits[5] ? OcpState.On : OcpState.Off;
            _outputState = bits[6] ? OutputState.On : OutputState.Off;
            if (_outputState == OutputState.On)
                _mode = bits[0] ? Mode.ConstantVoltage : Mode.ConstantCurrent;
        }
    }
}
