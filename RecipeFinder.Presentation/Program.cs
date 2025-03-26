using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using RecipeFinder.Infrastructure.Database;
using RecipeFinder.BusinessLogic.Interfaces;
using RecipeFinder.BusinessLogic.Services;
using RecipeFinder.DataAccess.Interfaces;
using RecipeFinder.DataAccess.Repositories;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(); // ����������� � �������

        // ������� ����������� � ������������� (MVC)
        builder.Services.AddControllersWithViews();

        // �������������� Neo4j
        builder.Services.AddSingleton(sp =>
        {
            var config = builder.Configuration.GetSection("Neo4j").Get<Neo4jSettings>();
            return GraphDatabase.Driver(config.Uri, AuthTokens.Basic(config.User, config.Password));
        });

        // ��������� ����������� � ������
        builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
        builder.Services.AddScoped<IRecipeService, RecipeService>();

        var app = builder.Build();

        // ��������� Middleware
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        // ��������
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}"
        );

        app.Run();
    }
}
