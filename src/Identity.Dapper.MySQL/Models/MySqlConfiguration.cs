using Identity.Dapper.Models;

namespace Identity.Dapper.MySQL.Models
{
    public class MySqlConfiguration : SqlConfiguration
    {
        public MySqlConfiguration()
        {
            ParameterNotation = "@";
            SchemaName = "`identity`";
            TableColumnStartNotation = "`";
            TableColumnEndNotation = "`";
            InsertRoleQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            DeleteRoleQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE `Id` = %ID%";
            UpdateRoleQuery = "UPDATE %SCHEMA%.%TABLENAME% %SETVALUES% WHERE `Id` = %ID%";
            SelectRoleByNameQuery = "SELECT * FROM %SCHEMA%.%TABLENAME% WHERE `Name` = %NAME%";
            SelectRoleByIdQuery = "SELECT * FROM %SCHEMA%.%TABLENAME% WHERE `Id` = %ID%";
            InsertUserQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%);SELECT LAST_INSERT_ID();";
            DeleteUserQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE `Id` = %ID%";
            UpdateUserQuery = "UPDATE %SCHEMA%.%TABLENAME% %SETVALUES% WHERE `Id` = %ID%";
            SelectUserByUserNameQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.`UserId` =  %SCHEMA%.%USERTABLE%.`Id` WHERE `UserName` = %USERNAME%";
            SelectUserByEmailQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.`UserId` =  %SCHEMA%.%USERTABLE%.`Id` WHERE `Email` = %EMAIL%";
            SelectUserByIdQuery = "SELECT %SCHEMA%.%USERTABLE%.*, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.`UserId` =  %SCHEMA%.%USERTABLE%.`Id` WHERE `Id` = %ID%";
            InsertUserClaimQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            InsertUserLoginQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            InsertUserRoleQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            GetUserLoginByLoginProviderAndProviderKeyQuery = "SELECT %USERFILTER%, %SCHEMA%.%USERROLETABLE%.* FROM %SCHEMA%.%USERTABLE% LEFT JOIN %SCHEMA%.%USERROLETABLE% ON %SCHEMA%.%USERROLETABLE%.`UserId` = %SCHEMA%.%USERTABLE%.`Id` INNER JOIN %SCHEMA%.%USERLOGINTABLE% ON %SCHEMA%.%USERTABLE%.`Id` = %SCHEMA%.%USERLOGINTABLE%.`UserId` WHERE `LoginProvider` = %LOGINPROVIDER% AND `ProviderKey` = %PROVIDERKEY% LIMIT 1";
            GetClaimsByUserIdQuery = "SELECT `ClaimType`, `ClaimValue` FROM %SCHEMA%.%TABLENAME% WHERE `UserId` = %ID%";
            GetRolesByUserIdQuery = "SELECT `Name` FROM %SCHEMA%.%ROLETABLE%, %SCHEMA%.%USERROLETABLE% WHERE `UserId` = %ID% AND %SCHEMA%.%USERROLETABLE%.`RoleId` = %SCHEMA%.%ROLETABLE%.`Id`";
            GetUserLoginInfoByIdQuery = "SELECT `LoginProvider`, `Name`, `ProviderKey` FROM %SCHEMA%.%TABLENAME% WHERE `UserId` = %ID%";
            GetUsersByClaimQuery = "SELECT %USERFILTER% FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERCLAIMTABLE% WHERE `ClaimValue` = %CLAIMVALUE% AND `ClaimType` = %CLAIMTYPE%";
            GetUsersInRoleQuery = "SELECT %USERFILTER% FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERROLETABLE%, %SCHEMA%.%ROLETABLE% WHERE %SCHEMA%.%ROLETABLE%.`Name` = %ROLENAME% AND %SCHEMA%.%USERROLETABLE%.`RoleId` = %SCHEMA%.%ROLETABLE%.`Id` AND %SCHEMA%.%USERROLETABLE%.`UserId` = %SCHEMA%.%USERTABLE%.`Id`";
            IsInRoleQuery = "SELECT 1 FROM %SCHEMA%.%USERTABLE%, %SCHEMA%.%USERROLETABLE%, %SCHEMA%.%ROLETABLE% WHERE %SCHEMA%.%ROLETABLE%.`Name` = %ROLENAME% AND %SCHEMA%.%USERTABLE%.`Id` = %USERID% AND %SCHEMA%.%USERROLETABLE%.`RoleId` = %SCHEMA%.%ROLETABLE%.`Id` AND %SCHEMA%.%USERROLETABLE%.`UserId` = %SCHEMA%.%USERTABLE%.`Id`";
            RemoveClaimsQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE `UserId` = %ID% AND `ClaimType` = %CLAIMTYPE% AND `ClaimValue` = %CLAIMVALUE%";
            RemoveUserFromRoleQuery = "DELETE FROM %SCHEMA%.%USERROLETABLE% WHERE `UserId` = %USERID% AND `RoleId` = (SELECT `Id` FROM %SCHEMA%.%ROLETABLE% WHERE `Name` = %ROLENAME%)";
            RemoveLoginForUserQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE `UserId` = %USERID% AND `LoginProvider` = %LOGINPROVIDER% AND `ProviderKey` = %PROVIDERKEY%";
            UpdateClaimForUserQuery = "UPDATE %SCHEMA%.%TABLENAME% SET `ClaimType` = %NEWCLAIMTYPE%, `ClaimValue` = %NEWCLAIMVALUE% WHERE `UserId` = %USERID% AND `ClaimType` = %CLAIMTYPE% AND `ClaimValue` = %CLAIMVALUE%";
            SelectClaimByRoleQuery = "SELECT %SCHEMA%.%ROLECLAIMTABLE%.* FROM %SCHEMA%.%ROLETABLE%, %SCHEMA%.%ROLECLAIMTABLE% WHERE `RoleId` = %ROLEID% AND %SCHEMA%.%ROLECLAIMTABLE%.`RoleId` = %SCHEMA%.%ROLETABLE%.`Id`";
            InsertRoleClaimQuery = "INSERT INTO %SCHEMA%.%TABLENAME% %COLUMNS% VALUES(%VALUES%)";
            DeleteRoleClaimQuery = "DELETE FROM %SCHEMA%.%TABLENAME% WHERE `RoleId` = %ROLEID% AND `ClaimType` = %CLAIMTYPE% AND `ClaimValue` = %CLAIMVALUE%";
            RoleTable = "`identityrole`";
            UserTable = "`identityuser`";
            UserClaimTable = "`identityuserclaim`";
            UserRoleTable = "`identityuserrole`";
            UserLoginTable = "`identitylogin`";
            RoleClaimTable = "`identityroleclaim`";
        }
    }
}
