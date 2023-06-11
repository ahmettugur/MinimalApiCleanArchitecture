namespace MinimalApiCleanArchitecture.LogConsumer.Models;

public class BaseLogModel
{
    public BaseLogModel()
    {

    }
    public BaseLogModel(string message)
    {
        Message = message;
    }
    public string? RequestID { get; set; }
    public DateTime RequestTime { get; set; }
    public DateTime ResponseTime { get; set; }
    public double Duration { get; set; }
    public string? Message { get; set; }
}
