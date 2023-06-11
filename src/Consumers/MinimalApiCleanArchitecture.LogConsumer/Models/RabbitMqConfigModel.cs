namespace MinimalApiCleanArchitecture.LogConsumer.Models;

public class RabbitMqConfigModel
{
    public string VHostname { get; set; } = null!;
    public string Hostname { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Exchange { get; set; } = null!;
    public string ExchangeType { get; set; } = null!;
    public int Port { get; set; }
}
