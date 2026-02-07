using Microservice.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetSection("KeycloakConfiguration"));
builder.Services.AddTransient<KeycloakService>();
builder.Services.AddHttpClient();
//builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
//builder.Services.AddAuthorization(opt =>
//{
//    opt.AddPolicy("test", policy =>
//    {
//        policy.RequireResourceRoles("test");
//    });
//}).AddKeycloakAuthorization(builder.Configuration);

var app = builder.Build();

app.MapGet("login", async (KeycloakService keycloakService) =>
{
    var res = await keycloakService.CreateToken("taner", "1", default);
    return res;
});

app.Run();
