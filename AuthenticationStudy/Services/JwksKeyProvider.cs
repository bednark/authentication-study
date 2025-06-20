using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AuthenticationStudy.Services;

public class JwksKeyProvider
{
  private readonly HttpClient _httpClient;
  private readonly Dictionary<string, SecurityKey> _keys = [];

  public JwksKeyProvider(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<SecurityKey?> GetSigningKeyAsync(string kid, string keycloakBaseUrl, string keycloakRealm)
  {
    if (_keys.TryGetValue(kid, out SecurityKey? value))
      return value;

    var keycloakUrl = $"{keycloakBaseUrl}/auth/realms/{keycloakRealm}/protocol/openid-connect/certs";

    // Fetch the JWKS from Keycloak
    var jwks = await _httpClient.GetFromJsonAsync<JsonWebKeySet>(keycloakUrl);

    // Check if the JWKS is null or has no keys
    if (jwks == null || jwks.Keys == null || !jwks.Keys.Any())
      return null;

    // Find the key with the matching kid
    foreach (var key in jwks.Keys)
    {
      var rsaKey = new RsaSecurityKey(new RSAParameters
      {
        Modulus = Base64UrlEncoder.DecodeBytes(key.N),
        Exponent = Base64UrlEncoder.DecodeBytes(key.E)
      })
      {
        KeyId = key.Kid
      };

      _keys[key.Kid] = rsaKey;
    }

    // Return the key if found
    return _keys.TryGetValue(kid, out var foundKey) ? foundKey : null;
  }
}
