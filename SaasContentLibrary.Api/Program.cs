using SaasContentLibrary.Api;
using SaasContentLibrary.Api.Common;
using SaasContentLibrary.Application;
using SaasContentLibrary.Infrastructure;
using Scalar.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails(); // enables IProblemDetailsService for global exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("ContentLibrary")!,
        name: "postgres",
        tags: new[] { "ready", "db" });

const string ServiceName = "saas-content-library-api";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
    .AddService(
        serviceName: ServiceName,
        serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0")
    .AddAttributes(new Dictionary<string, object>
    {
        ["deployment.environment"] = builder.Environment.EnvironmentName,
        ["host.name"] = Environment.MachineName
    }))
    .WithTracing(tracing => tracing
    .AddAspNetCoreInstrumentation(options =>
    {
        options.Filter = httpContext =>
        !httpContext.Request.Path.StartsWithSegments("/health") &&
        !httpContext.Request.Path.StartsWithSegments("/openapi") &&
        !httpContext.Request.Path.StartsWithSegments("/scalar");
    })
    .AddHttpClientInstrumentation()
    .AddNpgsql()
    .AddOtlpExporter(options => 
    {
        options.Endpoint = new Uri("http://127.0.0.1:4318");
        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
    })
    .AddConsoleExporter())
.WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddRuntimeInstrumentation()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://127.0.0.1:4318");
        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
    })
    .AddConsoleExporter());

//builder.Logging.AddOpenTelemetry(options =>
//{
//    options.IncludeScopes = true;
//    options.IncludeFormattedMessage = true;
//    options.ParseStateValues = true;
//    options.AddOtlpExporter();
//    options.AddConsoleExporter();
//});

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
    app.UseHttpsRedirection();

var contents = app.MapGroup("/v1/content-blocks")
    .WithTags("ContentBlock");

contents.MapPost("/", ContentBlockEndpoints.CreateContentBlock).WithName("CreateContentBlock");

contents.MapPost("/{id:guid}/versions", ContentBlockEndpoints.AddDraftVersion).WithName("AddDraftVersion");

contents.MapPost("/{id:guid}/versions/{versionId:guid}/submit", ContentBlockEndpoints.SubmitForReview).WithName("SubmitForReview");

contents.MapPost("/{id:guid}/versions/{versionId:guid}/approve", ContentBlockEndpoints.ApproveVersion).WithName("ApproveVersion");

contents.MapPost("/{id:guid}/archive", ContentBlockEndpoints.Archive).WithName("Archive");


contents.MapGet("/", ContentBlockEndpoints.ListContentBlocks).WithName("ListContentBlocks");

contents.MapGet("/{id:guid}", ContentBlockEndpoints.GetContentBlock).WithName("GetContentBlock");

contents.MapGet("/{id:guid}/versions", ContentBlockEndpoints.GetVersionHistory).WithName("GetVersionHistory");

// Liveness (no db)
app.MapHealthChecks("/health/live");
// Readiness (with db)
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();
