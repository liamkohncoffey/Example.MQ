using System;
using System.Collections.Generic;
using Example.MQ.Domain;

namespace Worker.MQ.Client
{
    class Program
    {
        private const string Exchange = "WorkerExchange";
        private const string Worker1Queue = "Worker1Queue";
        
        static void Main()
        {
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();
            Console.WriteLine();

            var messageCount = 0;
            var sender = new RabbitSender();
            sender.SetupExchangeAndQueue(new List<RabbitMqSetup>
            {
                new RabbitMqSetup
                {
                    Exchange = Exchange,
                    Queues = new List<string>
                    {
                        Worker1Queue
                    }
                }
            });
            Console.WriteLine("Press enter key to send a message");
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                if (!string.IsNullOrEmpty(line))
                {
                    var message =  $"Message: {line} Count:{messageCount}";
                    Console.WriteLine($"Sending - {message}");
                    sender.Send(message, Exchange);
                    messageCount++;
                }
            }
            
            Console.ReadLine();
        }
    }
}