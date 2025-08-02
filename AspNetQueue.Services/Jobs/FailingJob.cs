using AspNetQueue.Services.JobQueue;
using AspNetQueue.Services.Jobs.Parameters;
using LanguageExt;

namespace AspNetQueue.Services.Jobs;

public sealed class FailingJob : IJob<FailingJob, EmptyParameters>
{
    public static string JobName => 
        nameof(FailingJob);
    public static List<string> ConflictedJobs => 
        [nameof(LongRunningJob)];

    public async Task<Unit> Run(IJobContext<EmptyParameters> context, CancellationToken ct)
    {
        context.LogProgress("Attempting to fetch from db...");

        await Task.Delay(1000 * 5);

        context.LogProgress("Fatal error, exception thrown...");

        throw new Exception("Something happened");

        return Unit.Default;
    }
}
