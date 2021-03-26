using System;
using System.Text;
using RabbitMQ.Client;

namespace OneWay.MQ.Client
{
    public class RabbitSender : IDisposable
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "myQueueFromCode";
        private const string ExchangeName = "myExchangeFromCode";
        private const bool IsDurable = true;
        private const string VirtualHost = "";
        private int Port = 0;

        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;


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
            Console.WriteLine("QueueName: {0}", QueueName);
            Console.WriteLine("ExchangeName: {0}", ExchangeName);
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
            
            _model.QueueDeclare(QueueName, true, false, false, null);
            Console.WriteLine("Queue Created");
            
            _model.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
            Console.WriteLine("Exchange Created");
            
            _model.QueueBind(QueueName, ExchangeName, "RK");
            Console.WriteLine("Exchange Bound To Key");
        }

        public void Send(string message)
        {
            //Setup properties
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;

            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);

            //Send message
            _model.BasicPublish(ExchangeName, QueueName, properties, messageBuffer);
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
