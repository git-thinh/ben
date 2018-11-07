using SeasideResearch.LibCurlNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace test_libcurl
{
    //https://curl.haxx.se/libcurl/c/example.html

    class Program
    {
        static string URL = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over";
        static void Main(string[] args)
        {
            get1();
            Console.WriteLine("Enter to post ...");
            Console.ReadLine();
            post1();
        }

        static void get1()
        {
            //URL = "http://httpbin.org/";
            //URL = "https://vnexpress.net/";
            URL = "https://azure.microsoft.com/en-us/services/cognitive-services/bing-web-search-api/";
            Console.WriteLine("GET: " + URL);
            File.WriteAllText("libcurl-get.txt", string.Empty);

            var dataRecorder = new EasyDataRecorder();
            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_DEFAULT);
            try
            {
                using (Easy easy = new Easy())
                {
                    easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, (Easy.WriteFunction)dataRecorder.HandleWrite);

                    /* example.com is redirected, so we tell libcurl to follow redirection */
                    easy.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, 1L);

                    Easy.SSLContextFunction sf = new Easy.SSLContextFunction(OnSSLContext);
                    easy.SetOpt(CURLoption.CURLOPT_SSL_CTX_FUNCTION, sf);

                    easy.SetOpt(CURLoption.CURLOPT_URL, URL);

                    /* use a GET to fetch this */
                    //easy.SetOpt(CURLoption.CURLOPT_HTTPGET, 1L);

                    easy.SetOpt(CURLoption.CURLOPT_CAINFO, "ca-bundle.crt");

                    //write cookie
                    easy.SetOpt(CURLoption.CURLOPT_COOKIEJAR, "cookies.txt");

                    easy.Perform();
                }
            }
            finally
            {
                Curl.GlobalCleanup();
            }
            string s = Encoding.UTF8.GetString(dataRecorder.Written.ToArray());
            //Console.WriteLine(s);
            File.WriteAllText("libcurl-get.txt", s);
            Console.WriteLine("\r\n[END-GET]");
        }

        static void post1()
        {
            URL = "https://azure.microsoft.com/en-us/cognitive-services/demo/websearchapi/";
            Console.WriteLine("POST: " + URL);
            File.WriteAllText("libcurl-post.txt", string.Empty);

            var dataRecorder = new EasyDataRecorder();
            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_DEFAULT);
            try
            {
                using (Easy easy = new Easy())
                {
                    easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, (Easy.WriteFunction)dataRecorder.HandleWrite);

                    Easy.SSLContextFunction sf = new Easy.SSLContextFunction(OnSSLContext);
                    easy.SetOpt(CURLoption.CURLOPT_SSL_CTX_FUNCTION, sf);

                    //POST
                    easy.SetOpt(CURLoption.CURLOPT_URL, URL);

                    /* use a POST to fetch this */
                    //easy.SetOpt(CURLoption.CURLOPT_HTTPPOST, 1L);
                    string coo = File.ReadAllText("cookies.txt").Trim();
                    string __RequestVerificationToken = string.Empty;
                    if (coo.Contains("__RequestVerificationToken")) __RequestVerificationToken = coo.Split(new string[] { "__RequestVerificationToken" }, StringSplitOptions.None)[1].Trim();

                    /* Now specify the POST data */
                    easy.SetOpt(CURLoption.CURLOPT_POSTFIELDS, "__RequestVerificationToken=" + __RequestVerificationToken + "&Query=english+due+to+tienganh123&Market=en-us&Safesearch=Strict&freshness=");


                    easy.SetOpt(CURLoption.CURLOPT_CAINFO, "ca-bundle.crt");
                    
                    //read cookie
                    easy.SetOpt(CURLoption.CURLOPT_COOKIEFILE, "cookies.txt");

                    easy.Perform();
                }
            }
            finally
            {
                Curl.GlobalCleanup();
            }
            string s = Encoding.UTF8.GetString(dataRecorder.Written.ToArray());
            //Console.WriteLine(s);
            File.WriteAllText("libcurl-post.txt", s);
            Console.WriteLine("\r\n[END-POST]");
        }

        public static void run()
        {
            try
            {
                Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

                Easy easy = new Easy();

                Easy.WriteFunction wf = new Easy.WriteFunction(OnWriteData);
                easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);

                Easy.SSLContextFunction sf = new Easy.SSLContextFunction(OnSSLContext);
                easy.SetOpt(CURLoption.CURLOPT_SSL_CTX_FUNCTION, sf);

                easy.SetOpt(CURLoption.CURLOPT_URL, URL);
                easy.SetOpt(CURLoption.CURLOPT_CAINFO, "ca-bundle.crt");

                easy.Perform();
                //easy.Cleanup();

                Curl.GlobalCleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            Console.WriteLine("\r\n[BEGIN]");
            Console.Write(System.Text.Encoding.UTF8.GetString(buf));
            return size * nmemb;
        }

        public static CURLcode OnSSLContext(SSLContext ctx, Object extraData)
        {
            // To do anything useful with the SSLContext object, you'll need
            // to call the OpenSSL native methods on your own. So for this
            // demo, we just return what cURL is expecting.
            return CURLcode.CURLE_OK;
        }
    }
}
