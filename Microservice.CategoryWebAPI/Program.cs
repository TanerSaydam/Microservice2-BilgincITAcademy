using Asp.Versioning;
using Carter;
using Microservice.CategoryWebAPI.Context;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(action =>
{
    action.DefaultApiVersion = new ApiVersion(1); //query params da default almak için
    action.AssumeDefaultVersionWhenUnspecified = true; //query params da default almak için
    action.ReportApiVersions = true;
}).AddApiExplorer();

//builder.Services.AddConsulDiscoveryClient();

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddCarter();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    var con = builder.Configuration.GetConnectionString("SqlServer");
    opt.UseSqlServer(con);
});
builder.Services.AddCors();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod()
.SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.MapCarter();
app.MapControllers();

app.MapHealthChecks("health");

app.Run();