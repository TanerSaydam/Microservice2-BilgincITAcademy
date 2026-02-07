using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Dtos;
using Microservice.OrderWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.AddSqlServerDbContext<ApplicationDbContext>("mssql");
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb"));
});
builder.Services.AddHttpClient();

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

    //productdan stock adedini düþ
    var productReq = new
    {
        ProductId = request.ProductId,
        Quantity = request.Quantity,
    };
    var message = await httpClient.PutAsJsonAsync("http://localhost:5004/products", productReq, cancellationToken);
    if (!message.IsSuccessStatusCode)
    {
        //ödemeyi rollback yap
        return Results.BadRequest("Something went wrong");
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

    bool isFail = true;
    if (isFail)
    {
        //hem ödemeyi iade et hem product adedini düzelt ya da rollback yap
        throw new ArgumentException("something went wrong 2");
    }
    dbContext.SaveChanges();

    return Results.Ok("Order is successful");
});

app.MapGet(string.Empty, (ApplicationDbContext dbContext) =>
{
    return dbContext.Orders.ToList();
});

app.Run();
