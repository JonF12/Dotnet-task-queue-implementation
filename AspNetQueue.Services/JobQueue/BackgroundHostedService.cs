using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetQueue.Services.JobQueue;

public sealed class QueuedHostedService(IJobQueue jobQueue, ILogger<QueuedHostedService> logger) : BackgroundService
{
protected async override Task ExecuteAsync(CancellationToken stoppingToken)
{
    logger.LogInformation("Queued Hosted Service is starting.");
    while (!stoppingToken.IsCancellationRequested)
    {
        var workItem = await jobQueue.DequeueAsync(stoppingToken);
        try
        {
            await workItem(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
        }
    }
    logger.LogInformation("Queued Hosted Service is stopping.");
}
}
