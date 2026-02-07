using Carter;
using FluentEmail.Core;
using MassTransit;
using Microservice.ProductWebAPI;
using Microservice.ProductWebAPI.Consumers;
using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Polly.Registry;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddCarter();
builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCompression(res => res.EnableForHttps = true);

builder.Services.AddConsulDiscoveryClient();

builder.Services.AddCors();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order-product-qeue", e =>
        {
            e.ConfigureConsumer<OrderConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    var con = builder.Configuration.GetConnectionString("SqlServer");
    opt.UseSqlServer(con);
});

builder.AddServiceDefaults();

builder.Services.AddFluentEmail("info@test.com").AddSmtpSender("localhost", 25);

builder.Services.AddPollyPipeline();

var app = builder.Build();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod()
.SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.UseResponseCompression();

app.MapCarter();

app.MapGet("create", (ApplicationDbContext dbContext) =>
{
    Product product = new()
    {
        Name = "Product 1",
        Stock = 10,
        Id = Guid.Parse("3f8242de-a459-4aa6-aed9-e278453ca380"),
        CategoryId = Guid.Parse("cdebccc6-8ebc-4ffe-9e3b-76cd181dda57")
    };

    dbContext.Add(product);
    dbContext.SaveChanges();

    return Results.Ok(new { Message = "Product created" });
});

app.MapGet("test", async (
    IFluentEmail fluentEmail,
    ResiliencePipelineProvider<string> resiliencePipelineProvider
    ) =>
{
    var pipeline = resiliencePipelineProvider.GetPipeline("smtp");
    await pipeline.ExecuteAsync(async callback => await fluentEmail.To("tanersaydam@gmail.com").Subject("Test").Body("Hello world").SendAsync());
});

app.Run(); //15:17 görüþelim