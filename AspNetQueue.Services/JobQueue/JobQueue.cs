using System.Collections.Concurrent;
using AspNetQueue.Services.Jobs.Parameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetQueue.Services.JobQueue;

public interface IJobQueue
{
    (bool IsAdded, string Message) QueueJob<TJob, TParameters>(TParameters parameters)
        where TJob : class, IJob<TJob, TParameters>;

    (bool IsAdded, string Message) QueueJob<TJob>()
        where TJob : class, IJob<TJob, EmptyParameters>;

    Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    IDictionary<string, JobStatus> GetAllJobStatuses();
}

public sealed class JobQueue(IServiceScopeFactory serviceScopeFactory, ILogger<JobQueue> logger) : IJobQueue
{
    private readonly ConcurrentQueue<Func<CancellationToken, Task>> workItems = new();
    private readonly ConcurrentDictionary<string, bool> runningTasks = new();
    private readonly ConcurrentDictionary<string, JobStatus> jobStatuses = new();
    private readonly SemaphoreSlim signal = new(0);

    public (bool IsAdded, string Message) QueueJob<TJob, TParameters>(TParameters parameters)
        where TJob : class, IJob<TJob, TParameters>
    {
        var runningConflicts = TJob.ConflictedJobs.Where(runningTasks.ContainsKey).ToList();
        if (runningConflicts.Count > 0)
        {
            return (false, $"Cannot queue job {TJob.JobName} because conflicted jobs are running: {string.Join(", ", runningConflicts)}");
        }

        if (!runningTasks.TryAdd(TJob.JobName, true))
        {
            return (false, $"Job {TJob.JobName} is already running");
        }

        jobStatuses.AddOrUpdate(TJob.JobName, JobStatus.Queued(), (_, _) => JobStatus.Queued());

        var workItem = CreateJobWorkItem<TJob, TParameters>(TJob.JobName, parameters);
        workItems.Enqueue(workItem);

        signal.Release();
        return (true, $"Successfully queued job: {TJob.JobName}");
    }

    public (bool IsAdded, string Message) QueueJob<TJob>() where TJob : class, IJob<TJob, EmptyParameters>
    {
        return QueueJob<TJob, EmptyParameters>(EmptyParameters.New);
    }

    private Func<CancellationToken, Task> CreateJobWorkItem<TJob, TParameters>(string jobName, TParameters parameters)
        where TJob : class, IJob<TJob, TParameters>
    {
        return async (CancellationToken ct) =>
        {
            try
            {
                UpdateJobStatus(jobName, status =>
                {
                    status.Status = "Running";
                    status.StartedAt = DateTime.UtcNow;
                });

                using var scope = serviceScopeFactory.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService<TJob>();

                var context = new JobContext<TParameters>(
                    jobName,
                    logger,
                    parameters,
                    progressMessage => UpdateJobStatus(jobName, status =>
                    {
                        status.ProgressLog.Add($"{DateTime.UtcNow:HH:mm:ss} - {progressMessage}");
                        status.Status = progressMessage;
                        logger.LogInformation("[{JobName}] {Message}", jobName, progressMessage);
                    })
                );

                await job.Run(context, ct);

                UpdateJobStatus(jobName, status =>
                {
                    status.Status = "Completed";
                    status.CompletedAt = DateTime.UtcNow;
                });

                logger.LogInformation("Job {JobName} completed successfully", jobName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Job {JobName} failed", jobName);

                UpdateJobStatus(jobName, status =>
                {
                    status.Status = "Failed";
                    status.CompletedAt = DateTime.UtcNow;
                    status.ErrorMessage = ex.Message;
                });
            }
            finally
            {
                runningTasks.TryRemove(jobName, out _);
            }
        };
    }

    private void UpdateJobStatus(string jobName, Action<JobStatus> updateAction) =>
        jobStatuses.AddOrUpdate(jobName, JobStatus.Queued(), (_, existing) =>
        {
            updateAction(existing);
            return existing;
        });

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        await signal.WaitAsync(cancellationToken);
        workItems.TryDequeue(out var workItem);
        return workItem!;
    }

    public IDictionary<string, JobStatus> GetAllJobStatuses() =>
        new Dictionary<string, JobStatus>(jobStatuses);
}
