using System.Text;
using Microsoft.Extensions.Options;
using MinimalApiCleanArchitecture.LogConsumer.ElasticContexts;
using MinimalApiCleanArchitecture.LogConsumer.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MinimalApiCleanArchitecture.LogConsumer;

public class Consumer
{
    private readonly RabbitMqConfigModel _rabbitMqConfig;
    private readonly IElasticContext _elasticContext;
    public Consumer(IOptions<RabbitMqConfigModel> rabbitMqConfig, IElasticContext elasticContext)
    {
        _rabbitMqConfig = rabbitMqConfig?.Value ?? throw new ArgumentNullException(nameof(rabbitMqConfig));
        _elasticContext = elasticContext;
    }

    public Task Run()
    {
        Console.WriteLine($"_rabbitMqConfig.Hostname: {_rabbitMqConfig.Hostname}");
        Console.WriteLine($"_rabbitMqConfig.Username: {_rabbitMqConfig.Username}");
        Console.WriteLine($"_rabbitMqConfig.Password: {_rabbitMqConfig.Password}");
        Console.WriteLine($"_rabbitMqConfig.Port: {_rabbitMqConfig.Port}");
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqConfig.Hostname,
            UserName = _rabbitMqConfig.Username,
            Password = _rabbitMqConfig.Password,
            Port = _rabbitMqConfig.Port,
            DispatchConsumersAsync = true
        };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("LoggerQueue", ExchangeType.Fanout, true, false);
            channel.QueueDeclare(queue: "LoggerQueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.QueueBind("LoggerQueue", "LoggerQueue", "");
            channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var data = Encoding.UTF8.GetString(ea.Body.Span);
                object log = JsonConvert.DeserializeObject(data)!;
                try
                {
                    var response = await _elasticContext.IndexCustomAsync($"logstash", log);
                    if (response.IsValid)
                        channel.BasicAck(ea.DeliveryTag, false);
                    else
                        channel.BasicNack(ea.DeliveryTag, false, true);
                }
                catch (Exception)
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };
            channel.BasicConsume(queue: "LoggerQueue",
                                 autoAck: false,
                                 consumer: consumer);
            Console.WriteLine(" press X for shotdown.");
            Console.ReadLine();

        }

        return Task.CompletedTask;
    }
}