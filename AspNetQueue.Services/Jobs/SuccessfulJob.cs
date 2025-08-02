using AspNetQueue.Services.JobQueue;
using AspNetQueue.Services.Jobs.Parameters;
using LanguageExt;

namespace AspNetQueue.Services.Jobs;

public sealed class SuccessfulJob(IGenericService someScopedDependency) : IJob<SuccessfulJob, SuccessfulJobParameters>
{
    public static string JobName => 
        nameof(SuccessfulJob);
    public static List<string> ConflictedJobs => 
        [nameof(FailingJob)];

    public async Task<Unit> Run(IJobContext<SuccessfulJobParameters> context, CancellationToken ct)
    {
        var result = await someScopedDependency.SomeDependencyMethod();

        if (result)
        { 
            context.LogProgress("managed to fetch dependency");
        }

        context.LogProgress($"Attempting to fetch from db... job params: someParameter: {context.Parameters.someParameter} count: {context.Parameters.count}");

        await Task.Delay(5000);

        context.LogProgress("Success! job is complete");

        return Unit.Default;
    }
}
