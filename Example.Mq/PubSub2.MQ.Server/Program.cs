using System;
using Example.MQ.Domain;

namespace PubSub2.MQ.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ queue processor");
            Console.WriteLine();
            Console.WriteLine();

            var queueProcessor = new RabbitConsumer()
            {
                Enabled = true,
                Queue = "PubSub2Queue"
            };
            queueProcessor.Start();
            Console.ReadLine();
        }
    }
}