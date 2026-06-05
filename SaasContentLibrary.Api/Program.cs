using MediatR;
using Scalar.AspNetCore;
using SaasContentLibrary.Application;
using SaasContentLibrary.Infrastructure;
using SaasContentLibrary.Api.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var contents = app.MapGroup("/content-blocks")
    .WithName("CreateContentBlock")
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict);

contents.MapPost("/", async 
    (CreateContentBlockRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var result = await sender.Send(request.ToCommand(), cancellationToken);
    return result.IsSuccess 
    ? (IResult)TypedResults.Created($"/v1/content-blocks/{result.Value}", result.Value) 
    : result.Error.ToProblemDetails();
});

app.UseHttpsRedirection();

app.Run();
