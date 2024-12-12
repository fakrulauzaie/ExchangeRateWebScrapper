using HtmlAgilityPack;
using WebScrapperRazor.Models;


namespace WebScrapperRazor.Scrapers
{
    public class ExchangeRateScraper
    {
        private readonly ILogger<ExchangeRateScraper> _logger;

        public ExchangeRateScraper(ILogger<ExchangeRateScraper> logger)
        {
            _logger = logger;
        }

        public List<ExchangeRate> ScrapeExchangeRates(HtmlDocument document)
        {
            var exchangeRates = new List<ExchangeRate>();

            // Find all related tables in the page
            var tables = document.DocumentNode.QuerySelectorAll("table.desktop");

            foreach (var table in tables)
            {
                try
                {
                    // Find all rows in the table body
                    var rows = table.QuerySelectorAll("tbody tr");

                    foreach (var row in rows)
                    {
                        try
                        {
                            // Extract data from each row
                            var currencyCodeElement = row.QuerySelector("th[scope='row']");
                            var currencyNameElement = row.QuerySelectorAll("td")[0];
                            var ttSellElement = row.QuerySelectorAll("td")[1];
                            var ttBuyElement = row.QuerySelectorAll("td")[2];

                            if (currencyCodeElement == null || currencyNameElement == null ||
                                ttSellElement == null || ttBuyElement == null)
                            {
                                _logger.LogWarning("Incomplete row data found, skipping");
                                continue;
                            }

                            // Clean and parse the data
                            var currencyCode = HtmlEntity.DeEntitize(currencyCodeElement.InnerText.Trim());
                            var currencyName = HtmlEntity.DeEntitize(currencyNameElement.InnerText.Trim());

                            var exchangeRate = new ExchangeRate
                            {
                                CurrencyCode = currencyCode,
                                CurrencyName = currencyName,
                                // Use TryParse with out decimal
                                TTSell = decimal.TryParse(
                                    HtmlEntity.DeEntitize(ttSellElement.InnerText.Trim()),
                                    out decimal sellValue) ? sellValue : null,
                                TTBuy = decimal.TryParse(
                                    HtmlEntity.DeEntitize(ttBuyElement.InnerText.Trim()),
                                    out decimal buyValue) ? buyValue : null,
                                FlagUrl = ExchangeRate.GetFlagUrl(currencyCode)
                            };

                            exchangeRates.Add(exchangeRate);

                            _logger.LogInformation(
                                $"Successfully scraped {currencyCode} exchange rate"
                            );
                        }
                        catch (Exception rowEx)
                        {
                            _logger.LogError(rowEx, $"Error processing individual row");
                        }
                    }
                }
                catch (Exception tableEx)
                {
                    _logger.LogError(tableEx, "Error processing table");
                }
            }

            return exchangeRates;
        }
    }
}