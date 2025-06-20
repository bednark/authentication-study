using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace AuthenticationStudy.Services;

public class JwksSigningKeyProvider(JwksKeyProvider jwks, string baseUrl, string realm) : ISigningKeyProvider
{
  private readonly JwksKeyProvider _jwks = jwks;
  private readonly string _keycloakBaseUrl = baseUrl;
  private readonly string _keycloakRealm = realm;

  public async Task<SecurityKey?> GetKeyAsync(string token)
  {
    var handler = new JwtSecurityTokenHandler();
    var jwt = handler.ReadJwtToken(token);
    var kid = jwt.Header.Kid;
    if (string.IsNullOrEmpty(kid)) return null;
    return await _jwks.GetSigningKeyAsync(kid, _keycloakBaseUrl, _keycloakRealm);
  }
}
