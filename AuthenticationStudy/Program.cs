using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using AuthenticationStudy.Services;
using AuthenticationStudy.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddScoped<JwtAuthService>();

if (config["Auth:Method"] == "OAuth2") {
  builder.Services.AddAuthentication(options =>
  {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
  })
    .AddCookie()
    .AddOpenIdConnect("oidc", options =>
    { 
      options.Authority = $"{config["Auth:OAuth2:Keycloak:BaseUrl"]}/realms/{config["Auth:OAuth2:Keycloak:Realm"]}";
      options.ClientId = config["Auth:OAuth2:Keycloak:ClientId"];
      options.ClientSecret = config["Auth:OAuth2:Keycloak:ClientSecret"];
      options.ResponseType = OpenIdConnectResponseType.Code;
      options.SaveTokens = true;
      options.GetClaimsFromUserInfoEndpoint = true;
      options.CallbackPath = "/signin-oidc";
      options.RequireHttpsMetadata = false;

      options.TokenValidationParameters.NameClaimType = "preferred_username";
      options.TokenValidationParameters.RoleClaimType = "roles";

      options.Events.OnRemoteFailure = context =>
      {
          context.Response.Redirect("/error?failure=" + Uri.EscapeDataString(context.Failure?.Message ?? ""));
          context.HandleResponse();
          return Task.CompletedTask;
      };
    }
  );
}

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();
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
