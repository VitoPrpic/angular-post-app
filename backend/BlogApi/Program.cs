using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var defaultDatabasePath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, "..", "..", "database", "blog.db")
);
var connectionString = builder.Configuration.GetConnectionString("Blog");

if (string.IsNullOrWhiteSpace(connectionString))
{
  Directory.CreateDirectory(Path.GetDirectoryName(defaultDatabasePath)!);
  connectionString = $"Data Source={defaultDatabasePath};Foreign Keys=True";
}

builder.Services.AddDbContext<BlogDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
  options.AddPolicy("AngularApp", policy =>
      policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
          .AllowAnyHeader()
          .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var database = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
  database.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API v1");
  options.DocumentTitle = "Blog API";
});

app.UseCors("AngularApp");
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

app.Run();
