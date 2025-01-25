namespace VisualSoft.Middlewares
{
    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthMiddleware>();
        }
    }
}
