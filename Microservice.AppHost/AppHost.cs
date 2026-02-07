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

builder.AddProject<Microservice_AuthWebAPI>("auth");
builder.AddProject<Microservice_CategoryWebAPI>("category");
builder.AddProject<Microservice_ProductWebAPI>("product");
builder.AddProject<Microservice_OrderWebAPI>("order").WithReference(db).WaitFor(db);

builder.AddProject<Microservice_OcelotGateway>("ocelot");
builder.AddProject<Microservice_YARPGateway>("yarp");

builder.Build().Run();
