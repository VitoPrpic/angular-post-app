using BlogApi.Application.Security;
using BlogApi.Data.Persistence;
using BlogApi.Data.Repositories;
using BlogApi.Data.Security;
using BlogApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApi.Data.DependencyInjection;

public static class DataServiceCollectionExtensions
{
  public static IServiceCollection AddDataLayer(
      this IServiceCollection services,
      string connectionString)
  {
    services.AddDbContext<BlogDbContext>(options => options.UseSqlite(connectionString));
    services.AddScoped<IAdminUserRepository, AdminUserRepository>();
    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<IPostRepository, PostRepository>();
    services.AddScoped<IPasswordHashingService, PasswordHashingService>();
    return services;
  }
}
