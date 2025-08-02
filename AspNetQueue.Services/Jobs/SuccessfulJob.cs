using AspNetQueue.Services.JobQueue;
using AspNetQueue.Services.Jobs.Parameters;
using LanguageExt;

namespace AspNetQueue.Services.Jobs;

public sealed class SuccessfulJob : IJob<SuccessfulJob, SuccessfulJobParameters>
{
    public static string JobName => 
        nameof(SuccessfulJob);
    public static List<string> ConflictedJobs => 
        [nameof(FailingJob)];

    public async Task<Unit> Run(IJobContext<SuccessfulJobParameters> context, CancellationToken ct)
    {
        context.LogProgress($"Attempting to fetch from db... job params: someParameter: {context.Parameters.someParameter} count: {context.Parameters.count}");

        await Task.Delay(1000 * 5);

        context.LogProgress("Fatal error, exception thrown...");

        throw new Exception("Something happened");

        return Unit.Default;
    }
}
