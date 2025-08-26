namespace SkillsPortal.API.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            logger.LogInformation("Handling request {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await next(context);

            logger.LogInformation("Finished handling request. Status: {StatusCode}",
                context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Path}", context.Request.Path);
            throw; // let your exception handler handle it
        }
    }
}