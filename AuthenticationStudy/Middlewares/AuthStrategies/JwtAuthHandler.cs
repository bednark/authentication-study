using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationStudy.AuthStrategies;

public static class JwtAuthHandler {
  public static async Task<bool> TryAuthenticate(HttpContext context, string jwtSecret, RequestDelegate next) {
    try {
      var token = context.Request.Cookies["Authorization"].ToString() ?? string.Empty;

      if (string.IsNullOrEmpty(token)) {
        throw new SecurityTokenException("Missing token");
      }

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
          !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
        throw new SecurityTokenException("Invalid algorithm");
      }

      context.User = principal;
      await next(context);
      return true;
    }
    catch {
      return false;
    }
  }
}
