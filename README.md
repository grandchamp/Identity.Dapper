# Identity.Dapper
**Not ready yet**

Add a reference on your **project.json** file to the corresponding DBMS (Eg: SQL Server)
```
"dependencies": {
    "Microsoft.NETCore.App": {
      "version": "1.0.0-rc2-3002702",
      "type": "platform"
    },
    "Identity.Dapper.XXX": "1.0.0-*"
  }
```

On your configuration file add a section named **DapperIdentity** and **DapperIdentityCryptography**
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

The password can be encrypted with AES256 using the KEY and IV provided.

On **Startup.cs** file, go to **ConfigureServices** and add the following lines:
```
services.Configure<SqlConfiguration>(Configuration.GetSection("SqlConfiguration"));
services.ConfigureDapperXXXConnectionProvider(Configuration.GetSection("DapperIdentity"))
        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

services.AddIdentity<DapperIdentityUser, DapperIdentityRole<int>>()
        .AddDapperIdentityForXXX()
        .AddDefaultTokenProviders();
```

All **XXX** are replaced by your DBMS.

Currently, only SQL Server is supported. We plan support for Oracle and PostgreSQL.
