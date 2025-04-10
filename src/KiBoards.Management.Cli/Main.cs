using Microsoft.Extensions.Logging;

internal class Main(ILogger<Main> logger) 
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Hello World!");
        await Task.CompletedTask;
    }
}
