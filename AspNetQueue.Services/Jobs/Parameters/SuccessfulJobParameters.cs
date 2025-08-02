namespace AspNetQueue.Services.Jobs.Parameters;

public sealed record SuccessfulJobParameters(string someParameter, int count)
{ 
    public static SuccessfulJobParameters New(string someParameter, int count) =>
        new SuccessfulJobParameters(someParameter, count);
}
