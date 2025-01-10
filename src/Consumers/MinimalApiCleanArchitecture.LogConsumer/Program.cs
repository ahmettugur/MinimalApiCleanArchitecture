using MinimalApiCleanArchitecture.LogConsumer.ElasticContexts;
using MinimalApiCleanArchitecture.LogConsumer.Models;
using MinimalApiCleanArchitecture.LogConsumer;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.Configure<RabbitMqConfigModel>(builder.Configuration.GetSection("RabbitMqConfig"));
builder.Services.Configure<ElasticSearchConfigModel>(builder.Configuration.GetSection("ElasticSearchConfig"));
builder.Services.AddSingleton<IElasticContext, ElasticContext>();

builder.Services.AddHostedService<ConsumerBackgroundService>();

var host = builder.Build();
host.Run();
