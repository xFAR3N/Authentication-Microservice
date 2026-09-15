using AuthenticationMicroservice.Application.Common.Exceptions;
using AuthenticationMicroservice.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationMicroservice.API.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled error occured: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, "Data validation error"),
                InvalidCredentialException => (StatusCodes.Status401Unauthorized, "Incorrect credentials"),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                UserAlreadyExistsException => (StatusCodes.Status409Conflict, "User with this data already exists"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error occured")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = httpContext.Request.Path,
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
