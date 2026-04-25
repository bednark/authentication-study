using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography.X509Certificates;

using AuthenticationStudy.Middlewares.AuthStrategies;

namespace AuthenticationStudy.Middlewares;

public class AuthMiddleware(RequestDelegate next, IConfiguration config)
{
  private readonly RequestDelegate _next = next;
  private readonly string _authMethod = config["Auth:Method"] ?? "None";
  private readonly string _secretJWT = config["Auth:JWT:Key"] ?? string.Empty;
  private readonly string _caPath = config["Auth:mTLS:CaPath"] ?? string.Empty;
  private readonly string _caKeyPath = config["Auth:mTLS:CaKeyPath"] ?? string.Empty;

  // Method is resposible for checking if the requested path is a static file.
  private static bool IsStaticFile(string path)
  {
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
    // Get the requested path from the context.
    var requestedPath = context.Request.Path.Value ?? string.Empty;

    // If the authentication method is OIDC or mTLS and the request path is "/login",
    // redirect to "/klienci" to prevent access to the login page directly.
    if ((_authMethod is "OIDC" or "mTLS" or "None") && requestedPath == "/login")
    {
      context.Response.Redirect("/klienci");
      return;
    }

    if ((_authMethod is "mTLS" or "None") && requestedPath == "/api/auth/logout")
    {
      context.Response.StatusCode = StatusCodes.Status404NotFound;
      return;
    }

    // If the authentication method is "None", skip authentication and proceed to the next middleware.
    if (_authMethod.Equals("None"))
    {
      await _next(context);
      return;
    }

    // Define a set of API paths that should not require JWT authentication.
    var jwtApiPaths = new HashSet<string>
    {
      "/api/auth/login",
      "/api/auth/register"
    };

    // If the requested path is in the set of JWT API paths and the authentication method is not JWT,
    // return a 404 Not Found response.
    if (jwtApiPaths.Contains(requestedPath) && _authMethod is "mTLS" or "OIDC" or "None")
    {
      context.Response.StatusCode = StatusCodes.Status404NotFound;
      return;
    }

    if ((_authMethod is "OIDC" or "JWT") && (requestedPath is "/api/auth/logout"))
    {
      await _next(context);
      return;
    }

    // If the requested path is a static file skip authentication.
    if (IsStaticFile(requestedPath))
    {
      await _next(context);
      return;
    }

    // Check if the request's Accept header indicates HTML.
    var acceptHeaders = context.Request.Headers.Accept.ToString() ?? string.Empty;
    var isHtmlRequest = acceptHeaders.Contains("text/html");

    switch (_authMethod)
    {
      case "JWT":
        if (jwtApiPaths.Contains(requestedPath))
        {
          await _next(context);
          return;
        }
        // Execute JWT authentication.
        var isAuthenticatedJwt = await JwtAuthenticator.TryAuthenticate(context, _secretJWT);

        if (!isAuthenticatedJwt)
        {
          // If the request is not authenticated and the requested path is "/login",
          // allow the request to proceed to the next middleware.
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

      case "OIDC":
        // If the request is not authenticated execute OpenID Connect challenge.
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
          await context.ChallengeAsync("oidc", new AuthenticationProperties
          {
            RedirectUri = "/klienci"
          });
          return;
        }

        await _next(context);
        break;

      case "mTLS":
        // Get the client certificate from the connection.
        var clientCert = context.Connection.ClientCertificate;

        // If the client certificate is null, return 401 Unauthorized.
        if (clientCert == null)
        {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          return;
        }

        // Get the current time in the Poland time zone.
        var polandZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandZone);

        // Check if the client certificate is valid for the current time.
        if (localTime < clientCert.NotBefore || localTime > clientCert.NotAfter)
        {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          return;
        }

        using (var chain = new X509Chain())
        {
          chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
          chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
          chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
          chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

          var caCert = X509Certificate2.CreateFromPemFile(_caPath, _caKeyPath);
          chain.ChainPolicy.ExtraStore.Add(caCert);

          bool isValid = chain.Build(clientCert);

          if (!isValid)
          {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
          }
        }

        await _next(context);
        break;

      default:
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }
  }
}
