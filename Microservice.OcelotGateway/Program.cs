using Keycloak.AuthServices.Authentication;
using Microservice.OcelotGateway;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json");
builder.Services
    .AddOcelot(builder.Configuration)
    .AddConsul<MyConsulServiceBuilder>();

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();
await app.RunAsync();