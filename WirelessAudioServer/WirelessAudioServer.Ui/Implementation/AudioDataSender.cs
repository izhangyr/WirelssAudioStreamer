using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WirelessAudioServer.Ui.Implementation
{
    public class AudioDataSender
    {
        private readonly IPAddress _destinationAddress = Dns.GetHostAddresses("").FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        private readonly Int32 _destinationPort;
        private Socket _audionSenderSocket;

        public AudioDataSender(Int32 port)
        {
            _destinationPort = port;
        }

        public bool ConnectionEstablished { get; set; }

        public void OpenConnection()
        {
            _audionSenderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _audionSenderSocket.Bind(new IPEndPoint(_destinationAddress, _destinationPort));
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