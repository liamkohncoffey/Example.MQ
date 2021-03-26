using System;
using Example.MQ.Domain;

namespace Worker1.MQ.Server
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
                Queue = "Worker1Queue"
            };
            queueProcessor.Start();
            Console.ReadLine();
        }
    }
}