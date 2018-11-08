// Copyright © 2010-2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Threading.Tasks;

namespace test_mini
{
    public class Program
    { 
        public static int Main(string[] args)
        {
            string url = "http://www.reddit.com/";
            url = "http://icanhazip.com/";
            //url = "https://www.google.com/";
            //url = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over";
            url = "https://azure.microsoft.com/en-us/services/cognitive-services/bing-web-search-api/";

            // You need to replace this with your own call to Cef.Initialize();
            // Default is to use an InMemory cache, set CachePath to persist cache
            var settings = new CefSettings
            {
                CachePath = "cache",
                LogSeverity = LogSeverity.Disable
            };
            Cef.Initialize(settings, false, null);

            MainAsync(url);
            
            // We have to wait for something, otherwise the process will exit too soon.
            Console.ReadKey();

            // Clean up Chromium objects. You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();

            return 0;
        }

        private static async void MainAsync(string url)
        {
            var browserSettings = new BrowserSettings();
            //Reduce rendering speed to one frame per second, tweak this to whatever suites you best
            browserSettings.WindowlessFrameRate = 1;

            using (var browser = new ChromiumWebBrowser(url, browserSettings))
            {
                await LoadPageAsync(browser);

                //Get the browser source
                var source = await browser.GetSourceAsync();

                Console.WriteLine(source);

                //Allow for a little delay before attempting to `Dispose` of the ChromiumWebBrowser,
                // some of the background IPC messages need a few extra ticks to compelte,
                // if you perform some more complex operations this is likely not required.
                await Task.Delay(10);
            }

            Console.WriteLine("Press any key to exit");
        }

        public static Task LoadPageAsync(IWebBrowser browser)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                //Wait for while page to finish loading not just the first frame
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
                    tcs.TrySetResult(true);
                }
            };

            browser.LoadingStateChanged += handler;

            return tcs.Task;
        }
    }
}