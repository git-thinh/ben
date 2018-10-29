using System;

using System.Text;

using System.Net;

using System.IO;
using System.Diagnostics;
using System.Web;

namespace System
{
    public class HttpProxyServer : HttpServer
    {

        string f_link_getHtmlOnline(string url)
        {
            /* https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output */
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "bin/curl.exe";
            //process.StartInfo.Arguments = url;
            process.StartInfo.Arguments = url;
            //process.StartInfo.Arguments = "--insecure " + url;
            //process.StartInfo.Arguments = "--max-time 5 -v " + url; /* -v url: handle error 302 found redirect localtion*/
            //process.StartInfo.Arguments = "-m 5 -v " + url; /* -v url: handle error 302 found redirect localtion*/
            //process.StartInfo.Arguments = "--insecure -v " + url + @" -H ""User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:62.0) Gecko/20100101 Firefox/62.0"""; /* -v url: handle error 302 found redirect localtion*/
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();
            //* Read the output (or the error)
            string html = process.StandardOutput.ReadToEnd();
            //if (string.IsNullOrEmpty(html))
            //{
            //    string err = process.StandardError.ReadToEnd(), urlDirect = string.Empty;

            //    int pos = err.IndexOf("< Location: ");
            //    if (pos != -1)
            //    {
            //        urlDirect = err.Substring(pos + 12, err.Length - (pos + 12)).Split(new char[] { '\r', '\n' })[0].Trim();
            //        if (urlDirect[0] == '/')
            //        {
            //            Uri uri = new Uri(url);
            //            urlDirect = uri.Scheme + "://" + uri.Host + urlDirect;
            //        }

            //        Debug.WriteLine("-> Redirect: " + urlDirect);


            //        html = f_link_getHtmlCache(urlDirect);
            //        if (string.IsNullOrEmpty(html))
            //        {
            //            return "<script> location.href='" + urlDirect + "'; </script>";
            //        }
            //        else
            //            return html;
            //    }
            //    else
            //    {
            //        Debug.WriteLine(" ??????????????????????????????????????????? ERROR: " + url);
            //    }

            //    Debug.WriteLine(" -> Fail: " + url);

            //    return null;
            //}

            Debug.WriteLine(" -> Ok: " + url);

            //////string title = Html.f_html_getTitle(html);
            //html = _htmlFormat(url, html);
            //////f_cacheUrl(url);
            //////CACHE.TryAdd(url, html);

            //string err = process.StandardError.ReadToEnd();
            process.WaitForExit();

            ////if (_fomMain != null) _fomMain.f_browser_updateInfoPage(url, title);

            return html;

            //////* Create your Process
            ////Process process = new Process();
            ////process.StartInfo.FileName = "curl.exe";
            ////process.StartInfo.Arguments = url;
            ////process.StartInfo.UseShellExecute = false;
            ////process.StartInfo.RedirectStandardOutput = true;
            ////process.StartInfo.RedirectStandardError = true;
            ////process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            //////* Set your output and error (asynchronous) handlers
            ////process.OutputDataReceived += (se, ev) => {
            ////    string html = ev.Data;

            ////    _link.TryAdd(url, _link.Count + 1);
            ////    _html.TryAdd(url, html);
            ////};
            //////process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            //////* Start process and handlers
            ////process.Start();
            ////process.BeginOutputReadLine();
            ////process.BeginErrorReadLine();
            ////process.WaitForExit(); 
        }

        string _htmlFormat(string url, string html)
        {
            string s = HttpUtility.HtmlDecode(html);

            // Fetch all url same domain in this page ...
            //string[] urls = Html.f_html_actractUrl(url, s);
            s = Html.f_html_Format(url, s);

            int posH1 = s.ToLower().IndexOf("<h1");
            if (posH1 != -1) s = s.Substring(posH1, s.Length - posH1);

            return s;
        }

