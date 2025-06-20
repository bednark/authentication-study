using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthenticationStudy.Services;

public class SymmetricKeyProvider(string secret) : object(), ISigningKeyProvider
{
  private readonly string _secret = secret;

  public Task<SecurityKey?> GetKeyAsync(string token)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
    return Task.FromResult<SecurityKey?>(key);
  }
}
