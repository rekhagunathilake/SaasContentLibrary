using SaasContentLibrary.Api;
using SaasContentLibrary.Api.Common;
using SaasContentLibrary.Application;
using SaasContentLibrary.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails(); // enables IProblemDetailsService for global exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var contents = app.MapGroup("/v1/content-blocks")
    .WithTags("ContentBlock");

contents.MapPost("/", ContentBlockEndpoints.CreateContentBlock).WithName("CreateContentBlock");

contents.MapPost("/{id:guid}/version", ContentBlockEndpoints.AddDraftVersion).WithName("AddDraftVersion");

contents.MapPost("/{id:guid}/version/{versionId:guid}/submit", ContentBlockEndpoints.SubmitForReview).WithName("SubmitForReview");

contents.MapPost("/{id:guid}/version/{versionId:guid}/approve", ContentBlockEndpoints.ApproveVersion).WithName("ApproveVersion");

contents.MapPost("/{id:guid}/version/{versionId:guid}/archive", ContentBlockEndpoints.Archive).WithName("Archive");


contents.MapGet("/", ContentBlockEndpoints.ListContentBlocks).WithName("ListContentBlocks");

contents.MapGet("/{id:guid}", ContentBlockEndpoints.GetContentBlock).WithName("GetContentBlock");

contents.MapGet("/{id:guid}/versions", ContentBlockEndpoints.GetVersionHistory).WithName("GetVersionHistory");

app.UseHttpsRedirection();

app.Run();
