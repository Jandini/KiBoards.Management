# KiBoards.Management

[![Build](https://github.com/Jandini/KiBoards.Management/actions/workflows/build.yml/badge.svg)](https://github.com/Jandini/KiBoards.Management/actions/workflows/build.yml)
[![NuGet](https://github.com/Jandini/KiBoards.Management/actions/workflows/nuget.yml/badge.svg)](https://github.com/Jandini/KiBoards.Management/actions/workflows/nuget.yml)

A library for managing Kibana settings and saved objects.



# Quick Start



Add the Kibana saved objects file into your project and set it to be copied to the output.

```xml
<ItemGroup>
	<None Update="Test.ndjson">
		<CopyToOutputDirectory>Always</CopyToOutputDirectory>
	</None>
</ItemGroup>
```





Create a hosted service for the Kibana client that waits for Kibana and imports saved objects from the file.

```c#
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
                _logger.LogWarning($"Get kibana status failed. {ex.Message}");
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
            _logger.LogError($"Saved objects import failed. {ex.Message}");
        }
    }
}
```



Add the Kibana HTTP client and hosted service to the service collection.

```c#
builder.Services.AddHttpClient<KibanaHttpClient>(client => client.BaseAddress = new Uri("http://localhost:5601"));
builder.Services.AddHostedService<KibanaClientHostedService>();
```



---
Created from [JandaBox](https://github.com/Jandini/JandaBox)
Box icon created by [Freepik - Flaticon](https://www.flaticon.com/free-icons/box)
