using Microsoft.EntityFrameworkCore;
using AuthenticationStudy.Models;

public class AppDbContext : DbContext {
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

  public DbSet<Clients> Clients { get; set; }
  public DbSet<Users> Users { get; set; }

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

    modelBuilder.Entity<Users>(entity => {
      entity.ToTable("users");

      entity.Property(u => u.Id).HasColumnName("id");
      entity.Property(u => u.Username).HasColumnName("username")
        .IsRequired().HasMaxLength(50);
      entity.Property(u => u.FirstName).HasColumnName("first_name")
        .IsRequired().HasMaxLength(50);
      entity.Property(u => u.LastName).HasColumnName("last_name")
        .IsRequired().HasMaxLength(50);
      entity.Property(u => u.PasswordHash).HasColumnName("password_hash")
        .IsRequired();
      entity.Property(u => u.PasswordSalt).HasColumnName("password_salt")
        .IsRequired();
    });
    modelBuilder.Entity<Users>()
      .HasIndex(u => u.Username)
      .IsUnique();
  }
}
