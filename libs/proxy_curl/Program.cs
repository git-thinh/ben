using System;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Net;
using SeasideResearch.LibCurlNet;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace ConsoleApp9
{
    public class SslTcpClient
    {
        public static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);

            server.Start();
            Console.WriteLine("Server has started on 127.0.0.1:80.{0}Waiting for a connection...", Environment.NewLine);

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("A client connected.");

                NetworkStream stream = client.GetStream();

                while (client.Available < 3)
                    Console.WriteLine("::> wait for enough bytes to be available");

                Byte[] bytes = new Byte[client.Available];
                stream.Read(bytes, 0, bytes.Length);

                //translate bytes of request to string
                string _request = Encoding.UTF8.GetString(bytes),
                    line_first = _request.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0].Trim(),
                    url = line_first.Replace("GET ", string.Empty).Replace("POST ", string.Empty).Trim().Split(' ')[0];
                Console.WriteLine("-> " + url);

                //string res = addHTTPHeader("12345");
                //Byte[] bSendData = Encoding.ASCII.GetBytes(res);
                //stream.Write(bSendData, 0, bSendData.Length);
                if (_request.Contains("text/html") == false)
                {
                    string res = addHTTPHeader(0, "");
                    Byte[] bSendData = Encoding.ASCII.GetBytes(res);
                    stream.Write(bSendData, 0, bSendData.Length);
                    stream.Flush();
                }
                else
                {

                    string URL = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over";
                    URL = "https://azure.microsoft.com/en-us/services/cognitive-services/bing-web-search-api/";

                    var dataRecorder = new EasyDataRecorder();
                    Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_DEFAULT);
                    try
                    {
                        using (Easy easy = new Easy())
                        {
                            //easy.SetOpt(CURLoption.CURLOPT_HEADERFUNCTION, (Easy.HeaderFunction)dataRecorder.HandleHeader);
                            //easy.SetOpt(CURLoption.CURLOPT_HEADER, false);

                            easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, (Easy.WriteFunction)dataRecorder.HandleWrite);

                            Easy.SSLContextFunction sf = new Easy.SSLContextFunction(OnSSLContext);
                            easy.SetOpt(CURLoption.CURLOPT_SSL_CTX_FUNCTION, sf);

                            easy.SetOpt(CURLoption.CURLOPT_URL, URL);

                            /* example.com is redirected, so we tell libcurl to follow redirection */
                            //easy.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, 1L);

                            easy.SetOpt(CURLoption.CURLOPT_CAINFO, "ca-bundle.crt");

                            easy.Perform();
                        }
                    }
                    finally
                    {
                        Curl.GlobalCleanup();
                    }

                    byte[] bufHeader = dataRecorder.Header.ToArray();
                    byte[] bufBody = dataRecorder.Written.ToArray();

                    string header = dataRecorder.HeaderAsString,
                        body = Encoding.UTF8.GetString(bufBody);

                    //state.SourceSocket.Send(bufHeader, 0, bufHeader.Length, SocketFlags.None);
                    Console.WriteLine("OK> " + URL);
                    File.WriteAllText("text.txt", body);

                    Byte[] bSendData = Encoding.UTF8.GetBytes(addHTTPHeader(bufBody.Length, body));
                    stream.Write(bSendData, 0, bSendData.Length);
                    stream.Flush();
                }
            }

            Console.ReadLine();
        }

        static string addHTTPHeader(int contentLength, string body)
        {
            string str = "";
            //int contentLength = buffer.Length;
            //str += "HTTP 1.1\r\n";
            str += "HTTP/1.1 200 OK\r\n";
            str += "Server: localhost\r\n";
            str += "Content-Type: text/html\r\n";
            str += "Accept-Ranges: bytes\r\n";
            str += "Connection: Keep-Alive\r\n";
            str += "Content-Length: " + contentLength.ToString() + "\r\n\r\n";
            return str + body;
        }

        static CURLcode OnSSLContext(SSLContext ctx, Object extraData)
        {
            // To do anything useful with the SSLContext object, you'll need
            // to call the OpenSSL native methods on your own. So for this
            // demo, we just return what cURL is expecting.
            return CURLcode.CURLE_OK;
        }

        public static void Main1(string[] args)
        {
            //https://blog.brunogarcia.com/2012/10/simple-tcp-forwarder-in-c.html
            //https://azure.microsoft.com/en-us/services/cognitive-services/bing-web-search-api/

            string host = "encrypted.google.com";
            string proxy = "127.0.0.1";//host;
            int proxyPort = 8888;//443;

            var px = new TcpForwarderSlim();

            px.Start(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888), new IPEndPoint(IPAddress.Parse("216.58.221.99"), 443));

            Console.ReadLine();









            byte[] buffer = new byte[2048];
            int bytes;

            // Connect socket
            TcpClient client = new TcpClient(proxy, proxyPort);
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

            Console.WriteLine("END...");
            Console.ReadKey();
        }
    }
}