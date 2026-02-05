using Carter;
using Mapster;
using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Dtos;
using Microservice.ProductWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservice.ProductWebAPI.Modules;

public sealed class ProductModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder group)
    {
        var app = group.MapGroup("products");

        app.MapGet(string.Empty, async (
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var res = await dbContext
            .Products
            .OrderBy(p => p.Name)
            .Select(s => new ProductDto
            {
                Id = s.Id,
                Name = s.Name,
                CategoryId = s.CategoryId,
            })
            .ToListAsync(cancellationToken);

            //var categoryIds = res.Select(s => s.CategoryId).ToHashSet();

            var http = httpClientFactory.CreateClient();
            var categories = await http.GetFromJsonAsync<List<CategoryDto>>("http://localhost:5003/categories");

            foreach (var product in res)
            {
                product.CategoryName = categories?.FirstOrDefault(p => p.Id == product.CategoryId)?.Name ?? "";
            }

            return res;
        });

        app.MapPost(string.Empty, async (
            ProductCreateDto request,
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            Product product = request.Adapt<Product>();
            dbContext.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return product.Id;
        });
    }
}
