using Microsoft.EntityFrameworkCore;
using AuthenticationStudy.Models;

namespace AuthenticationStudy.Services;

public class JwtAuthService(AppDbContext context, IConfiguration config)
{
  private readonly AppDbContext _context = context;
  private readonly string _jwtKey = config["Auth:JWT:Key"]!;

  public async Task<bool> Register(string username, string password) {
    if (await _context.Users.AnyAsync(u => u.Username == username))
      return false;

    PasswordHelper.CreatePasswordHash(password, out var hash, out var salt);

    var user = new Users {
      Username = username,
      PasswordHash = hash,
      PasswordSalt = salt
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<string?> Login(string username, string password) {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
    {
      // Log if the user was not found or password verification failed
      return null;
    }

    return JwtTokenGenerator.Generate(user, _jwtKey);
  }
}
