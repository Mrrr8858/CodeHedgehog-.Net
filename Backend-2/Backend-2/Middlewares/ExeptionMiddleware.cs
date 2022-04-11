using Backend_2.Exeptions;


namespace Backend_2.Middlewares
{
    public static class MiddlewareExeption
    {
        public static void UseExeptionHandlingMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<ExeptionMiddleware>();
        }
    }
    public class ExeptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExeptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ObjectNotFoundExeption ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { ErrorMessage = ex.Message});
            }
            catch (ValidationExeption ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { ErrorMessage = ex.Message });
            }
            catch (ForbiddenExeption ex)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { ErrorMessage = ex.Message });
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { ErrorMessage = "Внутренняя ошибка сервера" });
            }
        }
    }
}
