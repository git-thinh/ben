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
using apigoogle;

namespace System
{
    public class HttpProxyServer : HttpServer
    {
        readonly IGooDriver _gooDriver;
        public HttpProxyServer(IGooDriver gooDriver) : base() { this._gooDriver = gooDriver; }
        protected override void ProcessRequest(System.Net.HttpListenerContext context)
        {
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            context.Response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            context.Response.AppendHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Pragma", "no-store");
            context.Response.AddHeader("cache-control", "no-cache");

            string result = string.Empty, file_id = string.Empty, content_type = "application/json; charset=utf-8";
            //content_type = "text/html; charset=utf-8"; 
            byte[] buf = null;

            try
            {
                switch (context.Request.Url.LocalPath)
                {
                    case "/favicon.ico":
                        break;
                    ///////////////////////////////////////////////////////////////////
                    // GET
                    case "/GET_USER_INFO":
                        result = this._gooDriver.f_get_userInfo();
                        break;
                    case "/GET_RETRIEVE_ALL_FILES":
                        result = this._gooDriver.f_get_retrieveAllFiles();
                        break;
                    case "/GET_FILE":
                        file_id = context.Request.QueryString["file_id"];
                        result = this._gooDriver.f_downloadFile(file_id);
                        break;
                    ///////////////////////////////////////////////////////////////////
                    // POST
                    case "/POST_CREATE_TOKEN_NEW":
                        buf = ReadFully(context.Request.InputStream);
                        if (buf.Length > 0)
                        {
                            string json = Encoding.UTF8.GetString(buf);
                            OPEN_AUTH_CLIENT clientCredentials = JsonConvert.DeserializeObject<OPEN_AUTH_CLIENT>(json);
                            result = this._gooDriver.f_create_TokenNew(clientCredentials);
                        }
                        else result = JsonConvert.SerializeObject(new { Ok = false, Message = "The data to POST must be not null" });
                        break;
                    case "/POST_GET_USER_INFO_OR_CREATE_NEW_IF_NOT_EXIST":
                        buf = ReadFully(context.Request.InputStream);
                        if (buf.Length > 0)
                        {
                            string json = Encoding.UTF8.GetString(buf);
                            OPEN_AUTH_CLIENT clientCredentials = JsonConvert.DeserializeObject<OPEN_AUTH_CLIENT>(json);
                            result = this._gooDriver.f_get_userInfoOrCreateNewIfNotExist(clientCredentials);
                        }
                        else result = JsonConvert.SerializeObject(new { Ok = false, Message = "The data to POST must be not null" });
                        break;
                    case "/POST_UPLOAD_FILE":
                        buf = ReadFully(context.Request.InputStream);
                        if (buf.Length > 0)
                        {
                            string json = Encoding.UTF8.GetString(buf);
                            DriveFile df = JsonConvert.DeserializeObject<DriveFile>(json);
                            result = this._gooDriver.f_uploadFile(df);
                        }
                        else result = JsonConvert.SerializeObject(new { Ok = false, Message = "The data to POST must be not null" });
                        break;
                    case "/POST_UPDATE_FILE":
                        buf = ReadFully(context.Request.InputStream);
                        if (buf.Length > 0)
                        {
                            string json = Encoding.UTF8.GetString(buf);
                            DriveFile df = JsonConvert.DeserializeObject<DriveFile>(json);
                            result = this._gooDriver.f_updateFile(df);
                        }
                        break;
                }
            }
            catch (Exception e) { result = JsonConvert.SerializeObject(new { Ok = false, Message = e.Message }); }

            byte[] bOutput = Encoding.UTF8.GetBytes(result);
            context.Response.ContentType = content_type;
            context.Response.ContentLength64 = bOutput.Length;
            context.Response.OutputStream.Write(bOutput, 0, bOutput.Length);
            context.Response.OutputStream.Close();
        }


        static byte[] ReadFully(Stream input)
        {
            try
            {
                int bytesBuffer = 1024 * 1024;
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
