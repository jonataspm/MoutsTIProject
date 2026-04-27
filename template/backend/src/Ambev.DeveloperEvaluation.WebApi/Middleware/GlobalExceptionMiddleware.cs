using System.Text.Json;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, type, error, detail) = exception switch
        {
            ValidationException vex => (StatusCodes.Status400BadRequest, "ValidationError", "Validation Failed", string.Join(" | ", vex.Errors.Select(e => e.ErrorMessage))),
            KeyNotFoundException knf => (StatusCodes.Status404NotFound, "ResourceNotFound", knf.Message, knf.Message),
            UnauthorizedAccessException ua => (StatusCodes.Status401Unauthorized, "AuthenticationError", ua.Message, ua.Message),
            InvalidOperationException ioe => (StatusCodes.Status400BadRequest, "InvalidOperation", ioe.Message, ioe.Message),
            _ => (StatusCodes.Status500InternalServerError, "ServerError", "An unexpected error occurred", "An unexpected error occurred, try again in a few minutes")
        };

        context.Response.StatusCode = statusCode;

        var payload = new
        {
            type,
            error,
            detail
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload, jsonOptions));
    }
}
