
using System;
using System.Security.Cryptography.X509Certificates;
using CefSharp;
using CefSharp.OffScreen;

namespace test_mini
{
    public class _CONST
    {
        //https://peter.sh/experiments/chromium-command-line-switches/#mute-audio


        //public const string URL = "http://www.reddit.com/";
        //public const string URL = "http://icanhazip.com/";
        //public const string URL = "https://www.google.com/";
        //public const string URL = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over";
        public const string URL = "https://azure.microsoft.com/en-us/services/cognitive-services/bing-web-search-api/";

        public static BrowserSettings BROWSER_SETTINGS = new BrowserSettings()
        {
            //////FileAccessFromFileUrls = CefState.Disabled,
            //////UniversalAccessFromFileUrls = CefState.Disabled,
            //////WebSecurity = CefState.Disabled,
            //////WebGl = CefState.Disabled,
            ////////Reduce rendering speed to one frame per second, tweak this to whatever suites you best
            //////////WindowlessFrameRate = 1,
        };

        // You need to replace this with your own call to Cef.Initialize();

        // Default is to use an InMemory cache, set CachePath to persist cache
        public static CefSettings CEF_SETTINGS = new CefSettings
        {
            CachePath = "cache",
            //LogSeverity = LogSeverity.Disable
        };

        static _CONST()
        {
            //////CEF_SETTINGS.SetOffScreenRenderingBestPerformanceArgs();

            //Autoshutdown when closing
            CefSharpSettings.ShutdownOnExit = true;
        }
    }
    
    public class RequestHandler : IRequestHandler
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
            if (request.Url != _CONST.URL)
            {
                Console.WriteLine("????> " + request.Url);
                return CefReturnValue.Cancel;
            }

            Console.WriteLine("----> " + request.Url);
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
