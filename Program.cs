using Microsoft.EntityFrameworkCore;
using MyAPI.Models;

var builder = WebApplication.CreateBuilder(args);

/* ─────────────────────────────────────────────────────────── */
/*  Services                                                  */
/* ─────────────────────────────────────────────────────────── */
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* PostgreSQL (Supabase) */
builder.Services.AddDbContext<EmployeeDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

/* CORS: allow                   */
/*  - localhost:3000             */
/*  - your production domain(s)  */
/*  - any *.vercel.app previews  */
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowReactApp", policy =>
        policy
            .SetIsOriginAllowed(origin =>
            {
                // 1️⃣ exact matches
                var allowedExact = new[]
                {
                    "http://localhost:3000",
                    "https://full-stack-dev-basics.vercel.app",
                    "https://www.full-stack-dev-basics.vercel.app"
                };
                if (allowedExact.Contains(origin))
                    return true;

                // 2️⃣ any Vercel preview:  https://<branch>--full-stack-dev-basics.vercel.app
                if (origin.EndsWith(".vercel.app"))
                    return true;

                return false;
            })
            .AllowAnyHeader()
            .AllowAnyMethod());
});

/* ─────────────────────────────────────────────────────────── */
/*  Pipeline                                                 */
/* ─────────────────────────────────────────────────────────── */
var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";   // /swagger
    });
}

// ⚠️ HTTPS handled by Render’s proxy – leave redirection off
// app.UseHttpsRedirection();

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
