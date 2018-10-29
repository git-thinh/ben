using System;
using System.Windows.Forms;
using Skybound.Gecko;
using System.Runtime.InteropServices;
using System.Net;
using System.Reflection;
using System.IO;
using System.Web;
using System.Text;
using System.Diagnostics;

namespace en2
{
    class App
    {

        static App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (se, ev) =>
            {
                Assembly asm = null;
                string comName = ev.Name.Split(',')[0];

                string resourceName = @"DLL\" + comName + ".dll";
                var assembly = Assembly.GetExecutingAssembly();
                resourceName = typeof(App).Namespace + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
                //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (Stream stream = File.OpenRead("bin/" + comName + ".dll"))
                {
                    if (stream == null)
                    {
                        //Debug.WriteLine(resourceName);
                    }
                    else
                    {
                        byte[] buffer = new byte[stream.Length];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                                ms.Write(buffer, 0, read);
                            buffer = ms.ToArray();
                        }
                        asm = Assembly.Load(buffer);
                    }
                }
                return asm;
            };
        }

        [STAThread]
        public static void Main(string[] args)
        {
            var http = new HttpProxyServer();
            http.Start("http://*:56789/");

            //System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
            //// active SSL 1.1, 1.2, 1.3 for WebClient request HTTPS
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | (SecurityProtocolType)3072 | (SecurityProtocolType)0x00000C00 | SecurityProtocolType.Tls;

            //Xpcom.Initialize(@"D:\temp\xulrunner");
            Xpcom.Initialize(@"bin");
            //GeckoPreferences.User["javascript.enabled"] = false;
            //GeckoPreferences.User["security.warn_viewing_mixed"] = false;
            //GeckoPreferences.User["plugin.state.flash"] = 0;
            //GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;

            Application.Run(new MyForm());
            http.Stop();
        }
    }

    class MyForm : Form
    {
        private TabControl m_tabControl;

        public MyForm()
        {
            this.Width = 800;
            this.Height = 600;

            m_tabControl = new TabControl();
            m_tabControl.Dock = DockStyle.Fill;

            AddTab();

            Controls.Add(m_tabControl);

            m_tabControl.ControlRemoved += delegate
            {
                if (m_tabControl.TabCount == 0)
                {
                    AddTab();
                }
            };

        }

        //protected void ModifyElements(GeckoElement element, string tagName, Action<GeckoElement> mod)
        //{
        //    while (element != null)
        //    {
        //        if (element.TagName == tagName)
        //        {
        //            mod(element);
        //        }
        //        ModifyElements(element.FirstChild as GeckoElement, tagName, mod);
        //        element = (element.NextSibling as GeckoElement);
        //    }
        //}

        //protected void TestModifyingDom(GeckoWebBrowser browser)
        //{
        //    GeckoElement g = browser.Document.DocumentElement;
        //    ModifyElements(g, "BODY", e =>
        //    {
        //        for (int i = 1; i < 4; ++i)
        //        {
        //            var newElement = g.OwnerDocument.CreateElement(String.Format("h{0}", i));
        //            newElement.TextContent = "Geckofx added this text.";
        //            g.InsertBefore(newElement, e);
        //        }
        //    });
        //}

        //protected void DisplayElements(GeckoElement g)
        //{
        //    while (g != null)
        //    {
        //        Console.WriteLine("tag = {0} value = {1}", g.TagName, g.TextContent);
        //        DisplayElements(g.FirstChild as GeckoElement);
        //        g = (g.NextSibling as GeckoElement);
        //    }

        //}

        //protected void TestQueryingOfDom(GeckoWebBrowser browser)
        //{
        //    GeckoElement g = browser.Document.DocumentElement;
        //    DisplayElements(g);
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        GeckoInputElement username = new GeckoInputElement(browser.Document.GetElementsByName("email")[0].DomObject);
        //        username.Value = "YourEMAIL";
        //        GeckoInputElement password = new GeckoInputElement(browser.Document.GetElementsByName("password")[0].DomObject);
        //        password.Value = "YourPassword";
        //        browser.Navigate("javascript:void(document.forms[0].submit())");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        protected void AddTab()
        {
            var tabPage = new TabPage();
            tabPage.Text = "blank";
            var browser = new GeckoWebBrowser() {  };
            browser.Dock = DockStyle.Fill;

            tabPage.DockPadding.Top = 25;
            tabPage.Dock = DockStyle.Fill;

            // add a handler showing how to view the DOM
            //browser.DocumentCompleted += (s, e) => TestQueryingOfDom(browser);

            // add a handler showing how to modify the DOM.
            //browser.DocumentCompleted += (s, e) => TestModifyingDom(browser);

            AddToolbarAndBrowserToTab(tabPage, browser);

            m_tabControl.TabPages.Add(tabPage);
            tabPage.Show();
            m_tabControl.SelectedTab = tabPage;

            // Uncomment this to stop links from navigating.
            // browser.DomClick += StopLinksNavigating;

        }

        public string f_link_getHtmlOnline(string url)
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

        /// <summary>
        /// An example event handler for the DomClick event.
        /// Prevents a link click from navigating.
        /// </summary>
        ////void StopLinksNavigating(object sender, GeckoDomEventArgs e)
        ////{
        ////    if (sender != null && e != null && e.Target != null && e.Target.TagName != null)
        ////    {
        ////        GeckoElement clicked = e.Target;

        ////        // prevent clicking on Links from navigation to the
        ////        if (clicked.TagName == "A")
        ////        {
        ////            e.Handled = true;
        ////            MessageBox.Show(sender as IWin32Window, String.Format("You clicked on Link {0}", clicked.GetAttribute("href")));
        ////        }

        ////    }
        ////}

        static string html = string.Empty;

        protected void AddToolbarAndBrowserToTab(TabPage tabPage, GeckoWebBrowser browser)
        {
            TextBox urlbox = new TextBox();
            urlbox.Top = 0;
            urlbox.Width = 200;
            //urlbox.Text = "https://getfirebug.com/releases/lite/1.2/";
            //urlbox.Text = "file:///G:/en2/build/view/demo.html";
            //urlbox.Text = "file:///G:/en2/build/view/demo.html";

            Button nav = new Button();
            nav.Text = "Go";
            nav.Left = 200;

            Button newTab = new Button();
            newTab.Text = "NewTab";
            newTab.Left = 200 + nav.Width;

            Button closeTab = new Button();
            closeTab.Text = "Close";
            closeTab.Left = 200 + nav.Width + newTab.Width;

            browser.DocumentCompleted += (se, ev) =>
            {
                if (html == string.Empty)
                {
                    string url = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over", text = string.Empty, _fix_lib = string.Empty;

                    Debug.WriteLine("#-> " + url);

                    text = f_link_getHtmlOnline(url);

                    string head = text.Split(new string[] { "<body" }, StringSplitOptions.None)[0], s = "<div" + text.Substring(head.Length + 5);
                    int posH1 = s.ToLower().IndexOf("<h1");
                    if (posH1 != -1) s = s.Substring(posH1, s.Length - posH1);

                    head = Html.f_html_Format(url, head);
                    s = Html.f_html_Format(url, s);

                    //if (File.Exists("view/fix.html")) _fix_lib = File.ReadAllText("view/fix.html");
                    text = head.Replace("<head>", @"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />" + _fix_lib) + "<body><article id=___body><!--START_BODY-->" + s + "<!--END_BODY--></article></body></html>";
                    html = s;
                }
                browser.Document.Body.InnerHtml = html;

                //Debug.WriteLine(browser.Document.Body.InnerHtml)

                string firebuglite_bookmark_run = "javascript:var firebug=document.createElement('script');firebug.setAttribute('src','http://localhost:56789/firebug-lite-compressed.js');document.body.appendChild(firebug);(function(){if(window.firebug.version){firebug.init();}else{setTimeout(arguments.callee);}})();void(firebug);";
                browser.Navigate(firebuglite_bookmark_run);
            };

            nav.Click += delegate
            {
                // use javascript to warn if url box is empty.
                if (string.IsNullOrEmpty(urlbox.Text.Trim())) browser.Navigate("javascript:alert('hey try typing a url!');");
                browser.Navigate(urlbox.Text);
                tabPage.Text = urlbox.Text;
            };

            newTab.Click += delegate { AddTab(); };

            closeTab.Click += delegate
            {
                m_tabControl.Controls.Remove(tabPage);
                browser.Dispose();
            };

            tabPage.Controls.Add(urlbox);
            tabPage.Controls.Add(nav);
            tabPage.Controls.Add(newTab);
            tabPage.Controls.Add(closeTab);
            tabPage.Controls.Add(browser);
        }
    }
}
