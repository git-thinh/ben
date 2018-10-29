using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

/* 
 * AutoResxTranslator
 * by Salar Khalilzadeh
 * 
 * https://autoresxtranslator.codeplex.com/
 * Mozilla Public License v2
 
void test_run_v1(string text)
{
    //IsBusy(true);
    GooTranslateService_v1.TranslateAsync(
        text, "en", "vi", string.Empty,
        (success, result, type) =>
        {
            //SetResult(result, type);
            //IsBusy(false);
            Console.WriteLine("\r\n -> " + text + " (" + type + "): " + result);
            Tracer.WriteLine(text + "(" + type + "): " + result);
        });
}

void test_run_v2(string text)
{
    //IsBusy(true);
    GooTranslateService_v2.TranslateAsync(
        text, "en", "vi", string.Empty,
        (success, result, type) =>
        {
            //SetResult(result, type);
            //IsBusy(false);
            Console.WriteLine("\r\n -> " + text + " (" + type + "): " + result);
            Tracer.WriteLine(text + "(" + type + "): " + result);
        });
}

//IsBusy(true);

GTranslateService.TranslateAsync(
    text, "en", "vi", string.Empty,
    (success, result, type) =>
    {
        //SetResult(result, type);
        //IsBusy(false);
        Console.WriteLine(text + "(" + type + "): " + result);
        Trace.WriteLine(text + "(" + type + "): " + result);
    });

 */
namespace System
{    
    public class GooTranslateService_v1
    {
        private const string RequestUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:55.0) Gecko/20100101 Firefox/55.0";
        private const string RequestGoogleTranslatorUrl = "http://www.google.com/translate_t?hl=en&ie=UTF8&langpair={0}|{1}&text={2}";
        
        public static void TranslateAsync(
            oEN_TRANSLATE_GOOGLE_MESSAGE oTranslateObject,
            string text,
            string sourceLng,
            string destLng,
            string textTranslatorUrlKey,
            TranslateCallBack callBack)
        {
            var request = CreateWebRequest(text, sourceLng, destLng, textTranslatorUrlKey);

            oTranslateObject.translateCallBack = callBack;
            oTranslateObject.webRequest = request;

            request.BeginGetResponse(TranslateRequestCallBack, oTranslateObject);
        }

        public static bool Translate(
            string text,
            string sourceLng,
            string destLng,
            string textTranslatorUrlKey,
            out string result,
            out string type)
        {
            var request = CreateWebRequest(text, sourceLng, destLng, textTranslatorUrlKey);
            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    result = "Response is failed with code: " + response.StatusCode;
                    type = string.Empty;
                    return false;
                }

                using (var stream = response.GetResponseStream())
                {
                    string _output, _type;
                    var succeed = ReadGoogleTranslatedResult(stream, out _output, out _type);
                    result = _output;
                    type = _type;
                    return succeed;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                type = string.Empty;
                return false;
            }
        }

        static WebRequest CreateWebRequest( 
            string text,
            string lngSourceCode,
            string lngDestinationCode,
            string textTranslatorUrlKey)
        {
            text = HttpUtility.UrlEncode(text);

            ////string temp = HttpUtility.UrlEncode(input.Replace(" ", "---"));
            //string temp = HttpUtility.UrlEncode(input);
            ////temp = temp.Replace("-----", "%20");
            var url = string.Format(RequestGoogleTranslatorUrl, lngSourceCode, lngDestinationCode, text);
            //string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", temp, "en|vi");

            var create = (HttpWebRequest)WebRequest.Create(url);
            create.UserAgent = RequestUserAgent;
            create.Timeout = 50 * 1000;
            return create;
        }

        private static void TranslateRequestCallBack(IAsyncResult ar)
        {
            var otran = (oEN_TRANSLATE_GOOGLE_MESSAGE)ar.AsyncState;
            var request = otran.webRequest;
            var callback = otran.translateCallBack;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.EndGetResponse(ar);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    otran.success = false;
                    otran.mean_vi = "Response is failed with code: " + response.StatusCode;
                    callback(otran);
                    return;
                }

                using (var stream = response.GetResponseStream())
                {
                    string output, type;
                    var succeed = ReadGoogleTranslatedResult(stream, out output, out type);

                    otran.success = true;
                    otran.type = type;
                    otran.mean_vi = output;
                    callback(otran);
                }
            }
            catch (Exception ex)
            {
                otran.success = false;
                otran.mean_vi = "Request failed.\r\n" + ex.Message;
                callback(otran);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        ///  the main trick :)
        /// </summary>
        static bool ReadGoogleTranslatedResult(Stream rawdata, out string result, out string type)
        {
            string text;
            using (var reader = new StreamReader(rawdata, Encoding.UTF8))
            {
                text = reader.ReadToEnd();
            }

            try
            {
                result = string.Empty;
                type = string.Empty;

                string s = HttpUtility.HtmlDecode(text);

                int p = s.IndexOf("id=result_box");
                if (p > 0)
                    s = "<span " + s.Substring(p, s.Length - p);
                p = s.IndexOf("</div>");
                if (p > 0)
                {
                    s = s.Substring(0, p);
                    s = s.Replace("<br>", "¦");
                    s = HttpUtility.HtmlDecode(s);
                    result = Regex.Replace(s, @"<[^>]*>", String.Empty);
                }
                if (result != string.Empty)
                {
                    string[] rs = result.Split('¦').Select(x => x.Trim()).ToArray();
                }
                return true;
            }
            catch (Exception ex)
            {
                result = ex.Message;
                type = string.Empty;
                return false;
            }
        }
    }

}
