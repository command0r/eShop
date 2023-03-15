using System.Reflection;
using System.Text.Json;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
        {
            // Unify base patch for seed files
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!context.ProductBrands.Any())
            {
                var brandsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                context.ProductBrands.AddRange(brands);
            }
            if (!context.ProductTypes.Any())
            {
                var typesData = await File.ReadAllTextAsync(path + @"/Data/SeedData/types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                context.ProductTypes.AddRange(types);
            }
            if (!context.Products.Any())
            {
                var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                context.Products.AddRange(products);
            }
            if (!context.DeliveryMethods.Any())
            {
                var dmData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);
                context.DeliveryMethods.AddRange(deliveryMethods);
            }
            if (context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();
        }
    }
}