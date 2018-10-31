using System.Text;
using System.Net;
using System.IO;
using System.Web;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Authentication.OAuth2;
using DrEdit.Models;
using System.ServiceModel;
using Google.Apis.Authentication;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Oauth2.v2;
using Google;
using Newtonsoft.Json;
using Google.Apis.Drive.v2;
using apigd;

namespace System
{
    public class HttpProxyServer : HttpServer
    {
        readonly IGooDriver _gooDriver;
        public HttpProxyServer(IGooDriver gooDriver) : base() { this._gooDriver = gooDriver; }
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
                case "/GET_USER_INFO_OR_CREATE_NEW_IF_NOT_EXIST":
                    content_type = "application/json; charset=utf-8";
                    byte[] buf = ReadFully(Request.InputStream);
                    if (buf.Length > 0)
                    {
                        string json = Encoding.UTF8.GetString(buf);
                        OPEN_AUTH_CLIENT clientCredentials = JsonConvert.DeserializeObject<OPEN_AUTH_CLIENT>(json);
                        result = this._gooDriver.f_get_userInfoOrCreateNewIfNotExist(clientCredentials);
                    }
                    break;
                case "/GET_RETRIEVE_ALL_FILES":
                    content_type = "application/json; charset=utf-8";
                    result = this._gooDriver.f_get_retrieveAllFiles();
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


        static byte[] ReadFully(Stream input)
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
            catch //(Exception ex)
            {
                // Exception handling here:  Response.Write("Ex.: " + ex.Message);
            }
            return new byte[] { };
        }
    }

    public delegate void delReceiveWebRequest(HttpListenerContext Context);
    public class HttpServer
    {
        protected HttpListener Listener;
        protected bool IsStarted = false;
        public event delReceiveWebRequest ReceiveWebRequest;
        public void Start(string UrlBase)
        {
            if (this.IsStarted) return;
            if (this.Listener == null) this.Listener = new HttpListener();
            this.Listener.Prefixes.Add(UrlBase);
            this.IsStarted = true;
            this.Listener.Start();
            IAsyncResult result = this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);
        }

        public void Stop()
        {
            if (Listener != null)
            {
                this.Listener.Close();
                this.Listener = null;
                this.IsStarted = false;
            }
        }

        protected void WebRequestCallback(IAsyncResult result)
        {
            if (this.Listener == null) return;
            HttpListenerContext context = this.Listener.EndGetContext(result);
            this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);
            if (this.ReceiveWebRequest != null) this.ReceiveWebRequest(context);
            this.ProcessRequest(context);
        }

        protected virtual void ProcessRequest(HttpListenerContext Context) { }
    }
}
