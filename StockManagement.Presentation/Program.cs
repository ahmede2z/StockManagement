using Microsoft.EntityFrameworkCore;
using StockManagement.Infrastructure;
using StockManagement.Infrastructure.Data;
using StockManagement.Services;
namespace StockManagement.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region // Add services to the container.


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure(builder.Configuration);
            #endregion


            var app = builder.Build();

            // Create and migrate the database, then seed initial data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

                    // Use the DbInitializer to handle migrations and seeding
                    await DbInitializer.InitializeAsync(services, logger);

                    logger.LogInformation("Database initialization completed successfully.");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}
