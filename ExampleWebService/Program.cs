
using ExampleWebService;
using ExampleWebService.Domain;
using ExampleWebService.Domain.Domain.V3;
using ExampleWebService.Domain.Domain.V4;
using ExampleWebService.Domain.Migrations;
using ExampleWebService.Domain.Repo;
using Microsoft.EntityFrameworkCore;
using MongoDbEFMigrations.Common;

var builder = WebApplication.CreateBuilder(args);

var dbConnString = builder.Configuration["MongoDB:ConnectionString"] ?? 
                               throw new ArgumentNullException("MongoDB:ConnectionString must be defined in appsettings");
var dbName = builder.Configuration["MongoDB:DatabaseName"] ?? 
                   throw new ArgumentNullException("MongoDB:DatabaseName must be defined in appsettings");

builder.Services.AddDbContext<CustomerDb>(x =>
{
    x.UseMongoDB(dbConnString, dbName);
});

var customerUpgrader = new MigrationRunner<IMigrate<CustomerDbEntity>, CustomerDbEntity>
(
    new List<IMigrate<CustomerDbEntity>>
    {
        new CustomerV1Migrate(),
        new CustomerV2Migrate(),
        new CustomerV3Migrate(),
        new CustomerV4Migrate()
    }
);
builder.Services.AddSingleton(customerUpgrader);

builder.Services.AddTransient<Repository>();
builder.Services.AddTransient<ServiceV3>();
builder.Services.AddTransient<ServiceV4>();

var app = builder.Build();

// service v3
app.MapGet("/customer/v3/{id}", (string id, ServiceV3 service) => service.GetAsync(id));
app.MapPost("/customer/v3/", (CustomerV3 customer, ServiceV3 service) => service.AddAsync(customer));

// service v4
app.MapGet("/customer/v4/{id}", (string id, ServiceV4 service) => service.GetAsync(id));
app.MapPost("/customer/v4/", (CustomerV4 customer, ServiceV4 service) => service.AddAsync(customer));

app.Run();