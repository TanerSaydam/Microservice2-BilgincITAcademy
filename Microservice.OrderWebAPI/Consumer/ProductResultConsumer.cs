using MassTransit;
using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Models;
using Microservice.Shared;
using Microsoft.EntityFrameworkCore;

namespace Microservice.OrderWebAPI.Consumer;

public class ProductResultConsumer(
    ApplicationDbContext dbContext
    ) : IConsumer<ProductResultQueueDto>
{
    public async Task Consume(ConsumeContext<ProductResultQueueDto> context)
    {
        Order? order = await dbContext.Orders.FirstOrDefaultAsync(p => p.Id == context.Message.OrderId, default);
        if (order is not null)
        {
            order.Status = context.Message.Result ? Status.Completed : Status.Fail;
            dbContext.Update(order);
            await dbContext.SaveChangesAsync(default);
        }
        else
        {
            throw new ArgumentException("Something went wrong");
        }
    }
}
