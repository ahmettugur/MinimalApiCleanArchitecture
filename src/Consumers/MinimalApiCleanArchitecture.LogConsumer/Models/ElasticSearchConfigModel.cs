namespace MinimalApiCleanArchitecture.LogConsumer.Models;

public class ElasticSearchConfigModel
{
    public string ConnectionString { get; set; } = null!;
    public int PingTimeMilliSeconds { get; set; }
}
