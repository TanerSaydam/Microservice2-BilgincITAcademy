using Microservice.OcelotGateway;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json");
builder.Services
    .AddOcelot(builder.Configuration)
    .AddConsul<MyConsulServiceBuilder>();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    string authority = builder.Configuration.GetSection("Keycloak:authority")!.Value!;
    options.Authority = authority;
    options.TokenValidationParameters.ValidateAudience = false;
    //options.TokenValidationParameters.ValidAudience = "account";
    options.RequireHttpsMetadata = false; //sadece development ortamý için. Prod alacaksanýz burayý sil
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();
await app.RunAsync();