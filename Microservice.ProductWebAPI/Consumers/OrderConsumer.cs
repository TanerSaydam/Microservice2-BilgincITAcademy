using MassTransit;
using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Models;
using Microservice.Shared;
using Microsoft.EntityFrameworkCore;

namespace Microservice.ProductWebAPI.Consumers;

public class OrderConsumer(
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint
    ) : IConsumer<OrderCreateQueueDto>
{
    public async Task Consume(ConsumeContext<OrderCreateQueueDto> context)
    {
        ProductResultQueueDto result;

        Product? product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == context.Message.ProductId, default);
        if (product is not null)
        {
            try
            {
                product.Stock -= context.Message.Quantity;
                dbContext.Update(product);
                await dbContext.SaveChangesAsync(default);
                result = new ProductResultQueueDto(context.Message.OrderId, true);
            }
            catch (Exception)
            {
                result = new ProductResultQueueDto(context.Message.OrderId, false);
            }
        }
        else
        {
            result = new ProductResultQueueDto(context.Message.OrderId, false);
        }

        await publishEndpoint.Publish(result);
    }
}