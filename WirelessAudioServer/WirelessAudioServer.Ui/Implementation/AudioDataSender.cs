using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WirelessAudioServer.Ui.Implementation
{
    public class AudioDataSender
    {
        private readonly IPAddress _destinationAddress = Dns.GetHostAddresses("").FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        private readonly Action<IEnumerable<string>> _updateConnectionState;
        private Socket _audionSenderSocket;

        public AudioDataSender(Action<IEnumerable<string>> updateConnectionState)
        {
            _updateConnectionState = updateConnectionState;
        }

        public bool ConnectionEstablished { get; set; }

        public void OpenConnection(Int32 port)
        {
            _audionSenderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _audionSenderSocket.Bind(new IPEndPoint(_destinationAddress, port));
            _audionSenderSocket.Listen(5);
            _audionSenderSocket.BeginAccept(Accept, _audionSenderSocket);
        }

        private void Accept(IAsyncResult ar)
        {
            if (!ar.IsCompleted)
            {
                return;
            }

            try
            {
                _audionSenderSocket = (Socket) ar.AsyncState;
                _audionSenderSocket = _audionSenderSocket.EndAccept(ar);
                ConnectionEstablished = true;
                _updateConnectionState(new[] { ((IPEndPoint)_audionSenderSocket.RemoteEndPoint).Address.ToString() });
            }
            catch(ObjectDisposedException)
            {}
        }

        public void Close()
        {
            _audionSenderSocket.Close();
            _audionSenderSocket.Dispose();
        }

        public void SendBytes(Byte[] bytes)
        {
            try
            {
                _audionSenderSocket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, ar => { }, null);
            }
            catch (SocketException)
            {
                ConnectionEstablished = false;
                _audionSenderSocket.BeginAccept(Accept, _audionSenderSocket);
            }
        }

        public void SendText(String str)
        {
            SendBytes(Encoding.ASCII.GetBytes(str));
        }
    }
}