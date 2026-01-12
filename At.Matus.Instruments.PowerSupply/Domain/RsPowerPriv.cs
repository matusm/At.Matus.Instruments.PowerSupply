using System;
using System.IO.Ports;
using System.Threading;
using System.Globalization;

namespace At.Matus.Instruments.PowerSupply.Domain
{
    public partial class RsPower
    {
        private readonly SerialPort _serialPort;
        private const int _transmitDelay = 100;

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

    }
}
