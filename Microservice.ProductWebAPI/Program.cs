using Carter;
using Microservice.ProductWebAPI.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddCarter();

builder.Services.AddResponseCompression(res => res.EnableForHttps = true);

builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    var con = builder.Configuration.GetConnectionString("SqlServer");
    opt.UseSqlServer(con);
});

var app = builder.Build();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod()
.SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.UseResponseCompression();

app.MapCarter();

app.Run();