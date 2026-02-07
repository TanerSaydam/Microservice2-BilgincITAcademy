using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(conf =>
{
    conf.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 100;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    string authority = builder.Configuration.GetSection("Keycloak:authority")!.Value!;
    options.Authority = authority;
    options.TokenValidationParameters.ValidateAudience = false;
    options.RequireHttpsMetadata = false;
});
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("require-authentication", policy => policy.RequireAuthenticatedUser());
    x.AddPolicy("MyPolicy", p => p.RequireClaim("name", "Taner Saydam"));
});

var app = builder.Build();

app.MapGet("", () => "Hello world from YARP");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
