namespace WebScrapperRazor.Models
{
    public class ExchangeRate
    {
        public required string CurrencyCode { get; set; }
        public required string CurrencyName { get; set; }
        public decimal? TTSell { get; set; }
        public decimal? TTBuy { get; set; }
        public string? FlagUrl { get; set; }

        public string FormatRate(decimal? rate)
        {
            return rate.HasValue ? rate.Value.ToString("F4") : "N/A";
        }

        public static string GetFlagUrl(string currencyCode)
        {
            return $"https://flagcdn.com/w320/{currencyCode.Substring(0, 2).ToLower()}.png";
        }
    }
}