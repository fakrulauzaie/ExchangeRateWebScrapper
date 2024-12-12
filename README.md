# WebScrapperRazor

An ASP.NET Razor application designed to scrape exchange rate data from HSBC website's page and display the rates in a cycling table on a webpage. It scrapes the exchange rate data hourly and cycles through 10 exchange rates every 2 minutes.

---

## Features

- **Exchange Rate Scraper**: Periodically scrapes exchange rate data every 1 hour from HSBC page.
- **Table Display**: Displays 10 exchange rates at a time, cycling every 2 minutes.
- **Logging**: Uses Serilog for application logging.

---

## How It Works

1. **Scraper Background Service**:
   - Runs as a hosted background service in the app.
   - Scrapes exchange rate data from `https://www.hsbc.com.my/investments/products/foreign-exchange/currency-rate/` every 1 hour.
   - Stores the scraped data in a thread-safe manner.

2. **Frontend Display**:
   - Renders exchange rate data in a table.
   - Server-side control: The server stores all the exchange rates and sends a set of 10 exchange rates to the client at a time. Every 2 minutes, the server cycles to the next set of 10 exchange rates from the stored data and sends it to the client.
   - The table on client side then refreshes automatically every 2 minutes to display the next set of 10 exchange rates.
   - Includes a timestamp showing the last update time.

3. **Logging**:
   - All scraping operations, errors, and general application activity are logged using Serilog.
   - Logs are stored in both console and file outputs.
