using Asp.Versioning;
using Carter;
using Microservice.CategoryWebAPI.Context;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(action =>
{
    action.DefaultApiVersion = new ApiVersion(1); //query params da default almak için
    action.AssumeDefaultVersionWhenUnspecified = true; //query params da default almak için
    action.ReportApiVersions = true;
}).AddApiExplorer();

builder.AddServiceDefaults();

builder.Services.AddConsulDiscoveryClient();

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

builder.Services.AddRateLimiter(conf =>
{
    conf.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 1;
        //opt.QueueLimit = 1;
        //opt.QueueProcessingOrder = QueueProcessingOrder.NewestFirst;
    });
});

//builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
//builder.Services.AddAuthorization();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.Use((context, next) =>
{
    string token = context.Request.Headers!.Authorization!;
    Console.WriteLine(token);
    return next(context);
});

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod()
.SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapCarter();
app.MapControllers();

app.MapHealthChecks("health");

app.Run();