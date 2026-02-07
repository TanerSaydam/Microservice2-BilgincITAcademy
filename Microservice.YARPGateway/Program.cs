using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

var app = builder.Build();

app.MapGet("", () => "Hello world from YARP");

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();
