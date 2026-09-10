using System.Net;
using System.Text.Json;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Exceptions;
using Todo.Identity.Infrastructure.Exceptions;

namespace Todo.Identity.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, code, message) = exception switch
        {
            // Domain exceptions → 400
            DomainException de => (HttpStatusCode.BadRequest, de.Code, de.Message),

            // Application exceptions → map by type
            InvalidCredentialsException ae => (HttpStatusCode.Unauthorized, ae.Code, ae.Message),
            AccountDeactivatedException ae => (HttpStatusCode.Forbidden, ae.Code, ae.Message),
            UserNotFoundException ae => (HttpStatusCode.NotFound, ae.Code, ae.Message),
            RegistrationFailedException ae => (HttpStatusCode.BadRequest, ae.Code, ae.Message),
            EmailAlreadyExistsException ae => (HttpStatusCode.Conflict, ae.Code, ae.Message),
            ConfirmPasswordNotCorrectException ae => (HttpStatusCode.BadRequest, ae.Code, ae.Message),
            PasswordResetFailedException ae => (HttpStatusCode.BadRequest, ae.Code, ae.Message),
            UserInfoUpdateFailedException ae => (HttpStatusCode.BadRequest, ae.Code, ae.Message),

            // Infrastructure exceptions
            InvalidTokenException ie => (HttpStatusCode.Unauthorized, ie.Code, ie.Message),
            InvalidTokenUserIdException ie => (HttpStatusCode.Unauthorized, ie.Code, ie.Message),
            InfrastructureException ie => (HttpStatusCode.InternalServerError, ie.Code, ie.Message),

            // Catch-all
            _ => (HttpStatusCode.InternalServerError, "Internal.ServerError", "An unexpected error occurred")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = new
            {
                code,
                message
            }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
