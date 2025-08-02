using Microsoft.Extensions.Logging;

namespace AspNetQueue.Services.JobQueue;

public interface IJobContext
{
    string JobName { get; }
    ILogger Logger { get; }
    void LogProgress(string message);
}
public interface IJobContext<TParameters> : IJobContext
{
    TParameters Parameters { get; }
}
internal sealed class JobContext<TParameters>(string jobName, ILogger logger, TParameters parameters, Action<string> logProgress) : IJobContext<TParameters>
{
    public string JobName { get; } = jobName;
    public ILogger Logger { get; } = logger;
    public TParameters Parameters { get; } = parameters;
    public void LogProgress(string message) => logProgress(message);
}
