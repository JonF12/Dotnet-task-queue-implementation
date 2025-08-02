namespace AspNetQueue.Services.Jobs.Parameters;

public sealed record EmptyParameters
{ 
    public static EmptyParameters New =>
        new EmptyParameters();
}
