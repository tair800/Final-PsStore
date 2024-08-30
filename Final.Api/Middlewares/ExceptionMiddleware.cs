using Final.Application.Exceptions;

namespace Final.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var errors = new Dictionary<string, string>();
                httpContext.Response.StatusCode = 500;
                if (ex is CustomExceptions custom)
                {
                    message = custom.Message;
                    errors = custom.Errors;
                    httpContext.Response.StatusCode = custom.Code;

                }
                await httpContext.Response.WriteAsJsonAsync(new { message, errors });
            }
        }
    }
}
