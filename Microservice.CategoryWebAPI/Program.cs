using Asp.Versioning;
using Carter;
using Microservice.CategoryWebAPI.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(action =>
{
    action.DefaultApiVersion = new ApiVersion(1); //query params da default almak için
    action.AssumeDefaultVersionWhenUnspecified = true; //query params da default almak için
    action.ReportApiVersions = true;
}).AddApiExplorer();

builder.Services.AddControllers();
builder.Services.AddCarter();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    var con = builder.Configuration.GetConnectionString("SqlServer");
    opt.UseSqlServer(con);
});

var app = builder.Build();

app.MapCarter();
app.MapControllers();

app.Run(); //11:35 görüþelim