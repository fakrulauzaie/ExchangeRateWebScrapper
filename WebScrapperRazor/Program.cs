using Serilog;
using WebScrapperRazor.Scrapers;
using WebScrapperRazor.Services;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration)
.CreateLogger();

Log.Logger = logger; // Set as global logger

builder.Host.UseSerilog(logger);

// Register services
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ExchangeRateScraper>();
builder.Services.AddHostedService<ExchangeRateBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

try
{
    Log.Information("Starting web application at {Time}", DateTime.Now);
    app.Run();
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    Log.Fatal(ex, "Application terminated unexpectedly at {Time}", DateTime.Now);
    throw;
}
finally
{
    Log.Information("Shutting down application...");
    Log.CloseAndFlush();
}
