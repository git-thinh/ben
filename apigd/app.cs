using System;

namespace apigd
{
    

    class Program
    {
        static void Main(string[] args)
        {
            var http = new HttpProxyServer(new GooDriver());
            http.Start("http://*:3399/");

            Console.ReadLine();
        }
    }
}