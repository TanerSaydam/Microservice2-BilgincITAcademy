using MassTransit;
using Microservice.OrderWebAPI.Consumer;
using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Dtos;
using Microservice.OrderWebAPI.Models;
using Microservice.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.AddSqlServerDbContext<ApplicationDbContext>("mssql");
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb"));
});
builder.Services.AddHttpClient();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductResultConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("product-result-order-qeue", e =>
        {
            e.ConfigureConsumer<ProductResultConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapGet("update-database", (ApplicationDbContext dbContext) =>
{
    dbContext.Database.Migrate();
});

app.MapPost(string.Empty, async (
    OrderCreateDto request,
    HttpClient httpClient,
    [FromHeader(Name = "idempotency-key")] int idempotencyKey,
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    CancellationToken cancellationToken) =>
{
    var isIdempotencyExists = dbContext.Idempotencies.Any(p => p.Key == idempotencyKey);
    if (isIdempotencyExists)
    {
        return Results.Ok("Order is successful");
    }

    //ödemeyi yap
    bool isPaymentSuccessful = true;
    if (!isPaymentSuccessful)
    {
        return Results.BadRequest("Ödeme baþarýsýz oldu");
    }

    Idempotency idempotency = new()
    {
        Key = idempotencyKey,
    };
    dbContext.Add(idempotency);

    Order order = new()
    {
        ProductId = request.ProductId,
        Quantity = request.Quantity
    };
    dbContext.Add(order);
    dbContext.SaveChanges();

    var queueRequest = new OrderCreateQueueDto(order.Id, request.ProductId, request.Quantity);
    await publishEndpoint.Publish(queueRequest);

    return Results.Ok("Order is successful");
});

app.MapGet(string.Empty, (ApplicationDbContext dbContext) =>
{
    return dbContext.Orders.ToList();
});

app.Run();
