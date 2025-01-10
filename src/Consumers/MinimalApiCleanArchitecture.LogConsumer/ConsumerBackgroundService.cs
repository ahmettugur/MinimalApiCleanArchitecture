using System.Dynamic;
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
        private IChannel? _channel;

        public ConsumerBackgroundService(ILogger<ConsumerBackgroundService> logger, IOptions<RabbitMqConfigModel> rabbitMqConfig, IElasticContext elasticContext)
        {
            _logger = logger;
            _rabbitMqConfig = rabbitMqConfig.Value ?? throw new ArgumentNullException(nameof(rabbitMqConfig));
            _elasticContext = elasticContext;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = _rabbitMqConfig.Hostname,
                UserName = _rabbitMqConfig.Username,
                Password = _rabbitMqConfig.Password,
                Port = _rabbitMqConfig.Port
            };

            _connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken:cancellationToken);


            await _channel.ExchangeDeclareAsync("LoggerQueue", ExchangeType.Fanout, true,cancellationToken:cancellationToken);
            await _channel.QueueDeclareAsync(queue: "LoggerQueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null,cancellationToken:cancellationToken);
            await _channel.QueueBindAsync("LoggerQueue", "LoggerQueue", "",cancellationToken:cancellationToken);
            await _channel.BasicQosAsync(0, 1, false,cancellationToken:cancellationToken);


            _logger.LogInformation($"Queue [LoggerQueue] is waiting for messages.");


            await base.StartAsync(cancellationToken:cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection?.CloseAsync(cancellationToken);
            
            _channel?.CloseAsync(cancellationToken);
            _logger.LogInformation("RabbitMQ connection is closed");

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                stoppingToken.ThrowIfCancellationRequested();
                var consumer = new AsyncEventingBasicConsumer(_channel!);
                consumer.ReceivedAsync += async (_, ea) =>
                {
                    var data = Encoding.UTF8.GetString(ea.Body.Span);
                    ExpandoObject log = JsonConvert.DeserializeObject<ExpandoObject>(data)!;
                    try
                    {
                        var response = await _elasticContext.IndexCustomAsync($"logstash", log, stoppingToken);
                        if (response.IsValid)
                            _channel?.BasicAckAsync(ea.DeliveryTag, false,stoppingToken);
                        else
                            _channel?.BasicNackAsync(ea.DeliveryTag, false, true,stoppingToken);
                    }
                    catch (Exception)
                    {
                        _channel?.BasicNackAsync(ea.DeliveryTag, false, true,stoppingToken);
                    }
                };
                _channel?.BasicConsumeAsync(queue: "LoggerQueue",
                    autoAck: false,
                    consumer: consumer,cancellationToken:stoppingToken);
            }

            await Task.Delay(100, stoppingToken);
        }

    }
}
