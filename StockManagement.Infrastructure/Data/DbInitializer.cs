using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, ILogger<ApplicationDbContext> logger)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Apply any pending migrations
                await context.Database.MigrateAsync();

                // Seed data only if the Products table is empty
                if (!await context.Products.AnyAsync())
                {
                    logger.LogInformation("Seeding the database with initial product data.");
                    await SeedProductsAsync(context);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        private static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            var products = new List<Product>
            {
                new Product { Name = "Laptop", StockQuantity = 50, Price = 999.99m },
                new Product { Name = "Smartphone", StockQuantity = 100, Price = 499.99m },
                new Product { Name = "Headphones", StockQuantity = 200, Price = 99.99m },
                new Product { Name = "Monitor", StockQuantity = 30, Price = 299.99m },
                new Product { Name = "Keyboard", StockQuantity = 75, Price = 59.99m },
                new Product { Name = "Mouse", StockQuantity = 80, Price = 29.99m },
                new Product { Name = "Printer", StockQuantity = 25, Price = 199.99m },
                new Product { Name = "External Hard Drive", StockQuantity = 40, Price = 129.99m },
                new Product { Name = "USB Flash Drive", StockQuantity = 150, Price = 19.99m },
                new Product { Name = "Webcam", StockQuantity = 60, Price = 79.99m }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}