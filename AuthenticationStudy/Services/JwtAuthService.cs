using Microsoft.EntityFrameworkCore;
using AuthenticationStudy.Models;

namespace AuthenticationStudy.Services;

public class JwtAuthService {
  private readonly AppDbContext _context;
  private readonly string _jwtKey;

  public JwtAuthService(AppDbContext context, IConfiguration config) {
    _context = context;
    _jwtKey = config["Auth:JWT:Key"]!;
  }

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
      return null;

    return JwtTokenGenerator.Generate(user, _jwtKey);
  }
}
