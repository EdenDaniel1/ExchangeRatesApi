using System.Text;
using System.Text.Json;
using Types;

namespace RateFetcher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient = new();

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var accountId = Environment.GetEnvironmentVariable("XE_ACCOUNT_ID");
        var apiKey = Environment.GetEnvironmentVariable("XE_API_KEY");

        if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Missing XE_ACCOUNT_ID or XE_API_KEY environment variables. Exiting.");
            return;
        }

        var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{accountId}:{apiKey}"));
        var urls = new[]
        {
            "https://xecdapi.xe.com/v1/convert_from.json/?from=USD&to=ILS&amount=1",
            "https://xecdapi.xe.com/v1/convert_from.json/?from=EUR&to=ILS,USD,GBP&amount=1",
            "https://xecdapi.xe.com/v1/convert_from.json/?from=GBP&to=ILS&amount=1"
        };
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var rateList = new List<ExchangeRate>();

            try
            {
                foreach (var url in urls)
                {
                    var response = await _httpClient.GetStringAsync(url, stoppingToken);
                    var jsonDoc = JsonDocument.Parse(response);
                    var fromCurrency = jsonDoc.RootElement.GetProperty("from").GetString();
                    var ratesArray = jsonDoc.RootElement.GetProperty("to");

                    foreach (var rateItem in ratesArray.EnumerateArray())
                    {
                        var toCurrency = rateItem.GetProperty("quotecurrency").GetString();
                        var rate = rateItem.GetProperty("mid").GetDouble();
                        var pair = $"{fromCurrency}{toCurrency}";
                        var lastUpdated = DateTime.UtcNow;

                        rateList.Add(new ExchangeRate
                        {
                            PairName = pair,
                            Rate = rate,
                            LastUpdateTime = lastUpdated
                        });
                    }
                }

                var jsonOutput = JsonSerializer.Serialize(rateList, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(Constants.FilePath, jsonOutput, stoppingToken);

                _logger.LogInformation("Rates written to JSON at {time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching or writing rates.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
