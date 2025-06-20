using AuthenticationStudy.Services;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

namespace AuthenticationStudy.Middlewares.AuthStrategies;

public static class JwtAuthenticator
{
  public static async Task<bool> TryAuthenticate(HttpContext context, ISigningKeyProvider keyProvider)
  {
    try
    {
      // Get the JWT token from the request cookies
      var token = context.Request.Cookies["Authorization"];

      // If the token is null or empty, return false
      if (string.IsNullOrEmpty(token)) return false;

      // Get the signing key using the key provider
      var key = await keyProvider.GetKeyAsync(token);
      if (key == null) return false;

      // Create a handler and validation parameters
      var handler = new JwtSecurityTokenHandler();

      // Set up the token validation parameters
      var validationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true, // Validate the signing key
        IssuerSigningKey = key, // Use the key from the provider
        ValidateIssuer = false, // Do not validate the issuer
        ValidateAudience = false, // Do not validate the audience
        ClockSkew = TimeSpan.Zero // Disable clock skew to ensure immediate expiration
      };

      // Validate the token and get the principal
      var principal = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

      // Check if the token is a JWT and if it uses HMAC with a symmetric key
      if (validatedToken is JwtSecurityToken jwt &&
          jwt.Header.Alg != null &&
          jwt.Header.Alg.StartsWith("HS") &&
          key is SymmetricSecurityKey &&
          !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
      {
        return false;
      }

      // Set the user in the context
      context.User = principal;
      return true;
    }
    catch
    {
      return false;
    }
  }
}
