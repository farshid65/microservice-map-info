// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

static async Task Main(string[] args)
{
    var envName =
Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
?? "Production";
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{envName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets(typeof(Program).Assembly, optional: true)
    .AddCommandLine(args)
    .Build();

    string jagerHost = configuration.GetValue<string>("openTelemetry:jaegerHost");
    using var traceProvider = Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(ResourceBuilder
        .CreateDefault()
        .AddService(typeof(Program).Assembly.GetName().Name))
        .AddHttpClientInstrumentation()
        .AddJaegerExporter(options =>
        {
            options.AgentHost = jagerHost;
        })
        .AddConsoleExporter()
        .Build();

    HttpClient httpClient = new HttpClient();
    string mapInfoUrl = configuration.GetValue<string>("mapInfoUrl");
    httpClient.BaseAddress = new Uri(mapInfoUrl);
    while (true)
    {
        Thread.Sleep(5000);
        try
        {
            string originCity = "Topeka,KS";
            string destinationCity = "Los Angeles,CA";
            var res = await httpClient.GetAsync($"/MapInfo/GetDistance?" +
            "originCity={originCity}" +
            "&destinationCity={destinationCity}");
            string data = await res.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {data}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
        }
    }
}
