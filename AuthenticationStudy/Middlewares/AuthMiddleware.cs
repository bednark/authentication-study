using AuthenticationStudy.Middlewares.AuthStrategies;
using AuthenticationStudy.Services;

namespace AuthenticationStudy.Middlewares;

public class AuthMiddleware(RequestDelegate next, IConfiguration config, JwksKeyProvider keyProvider)
{
  private readonly RequestDelegate _next = next;
  private readonly JwksKeyProvider _keyProvider = keyProvider;
  private readonly string _authMethod = config["Auth:Method"] ?? "None";
  private readonly string _secretJWT = config["Auth:JWT:Key"] ?? string.Empty;
  private readonly string _keycloakBaseUrl = config["Auth:OAuth2:Keycloak:BaseUrl"] ?? string.Empty;
  private readonly string _keycloakRealm = config["Auth:OAuth2:Keycloak:Realm"] ?? string.Empty;
  private readonly string _keycloakClientId = config["Auth:OAuth2:Keycloak:ClientId"] ?? string.Empty;

  private static bool IsStaticFile(string path) {
    return path.EndsWith(".js") ||
           path.EndsWith(".css") ||
           path.EndsWith(".ico") ||
           path.EndsWith(".png") ||
           path.EndsWith(".jpg") ||
           path.EndsWith(".jpeg") ||
           path.EndsWith(".svg") ||
           path.EndsWith(".woff") ||
           path.EndsWith(".woff2") ||
           path.StartsWith("/assets/");
  }

  public async Task InvokeAsync(HttpContext context)
  {
    // If the authentication method is OAuth2 or mTLS and the request path is "/login",
    // redirect to "/klienci" to prevent access to the login page directly.
    if ((_authMethod is "OAuth2" or "mTLS") && context.Request.Path == "/login")
    {
      context.Response.Redirect("/klienci");
      return;
    }

    // Define paths that do not require authentication.
    var pathExcluded = new HashSet<string>
    {
      "/api/auth/login",
      "/api/auth/register",
      "/api/auth/logout"
    };

    // Get the requested path from the context.
    var requestedPath = context.Request.Path.Value ?? string.Empty;

    // Check if the request's Accept header indicates HTML.
    var acceptHeaders = context.Request.Headers.Accept.ToString() ?? string.Empty;
    var isHtmlRequest = acceptHeaders.Contains("text/html");

    // If the requested path is a static file or excluded from authentication
    // or if the authentication method is set to "None", skip authentication.
    if (IsStaticFile(requestedPath) || pathExcluded.Contains(requestedPath)
      || _authMethod == "None")
    {
      await _next(context);
      return;
    }

    switch (_authMethod)
    {
      case "JWT":
        var isAuthenticatedJwt = await JwtAuthenticator.TryAuthenticate(context, new SymmetricKeyProvider(_secretJWT));
        if (!isAuthenticatedJwt)
        {
          if (requestedPath.Equals("/login"))
          {
            await _next(context);
            return;
          }

          // If the request is not authenticated and the Accept header indicates HTML,
          // redirect to the login page else return 401 Unauthorized.
          if (isHtmlRequest)
          {
            context.Response.Redirect("/login");
            return;
          }
          else
          {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
          }
          return;
        }
        else
        {
          // If the request is authenticated and the requested path is "/login" or "/",
          // redirect to "/klienci" to prevent access to the login page directly.
          if (requestedPath.Equals("/login") || requestedPath.Equals("/"))
          {
            context.Response.Redirect("/klienci");
            return;
          }
        }

        await _next(context);
        break;

      case "OAuth2":
        var isAuthenticatedOAuth2 = await JwtAuthenticator.TryAuthenticate(context,
          new JwksSigningKeyProvider(_keyProvider, _keycloakBaseUrl, _keycloakRealm));

        if (!isAuthenticatedOAuth2)
        {
          // If the request is not authenticated and the Accept header indicates HTML,
          // redirect to the login page else return 401 Unauthorized.
          if (isHtmlRequest)
          {
            var loginUrl = $"{_keycloakBaseUrl}/auth/realms/{_keycloakRealm}" +
              $"/protocol/openid-connect/auth?client_id={_keycloakClientId}" +
              $"&response_type=code&scope=openid&redirect_uri=" +
              Uri.EscapeDataString(context.Request.Scheme + "://" + context.Request.Host + "/api/auth/callback");

            context.Response.Redirect(loginUrl);
          }
          else
          {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
          }
          return;
        }
        break;

      case "mTLS":
        await _next(context);
        break;

      default:
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Authentication method not supported.");
        return;
    }
  }
}
