using LanguageExt;

namespace AspNetQueue.Services.JobQueue;

public interface IJob<TSelf, TParameters> where TSelf : IJob<TSelf, TParameters>
{
    public static abstract string JobName { get; }
    public static abstract List<string> ConflictedJobs { get; }
    public Task<Unit> Run(IJobContext<TParameters> context, CancellationToken ct);
}
