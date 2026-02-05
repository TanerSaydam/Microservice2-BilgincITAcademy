using Carter;
using Mapster;
using Microservice.CategoryWebAPI.Context;
using Microservice.CategoryWebAPI.Dtos;
using Microservice.CategoryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservice.CategoryWebAPI.Modules;

public sealed class CategoryModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder group)
    {
        //var versionSet = group.NewApiVersionSet()
        //    .HasApiVersion(new ApiVersion(1))
        //    .HasApiVersion(new ApiVersion(2))
        //    .ReportApiVersions()
        //    .Build();

        //var app = group.MapGroup("categories/v{version:apiVersion}").WithApiVersionSet(versionSet);        
        var app = group.MapGroup("categories");

        app.MapGet(string.Empty, async (
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken
            ) =>
        {
            var res = await dbContext.Categories.OrderBy(p => p.Name).ToListAsync(cancellationToken);
            return res;
        });//.HasApiVersion(1);

        app.MapPost(string.Empty, async (
            CategoryCreateDto request,
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken
            ) =>
        {
            Category category = request.Adapt<Category>();
            dbContext.Add(category);
            await dbContext.SaveChangesAsync(cancellationToken);

            return category.Id;
        });//.HasApiVersion(1);
    }
}
