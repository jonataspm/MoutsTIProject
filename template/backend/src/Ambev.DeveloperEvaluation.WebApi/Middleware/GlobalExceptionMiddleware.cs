using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Text.Json;

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

        var (statusCode, message, errors) = exception switch
        {
            ValidationException vex => (StatusCodes.Status400BadRequest, "Validation Failed", vex.Errors.Select(error => (ValidationErrorDetail)error)),
            KeyNotFoundException knf => (StatusCodes.Status404NotFound, knf.Message, default),
            UnauthorizedAccessException ua => (StatusCodes.Status401Unauthorized, ua.Message, default),
            InvalidOperationException ioe => (StatusCodes.Status400BadRequest, ioe.Message, default),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred, try again in a few minutes", default)
        };

        context.Response.StatusCode = statusCode;

        var payload = new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload, jsonOptions));
    }
}
