using BlogApi.Application.DependencyInjection;
using BlogApi.Data.DependencyInjection;
using BlogApi.Data.Persistence;
using BlogApi.Middleware;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var railwayPort = Environment.GetEnvironmentVariable("PORT");
if (int.TryParse(railwayPort, out var port))
{
  builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var railwayVolumePath = Environment.GetEnvironmentVariable("RAILWAY_VOLUME_MOUNT_PATH");
var defaultDatabasePath = Path.GetFullPath(
    string.IsNullOrWhiteSpace(railwayVolumePath)
        ? Path.Combine(builder.Environment.ContentRootPath, "..", "..", "database", "blog.db")
        : Path.Combine(railwayVolumePath, "blog.db")
);
var connectionString = builder.Configuration.GetConnectionString("Blog");

if (string.IsNullOrWhiteSpace(connectionString))
{
  Directory.CreateDirectory(Path.GetDirectoryName(defaultDatabasePath)!);
  connectionString = $"Data Source={defaultDatabasePath};Foreign Keys=True";
}

builder.Services.AddApplicationLayer();
builder.Services.AddDataLayer(connectionString);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
  {
    Description = "Upiši API ključ. Vrijednost se šalje u Authorization zaglavlju.",
    In = ParameterLocation.Header,
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey
  });
  options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
  {
    [new OpenApiSecuritySchemeReference("ApiKey", document, null)] = []
  });
});
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

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API v1");
    options.DocumentTitle = "Blog API";
  });
}

app.UseCors("AngularApp");
app.UseMiddleware<ApiExceptionMiddleware>();

var indexFile = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "index.html");
if (File.Exists(indexFile))
{
  app.UseDefaultFiles();
  app.UseStaticFiles();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).ExcludeFromDescription();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
  app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

if (File.Exists(indexFile))
{
  app.MapFallbackToFile("index.html");
}

app.Run();
