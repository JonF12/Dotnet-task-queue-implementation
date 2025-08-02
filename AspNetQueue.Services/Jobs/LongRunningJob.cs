using AspNetQueue.Services.JobQueue;
using AspNetQueue.Services.Jobs.Parameters;
using LanguageExt;

namespace AspNetQueue.Services.Jobs;

public sealed class LongRunningJob : IJob<LongRunningJob, EmptyParameters>
{
    public static string JobName => 
        nameof(LongRunningJob);
    public static List<string> ConflictedJobs => 
        [nameof(FailingJob)];

    public async Task<Unit> Run(IJobContext<EmptyParameters> context, CancellationToken ct)
    {
        context.LogProgress("Attempting long operation");

        await Task.Delay(1000 * 60);

        context.LogProgress("Long operation was successful");

        return Unit.Default;
    }
}
