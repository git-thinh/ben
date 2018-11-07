using SeasideResearch.LibCurlNet;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp9
{
    public class TcpForwarderSlim
    {
        private readonly Socket _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static Socket SendToBrowser(Byte[] bSendData,   Socket mySocket)
        {
            int numBytes = 0;
            try
            {
                if (mySocket.Connected)
                {
                    if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                    {
                        Console.WriteLine("Socket Error cannot Send Packet");
                    }
                    else
                    {
                        Console.WriteLine(String.Format("No. of bytes sent {0}", numBytes));
                    }
                }
                else
                {
                    Console.WriteLine("Connection Dropped....");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error Occurred : {0} ", e.Message));
            }

            return mySocket;
        }




        static string addHTTPHeader(string buffer)
        {
            string str = "";
            int contentLength = buffer.Length;
            //str += "HTTP 1.1\r\n";
            str += "HTTP/1.1 200 OK\r\n";
            str += "Server: localhost\r\n";
            str += "Content-Type: text/html\r\n";
            str += "Accept-Ranges: bytes\r\n";
            str += "Connection: Keep-Alive\r\n";
            str += "Content-Length: " + contentLength.ToString() + "\r\n\r\n";
            return str + buffer;
        }

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

        static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.WriteLine("\r\n[BEGIN]");
            Console.Write(System.Text.Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

        static CURLcode OnSSLContext(SSLContext ctx, Object extraData)
        {
            // To do anything useful with the SSLContext object, you'll need
            // to call the OpenSSL native methods on your own. So for this
            // demo, we just return what cURL is expecting.
            return CURLcode.CURLE_OK;
        }

        private static void OnDataReceive2(IAsyncResult result)
        {
            var state = (State)result.AsyncState;
            try
            {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead > 0)
                {
                    string _request = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);

                    //////////string URL = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over";
                    ////////////URL = "https://google.com.vn";

                    //////////var dataRecorder = new EasyDataRecorder();
                    //////////Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_DEFAULT);
                    //////////try
                    //////////{
                    //////////    using (Easy easy = new Easy())
                    //////////    {
                    //////////        //easy.SetOpt(CURLoption.CURLOPT_HEADERFUNCTION, (Easy.HeaderFunction)dataRecorder.HandleHeader);
                    //////////        easy.SetOpt(CURLoption.CURLOPT_HEADER, true);

                    //////////        easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, (Easy.WriteFunction)dataRecorder.HandleWrite);

                    //////////        Easy.SSLContextFunction sf = new Easy.SSLContextFunction(OnSSLContext);
                    //////////        easy.SetOpt(CURLoption.CURLOPT_SSL_CTX_FUNCTION, sf);

                    //////////        easy.SetOpt(CURLoption.CURLOPT_URL, URL);

                    //////////        /* example.com is redirected, so we tell libcurl to follow redirection */
                    //////////        //easy.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, 1L);

                    //////////        easy.SetOpt(CURLoption.CURLOPT_CAINFO, "ca-bundle.crt");

                    //////////        easy.Perform();
                    //////////    }
                    //////////}
                    //////////finally
                    //////////{
                    //////////    Curl.GlobalCleanup();
                    //////////}

                    //////////byte[] bufHeader = dataRecorder.Header.ToArray();
                    //////////byte[] bufBody = dataRecorder.Written.ToArray();

                    //////////string header = dataRecorder.HeaderAsString,
                    //////////    body = Encoding.UTF8.GetString(dataRecorder.Written.ToArray());

                    ////////////state.SourceSocket.Send(bufHeader, 0, bufHeader.Length, SocketFlags.None);
                    //////////state.SourceSocket.Send(bufBody);


                    Byte[] bSendData = Encoding.ASCII.GetBytes("12345");
                    state.SourceSocket = SendToBrowser(bSendData, state.SourceSocket);

                    //state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive2, state);
                    //state.SourceSocket.Close();
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
            public Socket SourceSocket { get; set; }
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