using KiBoards.Management;
using Microsoft.Extensions.Logging;

internal class Main(ILogger<Main> logger, KibanaHttpClient kibanaHttpClient) 
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Making kibana dark");
        await kibanaHttpClient.SetDarkModeAsync(false, null, cancellationToken);
        
        
    }
}
