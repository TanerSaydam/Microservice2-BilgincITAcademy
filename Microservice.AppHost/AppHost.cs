using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region Variables
var userName = builder.AddParameter("SqlUserName", "sa", true);
var password = builder.AddParameter("SqlPassword", "Password12*", true);
#endregion

var mssql = builder.AddSqlServer("mssql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHostPort(1233)
    .WithPassword(password);

var db = mssql.AddDatabase("OrderDb");

var rabbitMq = builder.AddRabbitMQ("rabbitMQ", null, null, 5672)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImage("rabbitmq", "3-management")
    .WithHttpEndpoint(port: 15672, targetPort: 15672, name: "management")
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
    ;

builder.AddProject<Microservice_AuthWebAPI>("auth");
builder.AddProject<Microservice_CategoryWebAPI>("category");
builder.AddProject<Microservice_ProductWebAPI>("product");
builder.AddProject<Microservice_OrderWebAPI>("order").WithReference(db).WaitFor(db);

builder.AddProject<Microservice_OcelotGateway>("ocelot");
builder.AddProject<Microservice_YARPGateway>("yarp");

builder.Build().Run();
