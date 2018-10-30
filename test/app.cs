using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var http = new HttpProxyServer();
            http.Start("http://*:3399/");

            Console.ReadLine();
        }


        static void Main9(string[] args)
        {
            ProcessStartInfo psi = new ProcessStartInfo("node.exe", "app.js");
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            Process p = Process.Start(psi);
            Console.WriteLine("{0} is active: {1}", p.Id, !p.HasExited);
            p.StandardInput.WriteLine("SIGTERM");
            //p.StandardInput.WriteLine("\x3");
            Console.WriteLine(p.StandardOutput.ReadToEnd());
            Console.WriteLine("{0} is active: {1}", p.Id, !p.HasExited);
            Console.ReadLine();
        }

        static void Main1(string[] args)
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.FileName = "node.exe";
            proc.StartInfo.Arguments = "-i";
            proc.Start();
            proc.BeginOutputReadLine();

            proc.StandardInput.WriteLine("2 + 2;");
            //proc.StandardInput.WriteLine(" console.log('Exit after 3 seconds...');  setTimeout(function(){ process.exit(); }, 3000).suppressOut;");
            proc.OutputDataReceived += proc_OutputDataReceived;
            proc.WaitForExit();

            Console.ReadLine();
            Console.WriteLine("{0} is active: {1}", proc.Id, !proc.HasExited);
            proc.StandardInput.WriteLine("\x3");
            Console.WriteLine(proc.StandardOutput.ReadToEnd());
            Console.WriteLine("{0} is active: {1}", proc.Id, !proc.HasExited);
            Console.ReadLine();

        }

        static void proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        static void Main0(string[] args)
        {
            //Process.Start("CMD.exe", @"/C node.exe bundle\main.js");
            //Process.Start("CMD.exe", @"/C node.exe app.js");

            //https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output 
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            //proc.StartInfo.FileName = "CMD.exe";
            //proc.StartInfo.Arguments = @"/C node.exe app.js";
            proc.StartInfo.FileName = "node.exe";
            proc.StartInfo.Arguments = @" app.js";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.RedirectStandardError = false;
            //process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            proc.Start();
            //proc.WaitForExit(); 

            int pid = proc.Id;

            Console.WriteLine(pid);
            Console.ReadLine();

            //Process.GetProcessById(pid).Kill();
            proc.Close();


            Console.WriteLine("{0} is active: {1}", proc.Id, !proc.HasExited);
            //proc.StandardInput.WriteLine("\x3");
            //Console.WriteLine(proc.StandardOutput.ReadToEnd());
            Console.WriteLine("{0} is active: {1}", proc.Id, !proc.HasExited);
            Console.ReadLine();


        }




        ////import in the declaration for GenerateConsoleCtrlEvent
        //[DllImport("kernel32.dll", SetLastError = true)]
        //static extern bool GenerateConsoleCtrlEvent(ConsoleCtrlEvent sigevent, int dwProcessGroupId);
        //public enum ConsoleCtrlEvent
        //{
        //    CTRL_C = 0,
        //    CTRL_BREAK = 1,
        //    CTRL_CLOSE = 2,
        //    CTRL_LOGOFF = 5,
        //    CTRL_SHUTDOWN = 6
        //}

        ////set up the parents CtrlC event handler, so we can ignore the event while sending to the child
        //public static volatile bool SENDING_CTRL_C_TO_CHILD = false;
        //static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        //{
        //    e.Cancel = SENDING_CTRL_C_TO_CHILD;
        //}

        //the main method..
        static int Main55(string[] args)
        {
            //hook up the event handler in the parent
            //Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            //spawn some child process
            System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
            proc.RedirectStandardInput = true;
            proc.RedirectStandardOutput = true;
            proc.UseShellExecute = false;
            //psi.Arguments = "childProcess.exe";
            proc.CreateNoWindow = false;
            proc.FileName = "CMD.exe";
            proc.Arguments = "/C node.exe app.js";
            Process p = new Process();
            p.StartInfo = proc;

            p.EnableRaisingEvents = true;
            p.Exited += nodeExited;

            p.Start();

            int p0 = Process.GetCurrentProcess().Id;
            int p1 = p.Id;
            Console.WriteLine(string.Format("{0} -> {1}", p0, p1));

            //sned the ctrl-c to the process group (the parent will get it too!)
            //SENDING_CTRL_C_TO_CHILD = true;
            //GenerateConsoleCtrlEvent(ConsoleCtrlEvent.CTRL_C, p.SessionId);


            Console.WriteLine("Enter to exit node...");
            Console.ReadLine();
            //p.StandardInput.WriteLine(" console.log('Exit after 3 seconds...');  setTimeout(function(){ process.exit(); }, 3000).suppressOut;");
            //p.WaitForExit();
            //p.Close();

            //Process.GetProcessById(p1).Kill("SIGTERM");

            //p.StandardInput.WriteLine("SIGTERM");

            //Process.GetProcessById(p1).Kill();
            //Console.WriteLine("Enter to exit program ...");
            //Console.ReadLine();

            //SENDING_CTRL_C_TO_CHILD = false;

            //note that the ctrl-c event will get called on the parent on background thread
            //so you need to be sure the parent has handled and checked SENDING_CTRL_C_TO_CHILD
            //already before setting it to false. 1000 ways to do this, obviously.

            //Console.ReadLine();

            Console.WriteLine("Exit program after 3 seconds ....");
            //Process.GetProcessById(p0).Kill();
            Thread.Sleep(1000);

            //get out....
            return 0;
        }

        private static void nodeExited(object sender, EventArgs e)
        {
            Console.WriteLine("exit node ...........");
        }
    }
}