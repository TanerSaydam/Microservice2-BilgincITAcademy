using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.AddSqlServerDbContext<ApplicationDbContext>("mssql");
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb"));
});

var app = builder.Build();

app.MapGet("update-database", (ApplicationDbContext dbContext) =>
{
    dbContext.Database.Migrate();
});

app.MapGet("create", (ApplicationDbContext dbContext) =>
{
    Order order = new()
    {
        ProductId = Guid.CreateVersion7()
    };
    dbContext.Add(order);
    dbContext.SaveChanges();
});

app.MapGet("get-all", (ApplicationDbContext dbContext) =>
{
    return dbContext.Orders.ToList();
});

app.Run();
