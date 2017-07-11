# Identity.Dapper
**Not ready yet**

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
services.ConfigureDapperXXXConnectionProvider(Configuration.GetSection("DapperIdentity"))
        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

services.AddIdentity<DapperIdentityUser, DapperIdentityRole<int>>()
        .AddDapperIdentityForXXX()
        .AddDefaultTokenProviders();
```

All **XXX** are replaced by your DBMS.

Currently, only SQL Server, PostgreSQL and MySQL are supported. We plan support for Oracle when the company release the .NET Core version for their System.Data implementation.

## Using Guid as Entity key
Specify the <TKey>
```
services.AddIdentity<DapperIdentityUser<Guid>, DapperIdentityRole<Guid>>()
        .AddDapperIdentityForXXX<Guid>();
```

## Changing the default schema (SqlServer)
Pass a SqlServerConfiguration()
```
services.AddDapperIdentityForSqlServer(new CustomSqlServerConfiguration())
```
Extend the Identity.Dapper.SqlServer.Models.SqlServerConfiguration
```
public class CustomSqlServerConfiguration : SqlServerConfiguration
{
    public CustomSqlServerConfiguration()
    {
        base.SchemaName = "[customSchema]";
    }
}
```
