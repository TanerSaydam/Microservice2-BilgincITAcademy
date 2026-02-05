using Carter;
using Microservice.ProductWebAPI.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddCarter();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    var con = builder.Configuration.GetConnectionString("SqlServer");
    opt.UseSqlServer(con);
});

var app = builder.Build();

app.MapCarter();

app.Run();