using Microsoft.EntityFrameworkCore;
using AuthenticationStudy.Models;

public class AppDbContext : DbContext {
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

  public DbSet<Clients> Clients { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.HasDefaultSchema("auth");
  
    modelBuilder.Entity<Clients>(entity => {
      entity.ToTable("clients");

      entity.Property(c => c.Id).HasColumnName("id");
      entity.Property(c => c.Name).HasColumnName("name")
        .IsRequired().HasMaxLength(100);
      entity.Property(c => c.Email).HasColumnName("email")
        .IsRequired().HasMaxLength(100);
      entity.Property(c => c.Phone).HasColumnName("phone")
        .IsRequired().HasMaxLength(15);
      entity.Property(c => c.Address).HasColumnName("address")
        .IsRequired().HasMaxLength(200);
      entity.Property(c => c.City).HasColumnName("city")
        .IsRequired().HasMaxLength(50);
      entity.Property(c => c.PostalCode).HasColumnName("postal_code")
        .IsRequired().HasMaxLength(20);
      entity.Property(c => c.Country).HasColumnName("country")
        .IsRequired().HasMaxLength(50);
    });
  }
}
