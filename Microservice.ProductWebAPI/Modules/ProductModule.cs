using Carter;
using Mapster;
using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Dtos;
using Microservice.ProductWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Polly.Registry;
using Steeltoe.Common.Discovery;

namespace Microservice.ProductWebAPI.Modules;

public sealed class ProductModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder group)
    {
        var app = group.MapGroup("products");

        app.MapGet(string.Empty, async (
            IHttpClientFactory httpClientFactory,
            IDiscoveryClient discoveryClient,
            ResiliencePipelineProvider<string> resiliencePipelineProvider,
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
            .ToListAsync(default);

            //var pipeline = resiliencePipelineProvider.GetPipeline("http");
            var pipeline = MyExentions.HttpPipeline;

            //var categoryIds = res.Select(s => s.CategoryId).ToHashSet();

            //var services = await discoveryClient.GetInstancesAsync("CategoryWebAPI", cancellationToken);

            //var firstService = services.FirstOrDefault();
            //if (firstService is null)
            //{
            //    return Results.NotFound();
            //}

            //var categoryUri = firstService!.Uri + "categories";
            var categoryUri = "http://localhost:5003/categories";

            var http = httpClientFactory.CreateClient();

            var message = await MyExentions.HttpPipeline.ExecuteAsync(async ct => await http.GetAsync(categoryUri, cancellationToken));
            var categories = await message.Content.ReadFromJsonAsync<List<CategoryDto>>();
            //var categories = await pipeline.ExecuteAsync(async (callback, state) =>
            //{
            //    return await http.GetFromJsonAsync<List<CategoryDto>>(categoryUri, cancellationToken);
            //});
            //var categories = await http.GetFromJsonAsync<List<CategoryDto>>(categoryUri, cancellationToken);

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
