using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MSExpo.Editor.Helpers
{
    public class SerialHelper
    {
        public SerialHelper()
        {
            var serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            serialPort.PortName = "COM6";
            serialPort.BaudRate = 9600;
            // _serialPort.Parity = SetPortParity(_serialPort.Parity);
            //_serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            //  _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            // _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

            // Set the read/write timeouts
            serialPort.ReadTimeout = 5000;
            serialPort.WriteTimeout = 500;

            serialPort.Open();
            var obs = CreatePortObservable(serialPort);

            obs.Subscribe(x => Debug.WriteLine(x));

        }
        public void Listen()
        {

        }

        static IObservable<byte> CreatePortObservable(SerialPort port)
        {
            return Observable.Create<byte>(obs =>
            {
                // Alternative Rx-driven approach on the inside
                //
                // var rcv = Observable.FromEvent<SerialDataReceivedEventArgs>(port, "DataReceived");
                // var err = Observable.FromEvent<SerialErrorReceivedEventArgs>(port, "ErrorReceived");

                var rcv = new SerialDataReceivedEventHandler((sender, e) =>
                {
                    if (e.EventType == SerialData.Eof)
                    {
                        obs.OnCompleted();
                    }
                    else
                    {
                        var buf = new byte[port.BytesToRead];
                        for (int i = 0; i < port.Read(buf, 0, buf.Length); i++)
                            obs.OnNext(buf[i]);
                    }
                });
                port.DataReceived += rcv;

                var err = new SerialErrorReceivedEventHandler((sender, e) =>
                {
                    obs.OnError(new Exception(e.EventType.ToString()));
                });
                port.ErrorReceived += err;

                return () =>
                {
                    port.DataReceived -= rcv;
                    port.ErrorReceived -= err;
                    // depending on ownership of port, you could Dispose it here too
                };
            });
        }
    }
}
