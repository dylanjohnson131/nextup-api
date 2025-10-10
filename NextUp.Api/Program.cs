using Microsoft.EntityFrameworkCore;
using NextUp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Read connection string: prefer user-secrets key, then appsettings, then a safe default for dev
var connectionString = builder.Configuration.GetConnectionString("NextUpDbConnectionString")
					   ?? builder.Configuration.GetConnectionString("DefaultConnection")
					   ?? "Host=localhost;Database=NextUp;Username=postgres;Password=postgres";

// Add services
builder.Services.AddDbContext<NextUpDbContext>(options =>
{
	options.UseNpgsql(connectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Redirect root to Swagger UI for convenience
app.MapGet("/", () => Results.Redirect("/swagger"));

// Minimal API endpoints for Users
app.MapGet("/users", async (NextUpDbContext db) =>
{
	var users = await db.Users.ToListAsync();
	return Results.Ok(users);
});

app.MapPost("/users", async (NextUpDbContext db, User user) =>
{
	db.Users.Add(user);
	await db.SaveChangesAsync();
	return Results.Created($"/users/{user.Id}", user);
});

app.Run();
