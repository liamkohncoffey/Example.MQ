using System;
using System.Collections.Generic;
using Example.MQ.Domain;

namespace RPC.MQ.Client
{
    class Program
    {
        private const string Exchange = "RPCExchange";
        private const string Queue = "RPCQueue";
        
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
                    QueueMqs = new List<QueuesMq>
                    {
                        new QueuesMq
                        {
                            Queue = Queue,
                            RoutingKey = "RK"
                        }
                    }
                }
            });
            sender.InitRpcClient();
            
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
                    sender.SendWithResponse(message, "RK", Exchange);
                    messageCount++;
                }
            }
            
            Console.ReadLine();
        }
    }
}