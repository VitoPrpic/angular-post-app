using BlogApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApi.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
  {
    services.AddScoped<IAdminUserService, AdminUserService>();
    services.AddScoped<ICategoryService, CategoryService>();
    services.AddScoped<IPostService, PostService>();
    return services;
  }
}
