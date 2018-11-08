namespace test_mini
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using CefSharp;
    using CefSharp.OffScreen;


    /// <summary>
    /// Extract text from HTML with CefSharp 3
    /// </summary>
    class Program
    {
        private static ChromiumWebBrowser browser;

        /// <summary>
        /// Main entry point of console application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        { 
            Cef.Initialize(_CONST.CEF_SETTINGS, false, null);

            using (browser = new ChromiumWebBrowser(_CONST.URL, _CONST.BROWSER_SETTINGS))
            {
                //browser.FrameLoadStart += (se, ev) => {
                //    Console.WriteLine("?> " + ev.Url);
                //    //if (ev.Url == urls.url) browser.Stop();
                //};

                browser.FrameLoadEnd += BrowserFrameLoadEnd;
                //browser.LoadError += browser_LoadError;

                browser.RequestHandler = new RequestHandler();

                // We have to wait for something, otherwise the process will exit too soon.
                Console.WriteLine("Wait for output and then press any key...");
                Console.ReadKey();
            }

            // Clean up Chromium objects. You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();
        }

        /// <summary>
        /// Method is executed if the browser reports an error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            if (e != null)
                Console.WriteLine("Retrieving error: {0}", e.ErrorText);
            else
                Console.WriteLine("Retrieving unknown error");
        }

        /// <summary>
        /// Method executes if the browser is done loading a page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void BrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            // Check to ensure it is the main frame which has finished loading
            // (rather than an iframe within the main frame).
            if (e.Frame.IsMain)
            {
                // Remove the load event handler, because we only want one snapshot of the initial page.
                browser.FrameLoadEnd -= BrowserFrameLoadEnd;

                // Wait for the screenshot to be taken.
                browser.GetTextAsync().ContinueWith(DisplayText);
            }

            if (e.Url == _CONST.URL) browser.Stop();
        }

        /// <summary>
        /// Method executes when the download page is completed and the extracted text is available.
        /// </summary>
        /// <param name="task"></param>
        private static void DisplayText(Task<string> task)
        {
            Console.WriteLine("Retrieving content...");

            var text = task.Result;

            Console.WriteLine("'{0}'", text);
        }
         
    }
}