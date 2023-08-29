using GoogleMapInfo;
using microservice_map_info.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var jaegerHost = builder.Configuration.GetValue<string>("openTelemetry:jaegerHost");
var otel= builder.Services.AddOpenTelemetry();
otel.WithTracing(providerBuilder=>
    {
        providerBuilder
        .SetResourceBuilder(ResourceBuilder
        .CreateDefault()
        .AddService(env.ApplicationName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter(options =>
        {
            options.AgentHost = jaegerHost;
        })
        .AddConsoleExporter();
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<GoogleDistanceApi>();
builder.Services.AddControllers();
builder.Services.AddHttpClient("GoogleDistanceApi", client =>
{
   
    // ...
}).UseHttpClientMetrics();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<DistanceInfoService>();


app.Run();
