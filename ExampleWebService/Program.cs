using ExampleWebService.Domain;
using ExampleWebService.Domain.Domain;
using ExampleWebService.Domain.Domain.V0;
using ExampleWebService.Domain.Domain.V1;
using ExampleWebService.Domain.Domain.V2;
using ExampleWebService.Domain.Domain.V3;
using ExampleWebService.Domain.Domain.V4;
using ExampleWebService.Domain.Migrations;
using ExampleWebService.Domain.Repo;
using Microsoft.EntityFrameworkCore;
using MongoDbEFMigrations.Common;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var dbConnString = builder.Configuration["MongoDB:ConnectionString"] ?? 
                               throw new ArgumentNullException("MongoDB:ConnectionString must be defined in appsettings");
var dbName = builder.Configuration["MongoDB:DatabaseName"] ?? 
                   throw new ArgumentNullException("MongoDB:DatabaseName must be defined in appsettings");

builder.Services.AddDbContext<CustomerDb>(x =>
{
    x.UseMongoDB(dbConnString, dbName);
});

var customerUpgrader = new CustomerDbEntityConverter
(
    new List<DbEntityMigratorBase<CustomerDbEntity>>
    {
        new CustomerV0DbEntityMigrator(),
        new CustomerV1DbEntityMigrator(),
        new CustomerV2DbEntityMigrator(),
        new CustomerV3DbEntityMigrator(),
        new CustomerV4DbEntityMigrator()
    },
    AutoMapperConfig.CreateMapper()
);
builder.Services.AddSingleton(customerUpgrader);

builder.Services.AddTransient<Repository>();
builder.Services.AddTransient<ServiceV0>();
builder.Services.AddTransient<ServiceV1>();
builder.Services.AddTransient<ServiceV2>();
builder.Services.AddTransient<ServiceV3>();
builder.Services.AddTransient<ServiceV4>();

var app = builder.Build();

// service v0
app.MapGet("/customer/v0/{id}", (string id, ServiceV0 service) => service.GetAsync(id));
app.MapPost("/customer/v0/", (CustomerV0 customer, ServiceV0 service) => service.AddAsync(customer));

// service v1
app.MapGet("/customer/v1/{id}", (string id, ServiceV1 service) => service.GetAsync(id));
app.MapPost("/customer/v1/", (CustomerV1 customer, ServiceV1 service) => service.AddAsync(customer));

// service v3
app.MapGet("/customer/v2/{id}", (string id, ServiceV2 service) => service.GetAsync(id));
app.MapPost("/customer/v2/", (CustomerV2 customer, ServiceV2 service) => service.AddAsync(customer));

// service v3
app.MapGet("/customer/v3/{id}", (string id, ServiceV3 service) => service.GetAsync(id));
app.MapPost("/customer/v3/", (CustomerV3 customer, ServiceV3 service) => service.AddAsync(customer));

// service v4
app.MapGet("/customer/v4/{id}", (string id, ServiceV4 service) => service.GetAsync(id));
app.MapPost("/customer/v4/", (CustomerV4 customer, ServiceV4 service) => service.AddAsync(customer));

app.Run();