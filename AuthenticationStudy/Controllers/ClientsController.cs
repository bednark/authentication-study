using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AuthenticationStudy.Models;

namespace AuthenticationStudy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase {
  private readonly AppDbContext _context;

  public ClientsController(AppDbContext context) {
    _context = context;
  }

  [HttpGet]
  public async Task<IActionResult> GetClients() {
    var clients = await _context.Clients.OrderBy(c => c.Name).ToListAsync();
    return Ok(clients);
  }

  [HttpPost]
  public async Task<IActionResult> CreateClient([FromBody] Clients client) {
    if (client == null) {
      return BadRequest(new { Message = "Client data is required" });
    }

    _context.Clients.Add(client);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetClients), new { id = client.Id }, client);
  }
}