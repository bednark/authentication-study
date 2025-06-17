using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AuthenticationStudy.Models;

namespace AuthenticationStudy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase {
  private readonly AppDbContext _context;

  public ClientController(AppDbContext context) {
    _context = context;
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetClient([FromRoute] int id) {
    var client = await _context.Clients.FindAsync(id);

    if (client == null) {
      return NotFound(new { Message = "Client not found" });
    }

    return Ok(client);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateClient([FromRoute] int id, [FromBody] Clients client) {
    if (client == null || client.Id != id) {
      return BadRequest(new { Message = "Invalid client data" });
    }

    var existingClient = await _context.Clients.FindAsync(id);
    if (existingClient == null) {
      return NotFound(new { Message = "Client not found" });
    }

    existingClient.Name = client.Name;
    existingClient.Email = client.Email;
    existingClient.Phone = client.Phone;
    existingClient.Address = client.Address;
    existingClient.City = client.City;
    existingClient.PostalCode = client.PostalCode;
    existingClient.Country = client.Country;

    _context.Clients.Update(existingClient);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteClient([FromRoute] int id) {
    var client = await _context.Clients.FindAsync(id);
    if (client == null) {
      return NotFound(new { Message = "Client not found" });
    }

    _context.Clients.Remove(client);
    await _context.SaveChangesAsync();

    return NoContent();
  }
}