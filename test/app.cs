using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Process.Start("CMD.exe", @"/C node.exe bundle\main.js");
            //Process.Start("CMD.exe", @"/C node.exe app.js");
            
            //https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output 
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "CMD.exe";
            process.StartInfo.Arguments = @"/C node.exe app.js";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            //process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();
            process.WaitForExit();
            Console.ReadKey();
        }
    }
}