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
      "/api/auth",
      "/login"
    };

    var requestedPath = context.Request.Path.Value ?? string.Empty;

    if (pathExcluded.Contains(requestedPath) || _authMethod == "None") {
      await _next(context);
      return;
    }

    switch (_authMethod) {
      case "JWT":
        var isAuthorized = await JwtAuthHandler.TryAuthenticate(context, _secretJWT, _next);
        if (!isAuthorized) {
          if (context.Request.Headers["Accept"].Any(h => h.Contains("text/html"))) {
            context.Response.Redirect("/login");
          } else {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
          }
          return;
        }
        break;

      case "OAuth2":
        // TODO: handle OAuth2 authentication
        await _next(context);
        break;

      case "mTLS":
        // TODO: handle mutual TLS authentication
        await _next(context);
        break;

      default:
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Authentication method not supported.");
        return;
    }
  }
}
