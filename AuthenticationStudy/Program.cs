using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

using AuthenticationStudy.Services;
using AuthenticationStudy.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>();

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.Migrate();
}

app.UseMiddleware<AuthMiddleware>();

var frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "frontend", "dist");

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions {
  FileProvider = new PhysicalFileProvider(frontendPath),
  RequestPath = ""
});

app.MapControllers();
app.MapFallbackToFile("index.html", new StaticFileOptions {
  FileProvider = new PhysicalFileProvider(frontendPath)
});

app.Run();
