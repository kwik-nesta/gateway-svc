using Hangfire.Console;
using Hangfire.Server;
using KwikNesta.Gateway.Svc.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Caching;

namespace KwikNesta.Gateway.Svc.Application.Services
{
    public class PingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PingService> _logger;
        private readonly PingSettings _options;

        public PingService(IHttpClientFactory httpClientFactory,
                           ILogger<PingService> logger,
                           IOptions<PingSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options.Value;
        }

        public async Task PingAllAsync(PerformContext context)
        {
            var client = _httpClientFactory.CreateClient("ServicePinger");
            var progress = context.WriteProgressBar();
            int processed = 0;

            if(_options.ServiceUrls.Count <= 0)
            {
                context.WriteLine("No service URLs configured to ping.");
                return;
            }

            context.WriteLine($"Starting ping job for {_options.ServiceUrls.Count} services...");

            foreach (var url in _options.ServiceUrls)
            {
                try
                {
                    var startTime = DateTimeOffset.UtcNow;

                    context.WriteLine("Pinging {0} started at {1}", url, startTime);
                    _logger.LogInformation("Pinging {0} started at {1}", url, startTime);

                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var endTime = DateTimeOffset.UtcNow;
                        var duration = (endTime - startTime).TotalMilliseconds;

                        context.WriteLine("Pinged {0} successfully at {1}. Duration: {2}ms", url, endTime, duration);
                        _logger.LogInformation("Pinged {0} successfully at {1}. Duration: {2}ms", url, endTime, duration);
                    }
                    else
                    {
                        context.WriteLine("Ping to {0} returned {1}", url, response.StatusCode);
                        _logger.LogWarning("Ping to {0} returned {1}", url, response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    context.WriteLine("Failed to ping {0}: {1}", url, ex.Message);
                    _logger.LogError(ex, "Failed to ping {Url}", url);
                }

                processed++;
                progress.SetValue((int)((processed / (double)_options.ServiceUrls.Count) * 100));
            }

            progress.SetValue(100);
            context.WriteLine("🎯 All pings completed!");
        }
    }
}