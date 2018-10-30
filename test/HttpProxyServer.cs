using Google.Protobuf;
using Google.Protobuf.Examples.Messages;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace System
{
    public class HttpProxyServer : HttpServer
    {
        private byte[] ReadFully(Stream input)
        {
            try
            {
                int bytesBuffer = 1024;
                byte[] buffer = new byte[bytesBuffer];
                using (MemoryStream ms = new MemoryStream())
                {
                    int readBytes;
                    while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, readBytes);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Exception handling here:  Response.Write("Ex.: " + ex.Message);
            }
            return new byte[] { };
        }

        protected override void ProcessRequest(System.Net.HttpListenerContext Context)
        {
            HttpListenerRequest Request = Context.Request;
            HttpListenerResponse Response = Context.Response;
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            Response.AppendHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");


            // Do any of these result in META tags e.g. <META HTTP-EQUIV="Expire" CONTENT="-1">
            // HTTP Headers or both?            
            // Is this required for FireFox? Would be good to do this without magic strings.
            // Won't it overwrite the previous setting
            //Response.Headers.Add("Cache-Control", "no-cache, no-store");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Pragma", "no-store");
            Response.AddHeader("cache-control", "no-cache");

            string result = string.Empty, data = string.Empty,
                content_type = "text/html; charset=utf-8",
                uri = HttpUtility.UrlDecode(Request.RawUrl);
            Stream OutputStream = Response.OutputStream;
            byte[] bOutput;

            switch (Request.Url.LocalPath)
            {
                case "/favicon.ico":
                    break;
                case "/nodejs":                    
                    byte[] buf = ReadFully(Request.InputStream);
                    if (buf.Length > 0)
                    {
                        using (CodedInputStream input = new CodedInputStream(buf, 0, buf.Length))
                        {
                            try
                            {
                                var l2 = Message.Parser.ParseFrom(input);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    break;
                default:
                    result = DateTime.Now.ToString();
                    break;
            }

            bOutput = Encoding.UTF8.GetBytes(result);
            Response.ContentType = content_type;
            Response.ContentLength64 = bOutput.Length;
            OutputStream.Write(bOutput, 0, bOutput.Length);
            OutputStream.Close();
        }
        
    }
}
