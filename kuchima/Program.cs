using System;
using System.Threading;
using kuchima.Hacks;

namespace kuchima
{
    internal class Program
    {
        static void Main()
        {
            Console.Title = "kuchima";

            Console.WriteLine("Grabbing offsets...");
            Offsets.Load();

            Console.WriteLine("Loading...");
            while (!CSGO.Load()) Thread.Sleep(100);

            ESP.StartThread();
            Console.WriteLine("ESP started.");
            Thread.Sleep(-1);
        }
    }
}
