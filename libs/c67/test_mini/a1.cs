using System;
using System.Security.Cryptography.X509Certificates;
using CefSharp;
using CefSharp.OffScreen;

namespace test_mini
{
    class Program
    {
        static void Main(string[] args)
        {
            Cef.Initialize(_CONST.CEF_SETTINGS, false, null);

            using (var browser = new ChromiumWebBrowser(_CONST.URL, _CONST.BROWSER_SETTINGS))
            {
                browser.RequestHandler = new BingRequestHandler();

                browser.LoadingStateChanged += (se, ev) => {
                    if (ev.IsLoading == false) {
                        Console.WriteLine("\r\n\r\n\r\n ::::::::::::::::::> LOCAD_COMPLETED ...");
                    }
                };

                // We have to wait for something, otherwise the process will exit too soon.
                Console.WriteLine("\r\n\r\n\r\n Wait for output and then press any key...");
                Console.ReadKey();
            }

            Cef.Shutdown();
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

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            string url = request.Url, method = request.Method;

            if (url == _CONST.URL)
            {
                Console.WriteLine("\r\n\r\n\r\n -> URL: " + request.Url);
                return CefReturnValue.Continue;
            }

            if(url.Contains("microsoft.com") == false && url.Contains("azure") == false)
            {
                //Console.WriteLine("\r\n\r\n -> CANCEL: " + url);
                return CefReturnValue.Cancel;
            }

            if (url.Contains(".js") || method == "POST")
            {
                Console.WriteLine("\r\n\r\n -> OK: " + url);
                return CefReturnValue.Continue;
            }

            //Console.WriteLine("\r\n\r\n -> CANCEL: " + url);
            return CefReturnValue.Cancel;
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

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
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
    }
}