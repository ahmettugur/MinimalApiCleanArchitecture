using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApiCleanArchitecture.LogConsumer.ElasticContexts;
using MinimalApiCleanArchitecture.LogConsumer.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MinimalApiCleanArchitecture.LogConsumer
{
    public class ConsumerBackgroundService : BackgroundService
    {
        private readonly ILogger<ConsumerBackgroundService> _logger;
        private readonly RabbitMqConfigModel _rabbitMqConfig;
        private readonly IElasticContext _elasticContext;
        
        private IConnection? _connection;
        private IModel? _channel;

        public ConsumerBackgroundService(ILogger<ConsumerBackgroundService> logger, IOptions<RabbitMqConfigModel> rabbitMqConfig, IElasticContext elasticContext)
        {
            _logger = logger;
            _rabbitMqConfig = rabbitMqConfig.Value ?? throw new ArgumentNullException(nameof(rabbitMqConfig));
            _elasticContext = elasticContext;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = _rabbitMqConfig.Hostname,
                UserName = _rabbitMqConfig.Username,
                Password = _rabbitMqConfig.Password,
                Port = _rabbitMqConfig.Port,
                DispatchConsumersAsync = true
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();


            _channel.ExchangeDeclare("LoggerQueue", ExchangeType.Fanout, true);
            _channel.QueueDeclare(queue: "LoggerQueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _channel.QueueBind("LoggerQueue", "LoggerQueue", "");
            _channel.BasicQos(0, 1, false);


            _logger.LogInformation($"Queue [LoggerQueue] is waiting for messages.");


            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection?.Close();
            
            _channel?.Close();
            _logger.LogInformation("RabbitMQ connection is closed");

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                stoppingToken.ThrowIfCancellationRequested();
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (_, ea) =>
                {
                    var data = Encoding.UTF8.GetString(ea.Body.Span);
                    var log = JsonConvert.DeserializeObject(data);
                    try
                    {
                        var response = await _elasticContext.IndexCustomAsync($"logstash", log, stoppingToken);
                        if (response.IsValid)
                            _channel?.BasicAck(ea.DeliveryTag, false);
                        else
                            _channel?.BasicNack(ea.DeliveryTag, false, true);
                    }
                    catch (Exception)
                    {
                        _channel?.BasicNack(ea.DeliveryTag, false, true);
                    }
                };
                _channel?.BasicConsume(queue: "LoggerQueue",
                    autoAck: false,
                    consumer: consumer);
            }

            await Task.Delay(100, stoppingToken);
        }

    }
}
