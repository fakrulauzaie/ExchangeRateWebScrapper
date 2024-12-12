using Microsoft.AspNetCore.Mvc.RazorPages;
using WebScrapperRazor.Services;
using WebScrapperRazor.Models;

public class ExchangeRatesModel : PageModel
{
    public List<ExchangeRate> ExchangeRates { get; set; } = new List<ExchangeRate>();
    public int TotalRates { get; set; }
    public int CurrentPage { get; set; }
    public DateTime LastScraped { get; set; } 
    public void OnGet()
    {
        // Get the next page of products
        ExchangeRates = PageCyclingService.GetNextPage();
        TotalRates = PageCyclingService.GetTotalRateCount();
        LastScraped = PageCyclingService.LastScraped;
    }
}