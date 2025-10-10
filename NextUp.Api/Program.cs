using Microsoft.EntityFrameworkCore;
using NextUp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure EF Core with Npgsql (PostgreSQL)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Prefer a user-secret key named 'ConnectionStrings:NextUpDbConnectionString',
// then fall back to the usual 'DefaultConnection' key and finally to a local default.
var connectionString = builder.Configuration.GetConnectionString("NextUpDbConnectionString")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Host=localhost;Database=nextup;Username=postgres;Password=postgres";

builder.Services.AddDbContext<NextUpDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// Minimal EF demo endpoints
app.MapGet("/users", async (NextUpDbContext db) =>
{
    return await db.Users.ToListAsync();
});

app.MapPost("/users", async (NextUpDbContext db, NextUp.Api.Data.User user) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
