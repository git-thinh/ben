using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace System
{
    static class Html
    {
        public static string f_html_getTitle(string html) {
            string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            return title;
        }

        //public static void f_html_getSourceByUrl(string url, Action<string, string> f_callback_fail, Action<string, oPage> f_callback_success)
        //{
        //    try
        //    {
        //        //using (WebClient webClient = new WebClient())
        //        //{
        //        //    var stream = webClient.OpenRead(new Uri(url));
        //        //    using (StreamReader sr = new StreamReader(stream))
        //        //    {
        //        //        string data = url + "{&}" + sr.ReadToEnd();
        //        //        Console.WriteLine(data);
        //        //    }
        //        //}

        //        HttpWebRequest w = (HttpWebRequest)WebRequest.Create(new Uri(url));
        //        w.BeginGetResponse(asyncResult =>
        //        {
        //            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
        //            string _url = request.RequestUri.ToString();
        //            try
        //            {
        //                HttpWebResponse rs = (HttpWebResponse)w.EndGetResponse(asyncResult);
        //                if (rs.StatusCode == HttpStatusCode.OK)
        //                {
        //                    using (StreamReader sr = new StreamReader(rs.GetResponseStream(), System.Text.Encoding.UTF8))
        //                    {
        //                        string data = sr.ReadToEnd();
        //                        string title = Regex.Match(data, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
        //                        if (title != null) title = title.Trim(); else title = string.Empty;

        //                        Console.WriteLine("-> OK: " + _url);
        //                        data = HttpUtility.HtmlDecode(data);

        //                        // Fetch all url same domain in this page ...
        //                        string[] urls = new string[] { };
        //                        //string[] urls = f_html_actractUrl(_url, data);
        //                        data = f_html_Format(_url, data);

        //                        int posH1 = data.ToLower().IndexOf("<h1");
        //                        if (posH1 != -1) data = data.Substring(posH1, data.Length - posH1);

        //                        data = "<!--" + _url + @"--><input id=""___title"" value=""" + title + @""" type=""hidden"">" + data;

        //                        //string domain = f_html_getDomainByUrl(para.Item2.Url);
        //                        //string file = f_html_getPathFileByUrl(para.Item2.Url);

        //                        f_callback_success(_url, new oPage() { Url = _url, Source = data, Urls = urls, Title = title });

        //                        //if (para.Item2.isWriteFileCache)
        //                        //{
        //                        //    if (!Directory.Exists("cache/" + domain)) Directory.CreateDirectory("cache/" + domain);
        //                        //    if (!File.Exists(file))
        //                        //    {
        //                        //        using (FileStream writer = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
        //                        //        {
        //                        //            byte[] buf = Encoding.UTF8.GetBytes(data);
        //                        //            writer.Write(buf, 0, buf.Length);
        //                        //            writer.Close();
        //                        //        }
        //                        //    }
        //                        //}
        //                    }
        //                    rs.Close();
        //                }
        //            }
        //            catch
        //            {
        //                f_callback_fail(url, null);
        //            }
        //        }, w);
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;
        //        f_callback_fail(url, msg);
        //    }
        //}

        public static string f_html_getDomainMainByUrl(string url)
        {
            string[] a = f_html_getDomainByUrl(url).Split('.');
            if (a.Length > 1) return a[a.Length - 2] + "." + a[a.Length - 1];
            return a[0];
        }

        public static string f_html_getDomainByUrl(string url)
        {
            string domain = url.Split('/')[2].Replace(':', '-').ToLower();
            if (domain.StartsWith("www.")) domain = domain.Substring(4);
            return domain;
        }

        public static string f_html_getNameFileByUrl(string url)
        {
            string domain = f_html_getDomainByUrl(url);
            string fn = url.Substring(domain.Length + 8).Replace('/', '_');

            if (fn.EndsWith(".php") || fn.EndsWith(".jsp")) fn = fn.Substring(0, fn.Length - 4);
            else if (fn.EndsWith(".aspx") || fn.EndsWith(".html")) fn = fn.Substring(0, fn.Length - 5);

            if (fn[0] == '_') fn = fn.Substring(1);
            if (fn.Length > 0 && fn[fn.Length - 1] == '_') fn = fn.Substring(0, fn.Length - 1);

            fn = Regex.Replace(fn, @"[^A-Za-z0-9-_]+", string.Empty);
            return fn;
        }

        public static string f_html_getPathFileByUrl(string url)
        {
            string domain = f_html_getDomainByUrl(url),
                   fn = f_html_getNameFileByUrl(url),
                   file = "cache/" + domain + "/" + fn + ".txt";
            return file;
        }

        public static string f_html_Format(string url, string s)
        {
            string si = string.Empty;
            s = Regex.Replace(s, @"<script[^>]*>[\s\S]*?</script>", string.Empty);
            s = Regex.Replace(s, @"<style[^>]*>[\s\S]*?</style>", string.Empty);
            s = Regex.Replace(s, @"<noscript[^>]*>[\s\S]*?</noscript>", string.Empty);
            s = Regex.Replace(s, @"(?s)(?<=<!--).+?(?=-->)", string.Empty).Replace("<!---->", string.Empty);

            //s = Regex.Replace(s, @"<noscript[^>]*>[\s\S]*?</noscript>", string.Empty);
            //s = Regex.Replace(s, @"<noscript[^>]*>[\s\S]*?</noscript>", string.Empty);
            //s = Regex.Replace(s, @"</?(?i:embed|object|frameset|frame|iframe|meta|link)(.|\n|\s)*?>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"</?(?i:base|nav|form|input|fieldset|button|symbol|path|canvas|use|ins|svg|embed|object|frameset|frame|meta|link|noscript)(.|\n|\s)*?>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            //// Remove attribute style="padding:10px;..."
            //s = Regex.Replace(s, @"<([^>]*)(\sstyle="".+?""(\s|))(.*?)>", string.Empty);
            //s = s.Replace(@">"">", ">");

            string[] lines = s.Split(new char[] { '\r', '\n' }, StringSplitOptions.None).Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            s = string.Join(Environment.NewLine, lines);

            ////////int pos = s.ToLower().IndexOf("<body");
            ////////if (pos > 0)
            ////////{
            ////////    s = s.Substring(pos + 5);
            ////////    pos = s.IndexOf('>') + 1;
            ////////    s = s.Substring(pos, s.Length - pos).Trim();
            ////////}

            ////////s = s
            ////////    .Replace(@" data-src=""", @" src=""")
            ////////    .Replace(@"src=""//", @"src=""http://");

            ////////var mts = Regex.Matches(s, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
            ////////if (mts.Count > 0)
            ////////{
            ////////    foreach (Match mt in mts)
            ////////    {
            ////////        string src = mt.Groups[1].Value;
            ////////        s = s.Replace(mt.ToString(), string.Format("{0}{1}{2}", "<p class=box_img___>", mt.ToString(), "</p>"));
            ////////        s = s.Replace(mt.ToString(), @"<p class=___box_img><input class=""___img_src"" value=""" + src + @""" type=""hidden"" /></p>");
            ////////    }
            ////////}
            s = s.Replace("</body>", string.Empty).Replace("</html>", string.Empty).Trim();

            return s;

            //HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(s);
            //string tagName = string.Empty, tagVal = string.Empty;
            //foreach (var node in doc.DocumentNode.SelectNodes("//*"))
            //{
            //    if (node.InnerText == null || node.InnerText.Trim().Length == 0)
            //    {
            //        node.Remove();
            //        continue;
            //    }

            //    tagName = node.Name.ToUpper();
            //    if (tagName == "A")
            //        tagVal = node.GetAttributeValue("href", string.Empty);
            //    else if (tagName == "IMG")
            //        tagVal = node.GetAttributeValue("src", string.Empty);

            //    //node.Attributes.RemoveAll();
            //    node.Attributes.RemoveAll_NoRemoveClassName();

            //    if (tagVal != string.Empty)
            //    {
            //        if (tagName == "A") node.SetAttributeValue("href", tagVal);
            //        else if (tagName == "IMG") node.SetAttributeValue("src", tagVal);
            //    }
            //}

            //si = doc.DocumentNode.OuterHtml;
            ////string[] lines = si.Split(new char[] { '\r', '\n' }, StringSplitOptions.None).Where(x => x.Trim().Length > 0).ToArray();
            //string[] lines = si.Split(new char[] { '\r', '\n' }, StringSplitOptions.None).Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            //si = string.Join(Environment.NewLine, lines);
            //return si;
        }

        //public static string[] f_html_actractUrl(string url, string htm)
        //{
        //    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //    doc.LoadHtml(htm);

        //    string[] auri = url.Split('/');
        //    string uri_root = string.Join("/", auri.Where((x, k) => k < 3).ToArray());
        //    string uri_path1 = string.Join("/", auri.Where((x, k) => k < auri.Length - 2).ToArray());
        //    string uri_path2 = string.Join("/", auri.Where((x, k) => k < auri.Length - 3).ToArray());

        //    var lsURLs = doc.DocumentNode
        //        .SelectNodes("//a")
        //        .Where(p => p.InnerText != null && p.InnerText.Trim().Length > 0)
        //        .Select(p => p.GetAttributeValue("href", string.Empty))
        //        .Select(x => x.IndexOf("../../") == 0 ? uri_path2 + x.Substring(5) : x)
        //        .Select(x => x.IndexOf("../") == 0 ? uri_path1 + x.Substring(2) : x)
        //        .Where(x => x.Length > 1 && x[0] != '#')
        //        .Select(x => x[0] == '/' ? uri_root + x : (x[0] != 'h' ? uri_root + "/" + x : x))
        //        .Select(x => x.Split('#')[0])
        //        .ToList();

        //    //string[] a = htm.Split(new string[] { "http" }, StringSplitOptions.None).Where((x, k) => k != 0).Select(x => "http" + x.Split(new char[] { '"', '\'' })[0]).ToArray();
        //    //lsURLs.AddRange(a);

        //    //????????????????????????????????????????????????????????????????????????????????
        //    //uri_root = "https://dictionary.cambridge.org/grammar/british-grammar/";

        //    var u_html = lsURLs
        //         .Where(x => x.IndexOf(uri_root) == 0)
        //         .Select(x => HttpUtility.UrlDecode(x))
        //         .GroupBy(x => x)
        //         .Select(x => x.First())
        //         //.Where(x =>
        //         //    !x.EndsWith(".pdf")
        //         //    || !x.EndsWith(".txt")

        //         //    || !x.EndsWith(".ogg")
        //         //    || !x.EndsWith(".mp3")
        //         //    || !x.EndsWith(".m4a")

        //         //    || !x.EndsWith(".gif")
        //         //    || !x.EndsWith(".png")
        //         //    || !x.EndsWith(".jpg")
        //         //    || !x.EndsWith(".jpeg")

        //         //    || !x.EndsWith(".doc")
        //         //    || !x.EndsWith(".docx")
        //         //    || !x.EndsWith(".ppt")
        //         //    || !x.EndsWith(".pptx")
        //         //    || !x.EndsWith(".xls")
        //         //    || !x.EndsWith(".xlsx"))
        //         .Distinct()
        //         .ToArray();

        //    //if (!string.IsNullOrEmpty(setting_URL_CONTIANS))
        //    //    foreach (string key in setting_URL_CONTIANS.Split('|'))
        //    //        u_html = u_html.Where(x => x.Contains(key)).ToArray();

        //    //var u_audio = lsURLs.Where(x => x.EndsWith(".mp3")).Distinct().ToArray();
        //    //var u_img = lsURLs.Where(x => x.EndsWith(".gif") || x.EndsWith(".jpeg") || x.EndsWith(".jpg") || x.EndsWith(".png")).Distinct().ToArray();
        //    //var u_youtube = lsURLs.Where(x => x.Contains("youtube.com/")).Distinct().ToArray();

        //    return u_html;
        //}
    }
}
