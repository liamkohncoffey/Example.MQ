using System;
using Example.MQ.Domain;

namespace Worker.MQ.Consumer1
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
                Queue = "WorkerQueue1"
            };
            queueProcessor.Start();
            Console.ReadLine();
        }
    }
}