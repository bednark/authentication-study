using AuthenticationStudy.Models;
using AuthenticationStudy.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationStudy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
  private readonly AuthService _authService;

  public AuthController(AuthService authService) {
    _authService = authService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(UserForm dto) {
    var success = await _authService.Register(dto.Username, dto.Password);

    if (!success)
      return Conflict("Username already exists.");

    return Ok("Registration successful.");
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(UserForm dto) {
    var token = await _authService.Login(dto.Username, dto.Password);

    if (token is null)
      return Unauthorized("Invalid credentials.");

    Response.Cookies.Append("Authorization", token, new CookieOptions {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTime.UtcNow.AddHours(2)
    });

    return Ok("Login successful.");
  }

  [HttpPost("logout")]
  public IActionResult Logout() {
    Response.Cookies.Delete("Authorization");
    return Ok("Logged out.");
  }
}
