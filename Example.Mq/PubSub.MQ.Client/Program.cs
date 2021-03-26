using System;
using System.Collections.Generic;
using Example.MQ.Domain;

namespace PubSub.MQ.Client
{
    class Program
    {
        private const string Exchange = "PubSubExchange";
        private const string PubSubQueue1 = "PubSubQueue1";
        private const string PubSubQueue2 = "PubSubQueue2";
        
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
                        PubSubQueue1,
                        PubSubQueue2
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