using EvoTicketingGRPC;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using TheTicketShop.IService;
using TheTicketShop.Middlewares;
using TheTicketShop.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddOpenTelemetry()
.ConfigureResource(resource => resource.AddService("TheTicketShop"))
.WithMetrics(metrics =>
{
    metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation();
    metrics.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:18890");
    });
})
.WithTracing(tracing =>
{
    tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation();
    tracing.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:18889");
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IBaseService, BaseService>();

builder.Services.AddScoped<TicketService>();

builder.Services.AddGrpcClient<TicketingService.TicketingServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:7206");
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LogEnrichmentMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
