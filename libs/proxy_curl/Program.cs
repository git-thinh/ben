using System;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Net;

namespace ConsoleApp9
{
    public class SslTcpClient
    {
        public static void Main(string[] args)
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