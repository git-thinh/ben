using System;
using System.IO;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;

namespace test_mini
{
    class app
    {
        static WebBrowser _browser;

        public static void Main(string[] args)
        {
            _browser = new WebBrowser();
            f1();
            Console.ReadLine();
            Cef.Shutdown();

        }

        static async void f1()
        {
            _browser.OpenUrl(_CONST.URL);
            string source = await _browser.Browser.GetSourceAsync();
            Console.WriteLine(source);
            
        }
    }


    public class WebBrowser
    {
        /// <summary>
        /// The browser page
        /// </summary>
        public ChromiumWebBrowser Browser { get; private set; }
        /// <summary>
        /// The request context
        /// </summary>
        public RequestContext RequestContext { get; private set; }

        // chromium does not manage timeouts, so we'll implement one
        private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        public WebBrowser()
        {
            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to     specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
            };

            //Autoshutdown when closing
            CefSharpSettings.ShutdownOnExit = true;

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            RequestContext = new RequestContext();
            Browser = new ChromiumWebBrowser("", null, RequestContext);
            Browser.FrameLoadStart += (se, ev) => {
                Console.WriteLine("?> " + ev.Url);
                if (ev.Url == urls.url) Browser.Stop();
            };
            PageInitialize();
        }

        /// <summary>
        /// Open the given url
        /// </summary>
        /// <param name="url">the url</param>
        /// <returns></returns>
        public void OpenUrl(string url)
        {
            try
            {
                Browser.LoadingStateChanged += PageLoadingStateChanged;
                if (Browser.IsBrowserInitialized)
                {
                    Console.WriteLine("-> " + url);
                    Browser.Load(url);

                    //create a 60 sec timeout 
                    bool isSignalled = manualResetEvent.WaitOne(TimeSpan.FromSeconds(60));
                    manualResetEvent.Reset();

                    //As the request may actually get an answer, we'll force stop when the timeout is passed
                    if (!isSignalled)
                    {
                        Browser.Stop();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                //happens on the manualResetEvent.Reset(); when a cancelation token has disposed the context
            }
            Browser.LoadingStateChanged -= PageLoadingStateChanged;
        }

        /// <summary>
        /// Manage the IsLoading parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Check to see if loading is complete - this event is called twice, one when loading starts
            // second time when it's finished
            if (!e.IsLoading)
            {
                manualResetEvent.Set();
            }
        }

        /// <summary>
        /// Wait until page initialization
        /// </summary>
        private void PageInitialize()
        {
            SpinWait.SpinUntil(() => Browser.IsBrowserInitialized);
        }
    }
}
