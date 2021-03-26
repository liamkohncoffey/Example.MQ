using System;
using Example.MQ.Domain;

namespace PubSub.MQ.Consumer1
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
                Queue = "PubSubQueue1"
            };
            queueProcessor.Start();
            Console.ReadLine();
        }
    }
}