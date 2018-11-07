using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp9
{
    public class TcpForwarderSlim
    {
        private readonly Socket _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public void Start(IPEndPoint local, IPEndPoint remote)
        {
            _mainSocket.Bind(local);
            _mainSocket.Listen(10);

            while (true)
            {
                var source = _mainSocket.Accept();
                ////////////var destination = new TcpForwarderSlim();
                ////////////var state = new State(source, destination._mainSocket);
                ////////////destination.Connect(remote, source);
                //source.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                
                var state = new State(source, null);
                source.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive2, state);
            }
        }

        private void Connect(EndPoint remoteEndpoint, Socket destination)
        {
            var state = new State(_mainSocket, destination);
            _mainSocket.Connect(remoteEndpoint);
            _mainSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnDataReceive, state);
        }

        private static void OnDataReceive2(IAsyncResult result)
        {
            var state = (State)result.AsyncState;
            try
            {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead > 0)
                {
                    string s = Encoding.ASCII.GetString(state.Buffer,0, bytesRead);

                    string host = "216.58.221.99";
                    byte[] buffer = new byte[2048];
                    int bytes;

                    // Connect socket
                    TcpClient client = new TcpClient("127.0.0.1", 8888);
                    NetworkStream stream = client.GetStream();

                    // Establish Tcp tunnel
                    byte[] tunnelRequest = Encoding.UTF8.GetBytes(String.Format("CONNECT {0}:443  HTTP/1.1\r\nHost: {0}\r\n\r\n", host));
                    stream.Write(tunnelRequest, 0, tunnelRequest.Length);
                    stream.Flush();

                    // Read response to CONNECT request
                    // There should be loop that reads multiple packets
                    bytes = stream.Read(buffer, 0, buffer.Length);
                    Console.Write(Encoding.UTF8.GetString(buffer, 0, bytes));

                    // Wrap in SSL stream
                    SslStream sslStream = new SslStream(stream);
                    sslStream.AuthenticateAsClient(host);

                    // Send request
                    byte[] request = Encoding.UTF8.GetBytes(String.Format("GET https://{0}/  HTTP/1.1\r\nHost: {0}\r\n\r\n", host));
                    sslStream.Write(request, 0, request.Length);
                    sslStream.Flush();

                    // Read response
                    do
                    {
                        bytes = sslStream.Read(buffer, 0, buffer.Length);
                        Console.Write(Encoding.UTF8.GetString(buffer, 0, bytes));
                    } while (bytes != 0);

                    client.Close();














                    state.SourceSocket.Send(buffer, 0, bytes, SocketFlags.None);

                    //state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);



                }
            }
            catch
            {
                state.DestinationSocket.Close();
                state.SourceSocket.Close();
            }
        }

        private static void OnDataReceive(IAsyncResult result)
        {
            var state = (State)result.AsyncState;
            try
            {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead > 0)
                {
                    state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                }
            }
            catch
            {
                state.DestinationSocket.Close();
                state.SourceSocket.Close();
            }
        }

        private class State
        {
            public Socket SourceSocket { get; private set; }
            public Socket DestinationSocket { get; private set; }
            public byte[] Buffer { get; private set; }

            public State(Socket source, Socket destination)
            {
                SourceSocket = source;
                DestinationSocket = destination;
                Buffer = new byte[8192];
            }
        }
    }
}