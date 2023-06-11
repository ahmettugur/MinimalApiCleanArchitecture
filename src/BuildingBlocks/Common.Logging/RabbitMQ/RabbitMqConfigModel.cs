using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging.RabbitMQ
{
    public class RabbitMqConfigModel
    {
        public string? VHostname { get; set; }
        public string? Hostname { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Exchange { get; set; }
        public string? ExchangeType { get; set; }
        public int Port { get; set; }
    }
}
