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
            IHttpContextAccessor httpContextAccessor,
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
                Quantity = s.Quantity,
            })
            .ToListAsync(default);

            var categoryUri = "http://localhost:5003/categories";
            var http = httpClientFactory.CreateClient();
            //var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString() ?? "";
            //http.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);
            var categories = await http.GetFromJsonAsync<List<CategoryDto>>(categoryUri, cancellationToken);
            foreach (var product in res)
            {
                product.CategoryName = categories?.FirstOrDefault(p => p.Id == product.CategoryId)?.Name ?? "";
            }

            return Results.Ok(res);
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