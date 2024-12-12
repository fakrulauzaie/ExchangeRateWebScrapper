using System.Collections.Concurrent;
using System.Collections.Immutable;
using WebScrapperRazor.Models;

namespace WebScrapperRazor.Services
{
    public class PageCyclingService
    {
        public static ImmutableList<ExchangeRate> _allRates { get; private set; }
            = ImmutableList<ExchangeRate>.Empty;
        private static int _currentPageIndex = 0;
        private const int PageSize = 10;
        public static DateTime LastScraped { get; private set; }
        public static DateTime LastPageUpdate { get; private set; } = DateTime.MinValue;

        public static void UpdateRates(IEnumerable<ExchangeRate> exchangeRates, DateTime scrapeUpdatedAt)
        {
            _allRates = ImmutableList.CreateRange(exchangeRates);
            _currentPageIndex = 0;
            LastPageUpdate = DateTime.Now;
            LastScraped = scrapeUpdatedAt;
        }

        public static List<ExchangeRate> GetNextPage()
        {
            if (!_allRates.Any())
                return new List<ExchangeRate>();

            // Check if 2 minutes have passed since last update
            if ((DateTime.Now - LastPageUpdate).TotalMinutes >= 2)
            {
                int totalPages = (int)Math.Ceiling((double)_allRates.Count / PageSize);
                _currentPageIndex = (_currentPageIndex + 1) % totalPages;
                LastPageUpdate = DateTime.Now;
            }

            // Return the current page
            return _allRates
                .Skip(_currentPageIndex * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public static int GetTotalRateCount() => _allRates.Count;
    }
}