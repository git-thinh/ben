using System;
using System.Windows.Forms;
using System.Net;
using System.Reflection;
using System.IO;
using System.Web;
using System.Text;
using System.Diagnostics;
using CefSharp;
using SeasideResearch.LibCurlNet;
using System.Linq;

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
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;

            string pathCache = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Cache");
            if (!Directory.Exists(pathCache)) Directory.CreateDirectory(pathCache);

            Settings settings = new Settings() { UserAgent = "Chrome7", CachePath = pathCache };
            BrowserSettings browserSettings = new BrowserSettings() { PageCacheDisabled = true };

            if (!CEF.Initialize(settings, browserSettings))
            {
                Console.WriteLine("Couldn't initialise CEF");
                return;
            }

            //CEF.RegisterScheme("local", new LocalSchemeHandlerFactory());
            //CEF.RegisterScheme("test", new TestSchemeHandlerFactory());
            //CEF.RegisterJsObject("bound", new BoundObject());

            Application.Run(new MyForm());

            CEF.Shutdown();
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
        
        protected void AddTab()
        {
            var tabPage = new TabPage();
            tabPage.Text = "blank";
            var browser = new CefWebBrowser();
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
        
        
        static string html = string.Empty;

        protected void AddToolbarAndBrowserToTab(TabPage tabPage, CefWebBrowser browser)
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

            //browser.PropertyChanged += (se, ev) =>
            //{
            //    if (ev.PropertyName != "IsBrowserInitialized") return;

            //    if (html == string.Empty)
            //    {
            //        string url = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over", text = string.Empty, _fix_lib = string.Empty;

            //        Debug.WriteLine("#-> " + url);

            //        text = f_link_getHtmlOnline(url);

            //        string head = text.Split(new string[] { "<body" }, StringSplitOptions.None)[0], s = "<div" + text.Substring(head.Length + 5);
            //        int posH1 = s.ToLower().IndexOf("<h1");
            //        if (posH1 != -1) s = s.Substring(posH1, s.Length - posH1);

            //        head = Html.f_html_Format(url, head);
            //        s = Html.f_html_Format(url, s);

            //        //if (File.Exists("view/fix.html")) _fix_lib = File.ReadAllText("view/fix.html");
            //        text = head.Replace("<head>", @"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />" + _fix_lib) + "<body><article id=___body><!--START_BODY-->" + s + "<!--END_BODY--></article></body></html>";
            //        html = s;
            //    }
            //    browser.Reload();

            //    //    browser.Document.Body.InnerHtml = html;

            //    //Debug.WriteLine(browser.Document.Body.InnerHtml)

            //    //string firebuglite_bookmark_run = "javascript:var firebug=document.createElement('script');firebug.setAttribute('src','http://localhost:56789/firebug-lite.1.2.js');document.body.appendChild(firebug);(function(){if(window.firebug.version){firebug.init();}else{setTimeout(arguments.callee);}})();void(firebug);";
            //    //browser.Navigate(firebuglite_bookmark_run);
            //};

            nav.Click += delegate
            {
                // use javascript to warn if url box is empty.
                if (string.IsNullOrEmpty(urlbox.Text.Trim())) browser.Load("javascript:alert('hey try typing a url!');");
                browser.Load(urlbox.Text);
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
