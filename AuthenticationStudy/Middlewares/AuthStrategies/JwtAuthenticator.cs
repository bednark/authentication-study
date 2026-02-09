using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthenticationStudy.Middlewares.AuthStrategies;

public static class JwtAuthenticator
{
  public static Task<bool> TryAuthenticate(HttpContext context, string secretJwt)
  {
    try
    {
      // Get the JWT token from the request cookies
      var token = context.Request.Cookies["Authorization"];

      // If the token is null or empty, return false
      if (string.IsNullOrEmpty(token)) return Task.FromResult(false);

      // Get the signing key using the key provider
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretJwt));

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
      if (validatedToken is not JwtSecurityToken jwtToken ||
          !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        return Task.FromResult(false);

      // Set the user in the context
      context.User = principal;
      return Task.FromResult(true);
    }
    catch
    {
      // If any exception occurs during validation, return false
      // Cases possible here include:
      // - Invalid token format
      // - Token expired
      // - Signature validation failed
      // - Any other validation error
      return Task.FromResult(false);
    }
  }
}
