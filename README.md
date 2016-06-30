# Identity.Dapper
**Not ready yet**

Add a reference on your **project.json** file to the corresponding DBMS (Eg: SQL Server)
```
"dependencies": {
    "Microsoft.NETCore.App": {
      "version": "1.0.0",
      "type": "platform"
    },
    "Identity.Dapper.XXX": "0.3.0-*"
  }
```

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

Currently, only SQL Server and PostgreSQL are supported. We plan support for Oracle and MySQL when the companies release the .NET Core version for their System.Data implementation.
