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
        private EventingBasicConsumer _consumer;
        private string _replyQueueName;
        private IBasicProperties _props;


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
        }

        public void SetupExchangeAndQueue(IEnumerable<RabbitMqSetup> rabbitMqSetups)
        {
            foreach (var rabbitMqSetup in rabbitMqSetups)
            {
                _model.ExchangeDeclare(rabbitMqSetup.Exchange, ExchangeType.Fanout);
                Console.WriteLine("Exchange Created");
                
                foreach (var queue in rabbitMqSetup.QueueMqs)
                {
                    _model.QueueDeclare(queue.Queue, true, false, false, null);
                    Console.WriteLine("Queue Created");
                    
                    _model.QueueBind(queue.Queue, rabbitMqSetup.Exchange, queue.RoutingKey);
                    Console.WriteLine("Exchange Bound To Key");
                }
            }
        }
        
        public void InitRpcClient()
        {
            _replyQueueName = _model.QueueDeclare().QueueName;
            
            _consumer = new EventingBasicConsumer(_model);

            _props = _model.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            _props.CorrelationId = correlationId;
            _props.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    Console.WriteLine($"Response Recieved: {response}");
                }
            };
        }

        public void SendWithResponse(string message, string routingKey, string exchange)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _model.BasicPublish(exchange, routingKey, _props, messageBytes);

            _model.BasicConsume(
                consumer: _consumer,
                queue: _replyQueueName,
                autoAck: true);
        }
        
        public void Send(string message, string routingKey, string exchange)
        {
            //Setup properties
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;

            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);

            //Send message
            _model.BasicPublish(exchange, routingKey, properties, messageBuffer);
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
