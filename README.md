# MongoDbEFMigrations

## What is it?
One problem when using Entity Framework (EF) with MongoDB is that of data migrations. This includes breaking changes where:
- A new mandatory field is added to a model
- A field is renamed
- A field type is changed

MongoDB is schema-free and so writes to the database will work just fine. However, when trying to fetch these entities from the database,
EF will break when trying to deserialise the documents into the strongly typed objects.

## Database data migrations
The problem with migrating the data in one sweep is that the service will be broken during this time, resulting in downtime.
Instead or in addition to this, a softer, 'On-The-Fly' approach means that the data can be upgraded on-demand, until it is eventually upgraded.

## On-The-Fly migrations
This approach means that when we fetch a record from the database that is different to the version we want, we can migrate it on-the-fly.
This will support both upgrades, but also downgrades for when an older version of the API needs to still be supported.

## How to use this code

**This code is a concrete example implementation - to use it, you will need to remove the example Migrations and entities and write your own.**

Clone the repo and run it against a local MongoDB - you many need to change the application settings.

Once running, you can hit different versions of the service to see how different versions of the data are returned.

```
POST http://localhost:5184/customer/v0

{
    "CustomerId": "c0",
    "FirstName": "John",
    "LastName": "Doe"
}
```

```
POST http://localhost:5184/customer/v3

{
    "CustomerId": "c3",
    "FullName": "John Doe",
    "Age": 30
}
```

```
POST http://localhost:5184/customer/v4

{
    "CustomerId": "c4",
    "FullName": "John Doe",
    "DateOfBirth": "1978-12-01"
}
```

Then try fetching the two items and see how they are upgraded.
```
GET http://localhost:5184/customer/v4/c0 - upgrade V0 -> V1 -> V2 -> V3 -> V4
```
```
GET http://localhost:5184/customer/v3/c3 - no upgrade
GET http://localhost:5184/customer/v4/c3 - upgraded (DateOfBirth calculated from Age)
```
```
GET http://localhost:5184/customer/v0/c4 - downgrade V4 -> V3 -> V2 -> V1 -> V0
GET http://localhost:5184/customer/v3/c4 - downgrade V4 -> V3
GET http://localhost:5184/customer/v4/c4 - no upgrade
```
