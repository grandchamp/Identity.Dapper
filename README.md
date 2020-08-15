# Identity.Dapper

[![Build Status](https://travis-ci.org/grandchamp/Identity.Dapper.svg?branch=master)](https://travis-ci.org/grandchamp/Identity.Dapper)
[![NuGet](https://img.shields.io/nuget/v/Identity.Dapper.svg?style=flat)](https://www.nuget.org/packages/Identity.Dapper/)

Find the corresponding NuGet package for your DBMS (Eg: Identity.Dapper.SqlServer).

To configure the DBMS connection, you can add a **DapperIdentity** and a **DapperIdentityCryptography** section to your configuration file like this:
```JSON
"DapperIdentity": {
    "ConnectionString": "Connection string of your database",
    "Username": "user",
    "Password": "123"
},
"DapperIdentityCryptography": {
    "Key": "Base64 32 bytes key",
    "IV": "Base64 16 bytes key"
}
```

**Example:**  
Key: "E546C8DF278CD5931069B522E695D4F2" (32 Bytes)  
Base64 Encoded Key: "RTU0NkM4REYyNzhDRDU5MzEwNjlCNTIyRTY5NUQ0RjI="

IV: "SomeReallyCoolIV" (16 Bytes)  
Base64 Encoded IV: "U29tZVJlYWxseUNvb2xJVg=="

Alternatively, you can use **ConnectionStrings** default section:

```JSON
"ConnectionStrings": {
    "DefaultConnection": "Connection string of your database"
}
```

Or you can use the User Secrets commands:
```
dotnet user-secrets set DapperIdentity:ConnectionString "Connection string of your database"
dotnet user-secrets set DapperIdentity:Password "123"
dotnet user-secrets set DapperIdentity:Username "user"

dotnet user-secrets set DapperIdentityCryptography:Key "Base64 32 bytes key"
dotnet user-secrets set DapperIdentityCryptography:IV "Base64 16 bytes key"
```

The **DapperIdentity:Password** can be encrypted with AES256 using the KEY and IV provided.

On **Startup.cs** file, go to **ConfigureServices** and add the following lines:
```csharp
services.ConfigureDapperConnectionProvider<T>(Configuration.GetSection("DapperIdentity"))
        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

services.AddIdentity<DapperIdentityUser, DapperIdentityRole<int>>()
        .AddDapperIdentityFor<T>()
        .AddDefaultTokenProviders();
```

or 

```csharp
services.ConfigureDapperConnectionProvider<T>(Configuration.GetSection("ConnectionStrings"))
```

Where ***T*** for the method ```ConfigureDapperConnectionProvider``` is ```DBMSNameConnectionProvider``` (eg: ```SqlServerConnectionProvider```) and ***T*** for the method ```AddDapperIdentityFor``` is ```DBMSNameConfiguration``` (eg: ```SqlServerConfiguration```).

If you want to use Transactions to all methods of Identity, you'll have to add `.ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = true })` below `ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));`

And inside your controller, you'll have to insert on constructor a `DapperUserStore<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>` variable, like this:

```csharp
private readonly DapperUserStore<CustomUser, int, DapperIdentityUserRole<int>, DapperIdentityRoleClaim<int>, DapperIdentityUserClaim<int>, DapperIdentityUserLogin<int>, CustomRole> _dapperStore;

...

 public ManageController(IUserStore<CustomUser> dapperStore)
        {
            _dapperStore = dapperStore as DapperUserStore<CustomUser, int, DapperIdentityUserRole<int>, DapperIdentityRoleClaim<int>, DapperIdentityUserClaim<int>, DapperIdentityUserLogin<int>, CustomRole>;
        }
```

And after all operations, you'll have to call `DapperUserStore.SaveChanges()` method, otherwise your changes will be rollbacked.

Currently, only SQL Server, PostgreSQL and MySQL are supported. We plan support for Oracle when the company release the .NET Core version for their System.Data implementation.

## Using Guid as Entity key
Specify the <TKey>
```csharp
services.AddIdentity<DapperIdentityUser<Guid>, DapperIdentityRole<Guid>>()
        .AddDapperIdentityFor<T, Guid>();
```

## Changing the default schema (SqlServer)

Pass a custom class that inherits from ```SqlServerConfiguration``` (or other)

```csharp
public class CustomSqlServerConfiguration : SqlServerConfiguration
{
    public CustomSqlServerConfiguration()
    {
        base.SchemaName = "[customSchema]";
    }
}
```

And add it with
```csharp
services.AddDapperIdentityFor<CustomSqlServerConfiguration>()
```
