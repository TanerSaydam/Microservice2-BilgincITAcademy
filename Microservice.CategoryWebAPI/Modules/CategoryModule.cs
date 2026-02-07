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
        var app = group.MapGroup("categories").WithTags("Categories");//.RequireAuthorization();

        app.MapGet(string.Empty, async (
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken
            ) =>
        {
            var res = await dbContext.Categories.OrderBy(p => p.Name).ToListAsync(cancellationToken);
            return res;
        });

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
        }).RequireRateLimiting("fixed"); //.RequireMyAuthorize().RequireAuthorization();
    }
}

static class Extensions
{
    public static RouteHandlerBuilder RequireMyAuthorize(this RouteHandlerBuilder route)
    {
        return route;
    }
}