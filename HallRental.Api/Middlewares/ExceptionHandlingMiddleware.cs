using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HallRental.Api.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
            => _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var traceId = context.TraceIdentifier;

            var (statusCode, title, detail, errors) = MapException(context, ex);

            if (statusCode >= 500)
            {
                _logger.LogError(ex,
                    "Unhandled exception. TraceId={TraceId}, Path={Path}, Method={Method}",
                    traceId, context.Request.Path, context.Request.Method);
            }
            else
            {
                _logger.LogWarning(ex,
                    "Handled exception. StatusCode={StatusCode}, TraceId={TraceId}, Path={Path}, Method={Method}",
                    statusCode, traceId, context.Request.Path, context.Request.Method);
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            if (errors is not null)
                problem.Extensions["errors"] = errors;

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private static (int StatusCode, string Title, string Detail, object? Errors) MapException(HttpContext context, Exception ex)
        {
            return ex switch
            {
                ValidationException fv => (
                    StatusCodes.Status400BadRequest,
                    "Validation error",
                    "Request validation failed.",
                    fv.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.ErrorMessage).ToArray()
                        )
                ),

                ArgumentException ae => (
                    StatusCodes.Status400BadRequest,
                    "Bad request",
                    ae.Message,
                    null
                ),

                KeyNotFoundException knf => (
                    StatusCodes.Status404NotFound,
                    "Not found",
                    knf.Message,
                    null
                ),

                InvalidOperationException ioe => (
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    ioe.Message,
                    null
                ),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Internal server error",
                    "An unexpected error occurred.",
                    null
                )
            };
        }
    }
}
