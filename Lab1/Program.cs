using System.IdentityModel.Tokens.Jwt;
using Lab1.Auth;
using Lab1.Data;
using Microsoft.EntityFrameworkCore;


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices().AddJwtAuthentication(builder.Configuration).AddAuthorization();
// Add CORS services
builder.Services.AddCors();

var app = builder.Build();
// Configure CORS
app.UseCors(builder => 
{
    builder.WithOrigins("http://localhost:3000") // Replace with your React app URL
           .AllowAnyHeader()
           .AllowAnyMethod();
});

app.ConfigureApiEndpoints();

app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
dbContext.Database.Migrate();

var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();

app.Run();

