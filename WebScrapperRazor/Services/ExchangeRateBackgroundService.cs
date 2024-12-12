using System.Collections.Concurrent;
using System.Collections.Immutable;
using HtmlAgilityPack;
using WebScrapperRazor.Services;
using WebScrapperRazor.Scrapers;
using WebScrapperRazor.Models;


namespace WebScrapperRazor.Services
{
    public class ExchangeRateBackgroundService : BackgroundService
    {
        private readonly ILogger<ExchangeRateBackgroundService> _logger;
        private readonly ExchangeRateScraper _scraper;
        private static readonly TimeSpan _scrapeInterval = TimeSpan.FromHours(1);

        public static ImmutableList<ExchangeRate> ScrapedExchangeRates { get; private set; }
            = ImmutableList<ExchangeRate>.Empty;

        public ExchangeRateBackgroundService(
            ILogger<ExchangeRateBackgroundService> logger,
            ExchangeRateScraper scraper)
        {
            _logger = logger;
            _scraper = scraper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var web = new HtmlWeb();
                    var document = web.Load("https://www.hsbc.com.my/investments/products/foreign-exchange/currency-rate/");

                    // Scrape rates
                    var exchangeRates = _scraper.ScrapeExchangeRates(document);

                    ScrapedExchangeRates = [.. exchangeRates];

                    PageCyclingService.UpdateRates(exchangeRates, DateTime.Now);

                    _logger.LogInformation($"Hourly scrape completed at {DateTime.Now}. Total rates: {exchangeRates.Count}");

                    // Wait for next interval
                    await Task.Delay(_scrapeInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during exchange rate scraping");

                    // Wait before retrying
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
