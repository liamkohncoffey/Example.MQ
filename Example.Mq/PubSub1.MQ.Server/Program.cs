using System;
using Example.MQ.Domain;

namespace PubSub.MQ.Server
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
                Queue = "PubSub1Queue"
            };
            queueProcessor.Start();
            Console.ReadLine();
        }
    }
}