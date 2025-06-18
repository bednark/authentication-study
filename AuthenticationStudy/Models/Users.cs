namespace AuthenticationStudy.Models;

public class Users {
  public int Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
  public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
}
