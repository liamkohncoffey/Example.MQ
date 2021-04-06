using System.Collections.Generic;

namespace Example.MQ.Domain
{
    public class RabbitMqSetup
    {
        public string Exchange { get; set; }
        public List<QueuesMq> QueueMqs { get; set; }
    }
}