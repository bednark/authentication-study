using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationStudy.AuthStrategies;

public static class JwtAuthHandler {
  public static Task<bool> TryAuthenticate(HttpContext context, string jwtSecret) {
    try {
      var token = context.Request.Cookies["Authorization"];
      if (string.IsNullOrEmpty(token)) return Task.FromResult(false);

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.UTF8.GetBytes(jwtSecret);

      var validationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
      };

      var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

      if (validatedToken is not JwtSecurityToken jwtToken ||
          !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        return Task.FromResult(false);

      context.User = principal;
      return Task.FromResult(true);
    } catch {
      return Task.FromResult(false);
    }
  }

  public static async Task<bool> TryAuthenticate(HttpContext context, string jwtSecret, RequestDelegate next) {
    if (!await TryAuthenticate(context, jwtSecret)) return false;

    await next(context);
    return true;
  }
}
