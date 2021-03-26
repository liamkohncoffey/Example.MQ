using System;

namespace OneWay.MQ.Client
{
    class Program
    {
       
        
        static void Main()
        {
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();
            Console.WriteLine();

            var messageCount = 0;
            var sender = new RabbitSender();

            Console.WriteLine("Press enter key to send a message");
            while (true)
            {
                var line = Console.ReadLine();

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                    break;

                if (key.Key == ConsoleKey.Enter)
                {
                    var message = string.Format("Message: {0} Count:{1}", line, messageCount);
                    Console.WriteLine(string.Format("Sending - {0}", message));
                    sender.Send(message);
                    messageCount++;
                }
            }
            
            Console.ReadLine();
        }
    }
}