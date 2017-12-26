# Identity.Dapper
[![Build Status](https://travis-ci.org/grandchamp/Identity.Dapper.svg?branch=master)](https://travis-ci.org/grandchamp/Identity.Dapper)

Find the corresponding NuGet package for your DBMS (Eg: Identity.Dapper.SqlServer).

To configure the DBMS connection, you can add a **DapperIdentity** and a **DapperIdentityCryptography** section to your configuration file like this:
```
"DapperIdentity": {
    "ConnectionString": "Connection string of your database",
    "Username": "user",
    "Password": "123"
},
"DapperIdentityCryptography": {
    "Key": "base64 32 bits key",
    "IV": "base64 16 bits key"
}
```

Alternatively, you can use **ConnectionStrings** default section:

```
"ConnectionStrings": {
    "DefaultConnection": "Connection string of your database"
}
```

Or you can use the User Secrets commands:
```
dotnet user-secrets set DapperIdentity:ConnectionString "Connection string of your database"
dotnet user-secrets set DapperIdentity:Password "123"
dotnet user-secrets set DapperIdentity:Username "user"

dotnet user-secrets set DapperIdentityCryptography:Key "base64 32 bits key"
dotnet user-secrets set DapperIdentityCryptography:IV "base64 16 bits key"
```

The **DapperIdentity:Password** can be encrypted with AES256 using the KEY and IV provided.

On **Startup.cs** file, go to **ConfigureServices** and add the following lines:
```
services.ConfigureDapperConnectionProvider<T>(Configuration.GetSection("DapperIdentity"))
        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

services.AddIdentity<DapperIdentityUser, DapperIdentityRole<int>>()
        .AddDapperIdentityFor<T>()
        .AddDefaultTokenProviders();
```

Where ***T*** for the method ```ConfigureDapperConnectionProvider``` is ```DBMSNameConnectionProvider``` (eg: ```SqlServerConnectionProvider```) and ***T*** for the method ```AddDapperIdentityFor``` is ```DBMSNameConfiguration``` (eg: ```SqlServerConfiguration```).

If you want to use Transactions to all methods of Identity, you'll have to add `.ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = true })` below `ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));`

And inside your controller, you'll have to insert on constructor a `DapperUserStore<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>` variable, like this:

```
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
```
services.AddIdentity<DapperIdentityUser<Guid>, DapperIdentityRole<Guid>>()
        .AddDapperIdentityFor<T, Guid>();
```

## Changing the default schema (SqlServer)

Pass a custom class that inherits from ```SqlServerConfiguration``` (or other)

```
public class CustomSqlServerConfiguration : SqlServerConfiguration
{
    public CustomSqlServerConfiguration()
    {
        base.SchemaName = "[customSchema]";
    }
}
```

And add it with
```
services.AddDapperIdentityFor<CustomSqlServerConfiguration>()
```
