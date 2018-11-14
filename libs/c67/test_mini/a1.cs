using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CefSharp;
using CefSharp.OffScreen;

namespace test_mini
{
    class Program
    {
        static ChromiumWebBrowser browser;
        static void Main(string[] args)
        {
            Cef.Initialize(_CONST.CEF_SETTINGS, false, null);

            using (browser = new ChromiumWebBrowser(_CONST.URL, _CONST.BROWSER_SETTINGS) { Size = new System.Drawing.Size(1280, 1111) })
            {
                browser.RequestHandler = new BingRequestHandler();

                browser.LoadingStateChanged += (se, ev) =>
                {
                    if (ev.IsLoading == false)
                    {
                        Console.WriteLine("\r\n\r\n\r\n ::::::::::::::::::> LOCAD_COMPLETED ...");
                    }
                };

                // We have to wait for something, otherwise the process will exit too soon.
                Console.WriteLine("\r\n\r\n\r\n Wait for output and then press any key...");
                Console.ReadKey();
            }

            Cef.Shutdown();
        }


        void f_post( )
        {
            string data = File.ReadAllText("post-data.txt");
            string header = File.ReadAllText("post-header.txt");

            string url = "https://azure.microsoft.com/en-us/cognitive-services/demo/websearchapi/";

            IFrame frame = browser.GetMainFrame();
            IRequest request = frame.CreateRequest();

            request.Url = url;
            request.Method = "POST";

            request.InitializePostData();
            var element = request.PostData.CreatePostDataElement();
            //element.Bytes = req.PostData.;
            request.PostData.AddElement(element);

            NameValueCollection headers = new NameValueCollection();
            //headers.Add("Content-Type", contentType);
            request.Headers = headers;

            frame.LoadRequest(request);
        }
    }

    public class BingRequestHandler : IRequestHandler
    {
        public bool CanGetCookies(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return false;
        }

        public bool CanSetCookie(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, Cookie cookie)
        {
            return false;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            return false;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            string url = request.Url, method = request.Method;

            //if (url == _CONST.URL)
            //{
            //    Console.WriteLine("\r\n\r\n\r\n -> URL: " + request.Url);
            //    return CefReturnValue.Continue;
            //}

            //if(url.Contains("microsoft.com") == false && url.Contains("azure") == false)
            //{
            //    //Console.WriteLine("\r\n\r\n -> CANCEL: " + url);
            //    return CefReturnValue.Cancel;
            //}

            //if (url.Contains(".js") || method == "POST")
            //{
            //    Console.WriteLine("\r\n\r\n -> OK: " + url);
            //    return CefReturnValue.Continue;
            //}

            ////Console.WriteLine("\r\n\r\n -> CANCEL: " + url);
            //return CefReturnValue.Cancel;

            if (method == "POST")
                Console.WriteLine("\r\n\r\n -> POST: " + url);

            if (method == "POST" && url.Contains("websearchapi")) {
                int count = request.PostData.Elements.Count;
                List<string> ls = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    var po = request.PostData.Elements[i];
                    string data = Encoding.UTF8.GetString(po.Bytes);
                    ls.Add(data);
                }
                
                IDictionary<string, string> dict = new Dictionary<string, string>();
                foreach (var k in request.Headers.AllKeys)
                    dict.Add(k, request.Headers[k]);
                string headers = Newtonsoft.Json.JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText("post-data.txt", string.Join("", ls.ToArray()));
                File.WriteAllText("post-header.txt", headers);

            }

            return CefReturnValue.Continue;
        }


        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, string url)
        {
            return false;
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }


        //public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        //{
        //    return null;
        //}

        //public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        //{
        //}

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            /*if (request.Headers["X-Requested-With"] != "XMLHttpRequest" ||
                response.ResponseHeaders["Content-Type"].Contains("application/json")) return null;*/

            //if (!response.ResponseHeaders["Content-Type"].Contains("application/json"))
            //{
            //    return null;
            //}

            if (request.Method == "POST")
            {
                var filter = FilterManager.CreateFilter(request.Identifier.ToString());
                return filter;
            }

            return null;
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            //if (!response.ResponseHeaders["Content-Type"].Contains("application/json"))
            //{
            //    return;
            //}

            if (request.Method != "POST")
                return;

            var filter = FilterManager.GetFileter(request.Identifier.ToString()) as TestJsonFilter;

            if (filter != null)
            {
                Console.WriteLine("\r\n\r\n -> DATA: " + request.Url + "\r\n\r\n");

                string data = string.Empty;
                if (filter.DataAll != null && filter.DataAll.Count > 0)
                    data = Encoding.UTF8.GetString(filter.DataAll.ToArray());
                Console.WriteLine(data + "\r\n\r\n");
            }
        }
    }

    public class TestJsonFilter : IResponseFilter
    {
        public List<byte> DataAll = new List<byte>();

        public FilterStatus Filter(System.IO.Stream dataIn, out long dataInRead, System.IO.Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null || dataIn.Length == 0)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;

                    return FilterStatus.Done;
                }

                dataInRead = dataIn.Length;
                dataOutWritten = Math.Min(dataInRead, dataOut.Length);

                dataIn.CopyTo(dataOut);
                dataIn.Seek(0, SeekOrigin.Begin);
                byte[] bs = new byte[dataIn.Length];
                dataIn.Read(bs, 0, bs.Length);
                DataAll.AddRange(bs);

                dataInRead = dataIn.Length;
                dataOutWritten = dataIn.Length;

                return FilterStatus.NeedMoreData;
            }
            catch (Exception ex)
            {
                dataInRead = dataIn.Length;
                dataOutWritten = dataIn.Length;

                return FilterStatus.Done;
            }
        }

        public bool InitFilter()
        {
            return true;
        }

        public void Dispose()
        {

        }
    }

    public class FilterManager
    {
        private static Dictionary<string, IResponseFilter> dataList = new Dictionary<string, IResponseFilter>();

        public static IResponseFilter CreateFilter(string guid)
        {
            lock (dataList)
            {
                var filter = new TestJsonFilter();
                dataList.Add(guid, filter);

                return filter;
            }
        }

        public static IResponseFilter GetFileter(string guid)
        {
            lock (dataList)
            {
                return dataList[guid];
            }
        }
    }
}