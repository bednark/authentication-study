using System.Text;
using AuthenticationStudy.AuthStrategies;
using Microsoft.AspNetCore.Http;

namespace AuthenticationStudy.Middlewares;

public class AuthMiddleware {
  private readonly RequestDelegate _next;
  private readonly string _authMethod;
  private readonly string _secretJWT;

  public AuthMiddleware(RequestDelegate next, IConfiguration config) {
    _next = next;
    _authMethod = config["Auth:Method"] ?? "None";
    _secretJWT = config["Auth:JWT:Key"] ?? string.Empty;
  }

  public async Task InvokeAsync(HttpContext context) {
    var pathExcluded = new List<string> {
      "/api/auth/login",
      "/api/auth/register",
      "/api/auth/logout",
      "/login"
    };

    var requestedPath = context.Request.Path.Value ?? string.Empty;

    var isStaticFile = requestedPath.EndsWith(".js") ||
      requestedPath.EndsWith(".css") ||
      requestedPath.EndsWith(".ico") ||
      requestedPath.EndsWith(".png") ||
      requestedPath.EndsWith(".jpg") ||
      requestedPath.EndsWith(".jpeg") ||
      requestedPath.EndsWith(".svg") ||
      requestedPath.EndsWith(".woff") ||
      requestedPath.EndsWith(".woff2") ||
      requestedPath.StartsWith("/assets/");

    if ((requestedPath.Equals("/login") || requestedPath.Equals("/")) && _authMethod == "JWT") {
      var isAuthorized = await JwtAuthHandler.TryAuthenticate(context, _secretJWT);
      if (isAuthorized) {
        context.Response.Redirect("/klienci");
        return;
      }
    }

    if (isStaticFile || pathExcluded.Contains(requestedPath) || _authMethod == "None") {
      await _next(context);
      return;
    }

    switch (_authMethod) {
      case "JWT":
        var isAuthenticated = await JwtAuthHandler.TryAuthenticate(context, _secretJWT, _next);
        if (!isAuthenticated) {
          var acceptHeaders = context.Request.Headers["Accept"].ToString() ?? string.Empty;
          if (acceptHeaders.Contains("text/html")) {
            context.Response.Redirect("/login");
          } else {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
          }
          return;
        }
        break;

      case "OAuth2":
        await _next(context);
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
