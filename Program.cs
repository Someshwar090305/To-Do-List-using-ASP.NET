using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- SERVICES ---
builder.Services.AddControllers();
builder.Services.AddDbContext<TodoDb>(opt => opt.UseSqlite("Data Source=todo.db"));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt => {
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, Description = "Enter token", Name = "Authorization", Type = SecuritySchemeType.Http, BearerFormat = "JWT", Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] {} }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false, ValidateAudience = false
        };
    });

// 1. Add CORS Service
builder.Services.AddCors(); 

var app = builder.Build();

// --- PIPELINE ---
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. ENABLE CORS (Must be before Auth) <-- THIS WAS MISSING
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// --- MODELS ---
public class User { public int Id { get; set; } public string Username { get; set; } = ""; public string PasswordHash { get; set; } = ""; }
public class Todo { public int Id { get; set; } public string? Name { get; set; } public bool IsComplete { get; set; } public int UserId { get; set; } }
public class TodoDb : DbContext {
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<User> Users => Set<User>();
}