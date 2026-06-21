using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Lensrock.API.Endpoints;
using Lensrock.Core;
using Lensrock.API.Middleware;
using Lensrock.API.Security;
using Lensrock.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La chaîne de connexion est absente.");
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("La clé JWT est absente.");

builder.Services.AddCors(options => options.AddPolicy("Angular", policy => policy
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCoreServices();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddSingleton<JwtTokenService>();

var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthEndpoints();
app.MapServiceEndpoints();
app.MapCartEndpoints();
app.MapOrderEndpoints();
app.MapAdminEndpoints();
app.Run();
