using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;

namespace EventStorePlayGround
{
    class Program
    {
        static string streamName = "balance_stream";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
