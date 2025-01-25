using System.Text;

namespace VisualSoft.Middlewares
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;
        private readonly string AUTH_HEADER = "Authorization";
        private readonly string AUTH_METHOD = "Basic ";
        private readonly string CREDENTIALS_SEPARATOR = ":";
        private readonly string BASIC_AUTH_USER = "BasicAuth:Username";
        private readonly string BASIC_AUTH_PASSWORD = "BasicAuth:Password";

        public BasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? authHeader = GetAuthHeader(context);
            if (authHeader == null)
            {
               MarekAsUnauthorized(context);
               return;
            }
            if (CheckCredentials(authHeader))
            {
                await next(context);
            }
            else
            {
                MarekAsUnauthorized(context);
            }
        }

        private string? GetAuthHeader(HttpContext context)
        {
            var authHeader = context.Request.Headers[AUTH_HEADER].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith(AUTH_METHOD))
            {
                return null;
            }
            return authHeader;
        }

        private bool CheckCredentials(string authHeader)
        {
            var token = authHeader.Substring(AUTH_METHOD.Length).Trim();
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token))
                .Split(CREDENTIALS_SEPARATOR);
            var username = credentials[0];
            var password = credentials[1];

            var configUsername = configuration[BASIC_AUTH_USER];
            var configPassword = configuration[BASIC_AUTH_PASSWORD];

            return username.Equals(configUsername, StringComparison.InvariantCulture)
                    && password.Equals(configPassword, StringComparison.InvariantCulture);
        }

        private void MarekAsUnauthorized(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
