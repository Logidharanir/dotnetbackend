using Microsoft.EntityFrameworkCore;
using MyAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔄 DbContext → PostgreSQL (Supabase)
builder.Services.AddDbContext<EmployeeDbContext>(opts =>
    opts.UseNpgsql(       // ← switched from UseSqlServer to UseNpgsql
        builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS (add your frontend URL later)
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowReactApp",
        p => p.WithOrigins(
                "http://localhost:3000",
                "https://your-frontend.vercel.app")   // add your real URL
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";   // URL = /swagger
    });
}

// app.UseHttpsRedirection();   // keep commented on Render if it loops
app.UseCors("AllowReactApp");
app.UseAuthorization();

app.MapControllers();
app.Run();
