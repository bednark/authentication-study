namespace AuthenticationStudy.Middlewares;

public class AuthMiddleware {
  private readonly RequestDelegate _next;
  private readonly string _authMethod;

  public AuthMiddleware(RequestDelegate next, IConfiguration config) {
    _next = next;
    _authMethod = config["Auth:Method"] ?? "None";
  }

  public async Task InvokeAsync(HttpContext context) {
    var pathExluded = new List<string> {
      "/api/auth",
      "/login"
    };
    string requestedPath = context.Request.Path.Value ?? string.Empty;

    if (pathExluded.Contains(requestedPath) || _authMethod == "None") {
      await _next(context);
      return;
    }
    else if (_authMethod == "JWT") {
      // Handle JWT authentication
    }
    else if (_authMethod == "OAuth2") {
      // Handle OAuth2 authentication
    }
    else if (_authMethod == "mTLS") {
      // Handle mTLS authorization
    }
    else {
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      await context.Response.WriteAsync("Authentication method not supported.");
      return;
    }

    await _next(context);
    return;
  }
}