        string f_text_convert_UTF8_ACSII(string utf8)
        {
            string stFormD = utf8.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

        protected override void ProcessRequest(System.Net.HttpListenerContext Context)
        {
            HttpListenerRequest Request = Context.Request;
            HttpListenerResponse Response = Context.Response;
            string url = Request.RawUrl;
            string htm = "";
            byte[] bOutput;
            Stream OutputStream = Response.OutputStream;
            string proxy = Request.QueryString["proxy"];
            if (!string.IsNullOrEmpty(proxy)) {
                proxy = HttpUtility.UrlDecode(proxy);

               // proxy = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over";

                string text = string.Empty, _fix_lib = string.Empty;

                Debug.WriteLine("#-> " + proxy);

                text = f_link_getHtmlOnline(proxy);

                string head = text.Split(new string[] { "<body" }, StringSplitOptions.None)[0], s = "<div" + text.Substring(head.Length + 5);
                int posH1 = s.ToLower().IndexOf("<h1");
                if (posH1 != -1) s = s.Substring(posH1, s.Length - posH1);

                head = Html.f_html_Format(proxy, head);
                s = Html.f_html_Format(proxy, s);

                //if (File.Exists("view/fix.html")) _fix_lib = File.ReadAllText("view/fix.html");
                text = head.Replace("<head>", @"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />" + _fix_lib) + "<body><article id=___body><!--START_BODY-->" + s + "<!--END_BODY--></article></body></html>";
                htm = s;

                bOutput = System.Text.Encoding.UTF8.GetBytes(htm);
                Response.ContentType = "text/html; charset=utf-8";
                Response.ContentLength64 = bOutput.Length;
                OutputStream.Write(bOutput, 0, bOutput.Length);
                OutputStream.Close();
                return;
            }

            StringBuilder bi = new StringBuilder();

            switch (Request.HttpMethod)
            {
                case "POST":
                    #region
                    htm = "{}";
                    //StreamReader stream = new StreamReader(Request.InputStream);
                    //string data = stream.ReadToEnd();
                    //data = HttpUtility.UrlDecode(data);

                    ////htm = dbi.ExcutePOST(data);

                    bOutput = System.Text.Encoding.UTF8.GetBytes(htm);
                    Response.ContentType = "application/json; charset=utf-8";
                    Response.ContentLength64 = bOutput.Length;
                    OutputStream.Write(bOutput, 0, bOutput.Length);
                    OutputStream.Close();
                    break;
                #endregion
                case "GET":
                    #region 
                    string _type = "text/html; charset=utf-8";
                    string _action = Request.QueryString["action"];
                    string _input = string.Empty, _model = string.Empty, _path = string.Empty;

                    Debug.WriteLine("-> " + url);

                    switch (url)
                    {
                        case "/favicon.ico":
                            break;
                        default:

                            switch (url)
                            {
                                case "/":
                                    htm = DateTime.Now.ToString();
                                    break;
                                case "/firebug-lite.1.2.js":
                                    _type = "text/javascript";
                                    _path = "bin" + url;
                                    htm = File.ReadAllText(_path);
                                    break;
                                case "/firebug-lite.1.2.css":
                                    _type = "text/css";
                                    _path = "bin" + url;
                                    htm = File.ReadAllText(_path);
                                    break;
                            }

                            #region

                            /*

                            if (!string.IsNullOrEmpty(_action))
                            {
                                #region
                                var _dicInput = Request.QueryString.AllKeys.Distinct().ToDictionary(x => x, x => HttpUtility.UrlDecode(Request.QueryString[x]).Trim());
                                _input = JsonConvert.SerializeObject(_dicInput);
                                _dicInput.TryGetValue("model", out _model);
                                if (_model == null) _model = string.Empty;
                                _model = _model.Trim();
                                string _qs = Request.Url.Query;
                                if (!string.IsNullOrEmpty(_qs)) // The character first is '?' then removed 
                                    _qs = _qs.Substring(1);
                                else _qs = string.Empty;
                                //htm = dbi.Excute(new Message[] { new Message() { method = "GET", output = string.Empty,
                                //    action = _action, model = _model, input = _input, query_string = _qs } });
                                #endregion
                            }
                            else
                            {
                                switch (url)
                                {
                                    case "/":
                                        #region
                                        var a = new string[] { };// dbi.model_getAll();
                                        string _help =
                                            @"<h3>POST CREATE MODEL: [{""model"":""test"", ""action"":""create"", ""data"":[{""key"":""value1"", ""key2"":""tiếng việt""}]}]</h3>"
                                            + @"<h3>POST INSERT ITEMS: [{""model"":""test"", ""action"":""insert"", ""data"":[{""key"":""value1"", ""key2"":""tiếng việt""}]}]</h3>"
                                            + @"<h3>POST UPDATE ITEMS: [{""model"":""test"", ""action"":""update"", ""data"":[{""_id"":""5a9c1313c937df03e83356b2"", ""key"":""update key 1"", ""key2"":""update tiếng việt""}]}]</h3>"
                                            + @"<h3>POST REMOVE ITEM BY ID: [{""model"":""test"", ""action"":""removebyid"", ""data"":[""ID_ITEM1"",""ID_ITEM2""]}]</h3>"
                                            + "<h3>GET COUNT: ?model=test&action=count</h3>"
                                            + "<h3>GET FETCH: ?model=test&action=fetch&skip=0&limit=10</h3>"
                                            + "<h3>GET [GET BY ID]: ?model=XXX&action=getbyid&_id=ID_ITEM&skip=0&limit=10</h3>"
                                            + "<h3>GET SELECT: ?model=XXX&action=select&... when &o.N = </h3>"
                                            + "<b> EQ         </b>: Returns all documents that value are equals to value (=) has type number or string"
                                            + "<br><b> LT         </b>: Returns all documents that value are less than value (<) has type number"
                                            + "<br><b> LTE        </b>: Returns all documents that value are less than or equals value (<=) has type number"
                                            + "<br><b> GT         </b>: Returns all document that value are greater than value (>) has type number"
                                            + "<br><b> GTE        </b>: Returns all documents that value are greater than or equals value (>=) has type number"
                                            + "<br><b> BETWEEN    </b>: Returns all document that values are between [number_start | number_end] values (BETWEEN) has type number"
                                            + "<br><b> START_WITH </b>: Returns all documents that starts with value (LIKE) has type number or string"
                                            + "<br><b> CONTAINS   </b>: Returns all documents that contains value (CONTAINS) has type number or string"
                                            + "<br><b> NOT        </b>: Returns all documents that are not equals to value (not equals) has type number or string"
                                            + "<br><b> IN_ARRAY   </b>: Returns all documents that has value in values array (IN) same as [ v1 | v2 | ... ] has type number or string"
                                            + "<hr>";
                                        bi.Append(_help);
                                        bi.Append("<h1>Total model: " + a.Length + "</h1>");

                                        string _id = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower().Substring(0, 24);
                                        string _select = "model={0}&action=select&skip=0&limit=10&" +
                                            "f.1=_id&o.1=eq&v.1=" + _id +
                                            "&or=2.3" +
                                            "&f.2=___dc&o.2=eq&v.2={1}" +
                                            "&f.3=___dc&o.3=eq&v.3={2}";
                                        foreach (string mi in a)
                                        {
                                            bi.Append("<h3>+ " + mi + ": ");
                                            bi.Append("<a href='?model=" + mi + "&action=fetch&skip=0&limit=10' target='_new'>fetch</a> | ");
                                            bi.Append("<a href='?model=" + mi + "&action=getbyid&_id=" + _id + "' target='_new'>getbyid</a> | ");
                                            bi.Append("<a href='?" + string.Format(_select, mi, DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd")) + "' target='_new'>select</a> | ");
                                            bi.Append("</h3>");
                                        }
                                        htm = bi.ToString();
                                        break;
                                    #endregion
                                    default:
                                        #region

                                        var uri = new Uri(url.Substring(1).Replace("_-_", "://"));

                                        var requestBytes = Encoding.UTF8.GetBytes(
                            @"GET " + uri.PathAndQuery + @" HTTP/1.1
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36
Host: " + uri.Host + @"

");
                                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                        socket.Connect("genk.vn", 80);
                                        if (socket.Connected)
                                        {
                                            socket.Send(requestBytes);
                                            var responseBytes = new byte[socket.ReceiveBufferSize];
                                            socket.Receive(responseBytes);
                                            htm = Encoding.UTF8.GetString(responseBytes);
                                        }


                                        if (htm != "")
                                        {
                                            if (htm.IndexOf('<') != -1)
                                                htm = htm.Substring(htm.IndexOf('<'));

                                            string _format = Request.QueryString["_format"];
                                            switch (_format)
                                            {
                                                case "text":
                                                    htm = new HtmlToText().ConvertHtml(htm);
                                                    _type = "text/plain; charset=utf-8";
                                                    break;
                                                case "body":
                                                    htm = new Regex(@"<script[^>]*>[\s\S]*?</script>").Replace(htm, string.Empty);
                                                    break;
                                                case "link":
                                                    HtmlDocument doc = new HtmlDocument();
                                                    doc.LoadHtml(htm);

                                                    DocumentWithLinks nwl = new DocumentWithLinks(doc);
                                                    Console.WriteLine("Linked urls:");
                                                    for (int i = 0; i < nwl.Links.Count; i++)
                                                    {
                                                        Console.WriteLine(nwl.Links[i]);
                                                    }

                                                    Console.WriteLine("Referenced urls:");
                                                    for (int i = 0; i < nwl.References.Count; i++)
                                                    {
                                                        Console.WriteLine(nwl.References[i]);
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        #endregion
                                        break;
                                }
                            }

                            */

                            #endregion
                            break;
                    }

                    bOutput = System.Text.Encoding.UTF8.GetBytes(htm);
                    Response.ContentType = _type;
                    Response.ContentLength64 = bOutput.Length;
                    OutputStream.Write(bOutput, 0, bOutput.Length);
                    OutputStream.Close();
                    break;
                    #endregion
            }
        }
    }

    /**********************************************************************************/

    public delegate void delReceiveWebRequest(HttpListenerContext Context);
    public class HttpServer
    {
        protected HttpListener Listener;
        protected bool IsStarted = false;
        public event delReceiveWebRequest ReceiveWebRequest;
        public HttpServer()
        {
        }

        public void Start(string UrlBase)
        {
            if (this.IsStarted)
                return;

            if (this.Listener == null)
            {
                this.Listener = new HttpListener();
            }

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
            if (this.Listener == null)
                return;

            // Get out the context object
            HttpListenerContext context = this.Listener.EndGetContext(result);

            // *** Immediately set up the next context
            this.Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), this.Listener);

            if (this.ReceiveWebRequest != null)
                this.ReceiveWebRequest(context);

            this.ProcessRequest(context);
        }

        protected virtual void ProcessRequest(HttpListenerContext Context)
        {
        }
    }
}
