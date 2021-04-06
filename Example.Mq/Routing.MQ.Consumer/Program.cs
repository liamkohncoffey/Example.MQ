using System;
using Example.MQ.Domain;

namespace Routing.MQ.Consumer
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
                Queue = "RPCQueue"
            };
            queueProcessor.StartAck();
            Console.ReadLine();
        }
    }
}