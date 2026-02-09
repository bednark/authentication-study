using AuthenticationStudy.Models;
using AuthenticationStudy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace AuthenticationStudy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(JwtAuthService authService, IConfiguration config) : ControllerBase {
  private readonly JwtAuthService _authService = authService;
  private readonly string _authMethod = config["Auth:Method"] ?? "None";

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
    {
      // Log the failed login attempt
      return Unauthorized("Invalid credentials.");
    }

    Response.Cookies.Append("Authorization", token, new CookieOptions {
      HttpOnly = true,
      Secure = true,
      SameSite = SameSiteMode.Strict,
      Expires = DateTime.UtcNow.AddHours(2)
    });

    return Ok("Login successful.");
  }

  [HttpGet("logout")]
  public IActionResult Logout() {
    var redirectBaseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/";

    switch (_authMethod)
    {
      case "JWT":
        Response.Cookies.Delete("Authorization");
        return Redirect(redirectBaseUrl);

      case "OAuth2":
        return SignOut(new AuthenticationProperties
        {
          RedirectUri = redirectBaseUrl,
        },
        CookieAuthenticationDefaults.AuthenticationScheme,
        "oidc");

      default:
        return BadRequest("Unsupported authentication method.");
    }
  }
}
