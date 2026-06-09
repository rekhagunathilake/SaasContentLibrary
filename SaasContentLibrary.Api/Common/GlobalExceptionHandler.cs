using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ValidationException = SaasContentLibrary.Application.Common.Exceptions.ValidationException;

namespace SaasContentLibrary.Api.Common
{
    public sealed class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = exception switch
            {
                ValidationException ve => new ProblemDetails {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation errors occurred. See the errors property for details.",
                    Extensions = {
                        ["errors"] = ve.Errors
                    }
                },

                _ => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred. Please try again later."
                }
            };

            if (problemDetails.Status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception,
                    "Unhandled exception occurred while processing {Method} {Path}",
                    httpContext.Request.Method, httpContext.Request.Path);
            }
            else {
                logger.LogWarning(exception,
                    "Handled {ExceptionType} while processing {Method} {Path}",
                    exception.GetType().Name, httpContext.Request.Method, httpContext.Request.Path);
            }

            httpContext.Response.StatusCode = problemDetails.Status!.Value;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
        }
    }
}
