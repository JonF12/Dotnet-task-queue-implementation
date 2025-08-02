using System.Text.Json.Serialization;

namespace AspNetQueue.Services.JobQueue;

public sealed class JobStatus
{
    public string Status { get; set; } = "Queued";
    [JsonIgnore]
    public DateTime? QueuedAt { get; set; }
    [JsonIgnore]
    public DateTime? StartedAt { get; set; }
    [JsonIgnore]
    public DateTime? CompletedAt { get; set; }
    public List<string> ProgressLog { get; set; } = [];
    public string? ErrorMessage { get; set; }
    public bool IsQueued => QueuedAt.HasValue && !StartedAt.HasValue;
    public bool IsRunning => StartedAt.HasValue && !CompletedAt.HasValue;
    public bool IsCompleted => CompletedAt.HasValue && ErrorMessage == null;
    public bool HasFailed => !string.IsNullOrEmpty(ErrorMessage);
    public string? QueuedAtFormatted => 
        QueuedAt?.ToString("yyyy-MM-dd HH:mm:ss");
    public string? StartedAtFormatted => 
        StartedAt?.ToString("yyyy-MM-dd HH:mm:ss");
    public string? CompletedAtFormatted => 
        CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss");
    public string? Duration => CompletedAt.HasValue && StartedAt.HasValue 
        ? (CompletedAt.Value - StartedAt.Value).ToString(@"hh\:mm\:ss") 
        : null;
    public static JobStatus Queued() => new() { Status = "Queued", QueuedAt = DateTime.UtcNow };
    public static JobStatus Running() => new() { Status = "Running", StartedAt = DateTime.UtcNow };
    public static JobStatus Completed() => new() { Status = "Completed", CompletedAt = DateTime.UtcNow };
    public static JobStatus Failed(string error) => new() { Status = "Failed", CompletedAt = DateTime.UtcNow, ErrorMessage = error };
    private JobStatus() { }
}
