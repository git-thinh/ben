using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading; 

namespace corel
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            app.f_INIT();
            app.f_RUN();
        }
    }
    public class app
    {
        public static void f_INIT()
        {
            ThreadPool.SetMaxThreads(25, 25);

            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
            //try
            //{
            //    // active SSL 1.1, 1.2, 1.3 for WebClient request HTTPS
            //    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
            //        | (SecurityProtocolType)3072
            //        | (SecurityProtocolType)0x00000C00
            //        | SecurityProtocolType.Tls;
            //}
            //catch
            //{
            //    // active SSL 1.1, 1.2, 1.3 for WebClient request HTTPS
            //    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
            //        | SecurityProtocolType.Tls;
            //}
        }

        public static void f_RUN()
        {
            //test_job.f_rpc_Handle();
            //test_job.f_websocket_Handle();
            //test_job.f_JobTestRequestUrl();
            //test_job.f_handle_HTTP_FILE();
            //test_job.f_jobTest_Handle();
            //test_job.f_jobTest_Factory();

            //IpcChannel._test_IpcChannel.RUN();
            //Rpc._test_Rpc.RUN();
            //Google.ProtocolBuffers._test.RUN_1();
            //Google.ProtocolBuffers._test.RUN_2();

            f_EXIT();            
        }
        
        static void f_EXIT()
        {
            Console.WriteLine("Enter to EXIT...");
            Console.ReadLine();
            GC.Collect();
            GC.WaitForPendingFinalizers();            
        }         
    }

}
