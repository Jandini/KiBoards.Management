namespace KiBoards.Management.Service.Services.Kibana
{
    public class KibanaClientHostedService : BackgroundService
    {
        private readonly KibanaHttpClient _kibanaClient;
        private readonly ILogger<KibanaClientHostedService> _logger;

        public KibanaClientHostedService(KibanaHttpClient kibanaClient, ILogger<KibanaClientHostedService> logger)
        {
            _kibanaClient = kibanaClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    var status = await _kibanaClient.GetStatus(stoppingToken);
                    
                    _logger.LogInformation($"Kibana {status.Version.Number} status is {status.Status.Overall.Level}: {status.Status.Overall.Summary}");

                    if (string.Equals(status.Status.Overall.Level, "available", StringComparison.OrdinalIgnoreCase))
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Get kibana status failed.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            
            try
            {
                _logger.LogInformation("Importing saved objects");

                await _kibanaClient.ImportSavedObjectsAsync("Test.ndjson", stoppingToken);
                
                _logger.LogInformation("Saved objects imported successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Saved objects import failed.");
            }
        }
    }
}
