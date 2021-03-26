using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Example.MQ.Domain
{
    public class RabbitSender : IDisposable
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const bool IsDurable = true;
        private const string VirtualHost = "";
        private int Port = 0;
        private string _responseQueue = "TempQueue";

        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;
        private IBasicConsumer _consumer;
        


        public RabbitSender()
        {
            DisplaySettings();
            SetupRabbitMq();
        }

        private void DisplaySettings()
        {
            Console.WriteLine("Host: {0}", HostName);
            Console.WriteLine("Username: {0}", UserName);
            Console.WriteLine("Password: {0}", Password);
            Console.WriteLine("VirtualHost: {0}", VirtualHost);
            Console.WriteLine("Port: {0}", Port);
            Console.WriteLine("Is Durable: {0}", IsDurable);
        }
        
        private void SetupRabbitMq()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };
    
            if (string.IsNullOrEmpty(VirtualHost) == false)
                _connectionFactory.VirtualHost = VirtualHost;
            if (Port > 0)
                _connectionFactory.Port = Port;

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            
            //Create dynamic response queue
            _consumer = new EventingBasicConsumer(_model);
            _model.BasicConsume(_responseQueue, false, _consumer);
        }

        public void SetupExchangeAndQueue(IEnumerable<RabbitMqSetup> rabbitMqSetups)
        {
            foreach (var rabbitMqSetup in rabbitMqSetups)
            {
                _model.ExchangeDeclare(rabbitMqSetup.Exchange, ExchangeType.Fanout);
                Console.WriteLine("Exchange Created");
                
                foreach (var queue in rabbitMqSetup.Queues)
                {
                    _model.QueueDeclare(queue, true, false, false, null);
                    Console.WriteLine("Queue Created");
                    
                    _model.QueueBind(queue, rabbitMqSetup.Exchange, "RK");
                    Console.WriteLine("Exchange Bound To Key");
                }
            }
        }

        public void Send(string message, string exchange)
        {
            //Setup properties
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;

            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);

            //Send message
            _model.BasicPublish(exchange, "RK", properties, messageBuffer);
        }
        
        public string SendWithAck(string message, string exchange, TimeSpan timeout)
        {
            var correlationToken = Guid.NewGuid().ToString();

            //Setup properties
            var properties = _model.CreateBasicProperties();
            properties.ReplyTo = _responseQueue;
            properties.CorrelationId = correlationToken;

            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);

            //Send
            var timeoutAt = DateTime.Now + timeout;
            _model.BasicPublish(exchange, "RK", properties, messageBuffer);

            //Wait for response
            while (DateTime.Now <= timeoutAt)
            {
                var basicGet = _consumer.Model.BasicGet(_responseQueue, true);
                if (basicGet != null && basicGet.BasicProperties.CorrelationId == correlationToken)
                {
                    var response = Encoding.Default.GetString(basicGet.Body.Span);
                    _model.ExchangeDelete(_responseQueue);
                    _model.QueueDelete(_responseQueue);
                    return response;
                }
            }
            throw new TimeoutException(@"The response was not returned before the timeout");
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_connection != null)
                _connection.Close();
            
            if (_model != null && _model.IsOpen)
                _model.Abort();

            _connectionFactory = null;

            GC.SuppressFinalize(this);
        }
    }
}
