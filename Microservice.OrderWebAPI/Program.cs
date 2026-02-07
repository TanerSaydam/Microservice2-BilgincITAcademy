using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Dtos;
using Microservice.OrderWebAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.AddSqlServerDbContext<ApplicationDbContext>("mssql");
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb"));
});

var app = builder.Build();

//app.MapGet("update-database", (ApplicationDbContext dbContext) =>
//{
//    dbContext.Database.Migrate();
//});

app.MapPost(string.Empty, (OrderCreateDto request, ApplicationDbContext dbContext) =>
{
    Order order = new()
    {
        ProductId = request.ProductId,
        Quantity = request.Quantity
    };
    dbContext.Add(order);
    dbContext.SaveChanges();
});

app.MapGet(string.Empty, (ApplicationDbContext dbContext) =>
{
    return dbContext.Orders.ToList();
});

app.Run();
