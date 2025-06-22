using Microsoft.EntityFrameworkCore;
using MyAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<EmployeeDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS (add your Render/Vercel frontend later)
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowReactApp",
        p => p.WithOrigins(
                "http://localhost:3000",
                "https://your-frontend.vercel.app")   // add Render/Vercel URL here
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────
// Enable Swagger in *both* Development & Production
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";   // so URL is /swagger
    });
}

// UseHttpsRedirection is optional on Render.
// Comment out if it causes redirect loops.
// app.UseHttpsRedirection();

app.UseCors("AllowReactApp");
app.UseAuthorization();

app.MapControllers();

app.Run();
