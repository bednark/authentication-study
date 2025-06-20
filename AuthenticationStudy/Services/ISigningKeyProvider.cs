using Microsoft.IdentityModel.Tokens;

namespace AuthenticationStudy.Services;

// ISigningKeyProvider is an interface that defines a method to retrieve a signing key based on a token.
public interface ISigningKeyProvider
{
  Task<SecurityKey?> GetKeyAsync(string token);
}
