using Carter;
using Microservice.CategoryWebAPI.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    var con = builder.Configuration.GetConnectionString("SqlServer");
    opt.UseSqlServer(con);
});

var app = builder.Build();

app.MapCarter();

app.Run();