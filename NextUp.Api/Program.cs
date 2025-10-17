using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NextUp.Api.DTOs;
using NextUp.Api.Endpoints;
using NextUp.Api.Services;
using NextUp.Data;
using NextUp.Models;
using System.Security.Claims;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NextUpDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NextUpDbConnectionString")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CoachOnly", policy => policy.RequireClaim(ClaimTypes.Role, User.COACH_ROLE));
    options.AddPolicy("PlayerOnly", policy => policy.RequireClaim(ClaimTypes.Role, User.PLAYER_ROLE));
    options.AddPolicy("AthleticDirectorOnly", policy => policy.RequireClaim(ClaimTypes.Role, User.ATHLETIC_DIRECTOR_ROLE));
});
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NextUpDbContext>();
    var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    await DataSeeder.SeedAsync(context, passwordService);
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "NextUp API" }));
app.MapPost("/auth/login", async (LoginRequest request, NextUpDbContext db, IPasswordService passwordService, HttpContext httpContext) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
    if (user == null || !passwordService.VerifyPassword(request.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        new(ClaimTypes.Role, user.Role)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    return Results.Ok(new { Message = "Logged in successfully", User = new { user.Email, user.FirstName, user.LastName } });
});
app.MapPost("/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok(new { Message = "Logged out successfully" });
});
app.MapGet("/auth/me", (ClaimsPrincipal user) =>
{
    if (!user.Identity?.IsAuthenticated ?? true)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(new
    {
        IsAuthenticated = true,
        UserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
        Name = user.Identity?.Name ?? "",
        Email = user.FindFirst(ClaimTypes.Email)?.Value,
        Role = user.FindFirst(ClaimTypes.Role)?.Value
    });
}).RequireAuthorization();
app.MapTeamEndpoints();
app.MapPlayerEndpoints();
app.MapAuthEndpoints();
app.MapCoachEndpoints();
app.MapAthleticDirectorEndpoints();
app.MapGameEndpoints();
app.MapStatsEndpoints();
app.MapPlayerNotesEndpoints();
app.MapPlayerGoalsEndpoints();
app.MapGameNotesEndpoints();
app.MapUserEndpoints();
app.Run();
