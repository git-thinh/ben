using SeasideResearch.LibCurlNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace demo
{
    class Program
    {
        static string URL = "https://dictionary.cambridge.org/grammar/british-grammar/above-or-over";
        static void Main(string[] args)
        {
            //URL = "http://httpbin.org/";
            URL = "https://vnexpress.net/";

            var dataRecorder = new EasyDataRecorder();

            Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_DEFAULT);
            try
            {
                using (Easy easy = new Easy())
                {
                    easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, (Easy.WriteFunction)dataRecorder.HandleWrite);


                    Easy.SSLContextFunction sf = new Easy.SSLContextFunction(OnSSLContext);
                    easy.SetOpt(CURLoption.CURLOPT_SSL_CTX_FUNCTION, sf);

                    easy.SetOpt(CURLoption.CURLOPT_URL, URL);
                    easy.SetOpt(CURLoption.CURLOPT_CAINFO, "ca-bundle.crt");


                    easy.Perform();
                }
            }
            finally
            {
                Curl.GlobalCleanup();
            }

            string s = Encoding.UTF8.GetString(dataRecorder.Written.ToArray());

            Console.WriteLine(s);


            Console.WriteLine("\r\n[END]");
            Console.ReadLine();
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
