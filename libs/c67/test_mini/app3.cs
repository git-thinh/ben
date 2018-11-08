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
            Cef.Initialize(_CONST.CEF_SETTINGS, false, null);

            MainAsync();

            Console.WriteLine(":::::::::::::::::::::::> DONE: Press any key to exit");
            // We have to wait for something, otherwise the process will exit too soon.
            Console.ReadKey();

            // Clean up Chromium objects. You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();

            return 0;
        }

        private static async void MainAsync()
        { 
            using (var browser = new ChromiumWebBrowser(_CONST.URL, _CONST.BROWSER_SETTINGS))
            {
                browser.RequestHandler = new RequestHandler();
                browser.FrameLoadStart += (se, ev) =>
                {
                    Console.WriteLine("?> " + ev.Url);
                    if (ev.Url == _CONST.URL)
                        browser.Stop();
                };

                await LoadPageAsync(browser);

                //Get the browser source
                var source = await browser.GetSourceAsync();
                Console.WriteLine("\r\n\r\n\r\n");
                Console.WriteLine(source);

                //Allow for a little delay before attempting to `Dispose` of the ChromiumWebBrowser,
                // some of the background IPC messages need a few extra ticks to compelte,
                // if you perform some more complex operations this is likely not required.
                await Task.Delay(10);
            }

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

        public static Task LoadPageAsync(IWebBrowser browser)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                Console.WriteLine(":: Wait for while page to finish loading not just the first frame");

                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
                    tcs.TrySetResult(true);

                    // Wait for the screenshot to be taken.
                    //browser.GetTextAsync().ContinueWith(DisplayText);
                }
            };

            browser.LoadingStateChanged += handler;

            return tcs.Task;
        }
    }
}