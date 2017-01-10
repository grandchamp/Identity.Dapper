using Identity.Dapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Dapper.PostgreSQL.Models
{
    public class PostgreSqlConfiguration : SqlConfiguration
    {
        public PostgreSqlConfiguration()
        {
            ParameterNotation = "@";
            SchemaName = "dbo";
            InsertRoleQuery = "INSERT INTO %SCHEMA%.\"%TABLENAME%\" %COLUMNS% VALUES(%VALUES%)";
            DeleteRoleQuery = "DELETE FROM %SCHEMA%.\"%TABLENAME%\" WHERE \"Id\" = %ID%";
            UpdateRoleQuery = "UPDATE %SCHEMA%.\"%TABLENAME%\" %SETVALUES% WHERE \"Id\" = %ID%";
            SelectRoleByNameQuery = "SELECT * FROM %SCHEMA%.\"%TABLENAME%\" WHERE UPPER(\"Name\") = %NAME%";
            SelectRoleByIdQuery = "SELECT * FROM %SCHEMA%.\"%TABLENAME%\" WHERE \"Id\" = %ID%";
            InsertUserQuery = "INSERT INTO %SCHEMA%.\"%TABLENAME%\" %COLUMNS% VALUES(%VALUES%) RETURNING \"Id\"";
            DeleteUserQuery = "DELETE FROM %SCHEMA%.\"%TABLENAME%\" WHERE \"Id\" = %ID%";
            UpdateUserQuery = "UPDATE %SCHEMA%.\"%TABLENAME%\" %SETVALUES% WHERE \"Id\" = %ID%";
            SelectUserByUserNameQuery = "SELECT * FROM %SCHEMA%.\"%TABLENAME%\" WHERE UPPER(\"UserName\") = %USERNAME%";
            SelectUserByEmailQuery = "SELECT * FROM %SCHEMA%.\"%TABLENAME%\" WHERE UPPER(\"Email\") = %EMAIL%";
            SelectUserByIdQuery = "SELECT * FROM %SCHEMA%.\"%TABLENAME%\" WHERE \"Id\" = %ID%";
            InsertUserClaimQuery = "INSERT INTO %SCHEMA%.\"%TABLENAME%\" %COLUMNS% VALUES(%VALUES%)";
            InsertUserLoginQuery = "INSERT INTO %SCHEMA%.\"%TABLENAME%\" %COLUMNS% VALUES(%VALUES%)";
            InsertUserRoleQuery = "INSERT INTO %SCHEMA%.\"%TABLENAME%\" %COLUMNS% VALUES(%VALUES%)";
            GetUserLoginByLoginProviderAndProviderKeyQuery = "SELECT %USERFILTER% FROM %SCHEMA%.\"%USERTABLE%\", %SCHEMA%.\"%USERLOGINTABLE%\" WHERE \"LoginProvider\" = %LOGINPROVIDER% AND \"ProviderKey\" = %PROVIDERKEY% LIMIT 1";
            GetClaimsByUserIdQuery = "SELECT \"ClaimType\", \"ClaimValue\" FROM %SCHEMA%.\"%TABLENAME%\" WHERE \"UserId\" = %ID%";
            GetRolesByUserIdQuery = "SELECT \"Name\" FROM %SCHEMA%.\"%ROLETABLE%\", %SCHEMA%.\"%USERROLETABLE%\" WHERE \"UserId\" = %ID% AND %SCHEMA%.\"%USERROLETABLE%\".\"RoleId\" = %SCHEMA%.\"%ROLETABLE%\".\"Id\"";
            GetUserLoginInfoByIdQuery = "SELECT \"LoginProvider\", \"ProviderKey\", \"Name\" FROM %SCHEMA%.\"%TABLENAME%\" WHERE \"UserId\" = %ID%";
            GetUsersByClaimQuery = "SELECT %USERFILTER% FROM %SCHEMA%.\"%USERTABLE%\", %SCHEMA%.\"%USERCLAIMTABLE%\" WHERE \"ClaimValue\" = %CLAIMVALUE% AND \"ClaimType\" = %CLAIMTYPE%";
            GetUsersInRoleQuery = "SELECT %USERFILTER% FROM %SCHEMA%.\"%USERTABLE%\", %SCHEMA%.\"%USERROLETABLE%\", %SCHEMA%.\"%ROLETABLE%\" WHERE UPPER(%SCHEMA%.\"%ROLETABLE%\".\"Name\") = %ROLENAME% AND %SCHEMA%.\"%USERROLETABLE%\".\"RoleId\" = %SCHEMA%.\"%ROLETABLE%\".\"Id\" AND %SCHEMA%.\"%USERROLETABLE%\".\"UserId\" = %SCHEMA%.\"%USERTABLE%\".\"Id\"";
            IsInRoleQuery = "SELECT 1 FROM %SCHEMA%.\"%USERTABLE%\", %SCHEMA%.\"%USERROLETABLE%\", %SCHEMA%.\"%ROLETABLE%\" WHERE UPPER(%SCHEMA%.\"%ROLETABLE%\".\"Name\") = %ROLENAME% AND %SCHEMA%.\"%USERTABLE%\".\"Id\" = %USERID% AND %SCHEMA%.\"%USERROLETABLE%\".\"RoleId\" = %SCHEMA%.\"%ROLETABLE%\".\"Id\" AND %SCHEMA%.\"%USERROLETABLE%\".\"UserId\" = %SCHEMA%.\"%USERTABLE%\".\"Id\"";
            RemoveClaimsQuery = "DELETE FROM %SCHEMA%.\"%TABLENAME%\" WHERE \"UserId\" = %ID% AND \"ClaimType\" = %CLAIMTYPE% AND \"ClaimValue\" = %CLAIMVALUE%";
            RemoveUserFromRoleQuery = "DELETE FROM %SCHEMA%.\"%USERROLETABLE%\" UR USING %SCHEMA%.\"%ROLETABLE%\" R WHERE UR.\"RoleId\" = R.\"Id\" AND UR.\"UserId\" = %USERID% AND UPPER(R.\"Name\") = %ROLENAME%";
            RemoveLoginForUserQuery = "DELETE FROM %SCHEMA%.\"%TABLENAME%\" WHERE \"UserId\" = %USERID% AND \"LoginProvider\" = %LOGINPROVIDER% AND \"ProviderKey\" = %PROVIDERKEY%";
            UpdateClaimForUserQuery = "UPDATE %SCHEMA%.\"%TABLENAME%\" SET \"ClaimType\" = %NEWCLAIMTYPE% AND \"ClaimValue\" = %NEWCLAIMVALUE% WHERE \"UserId\" = %USERID% AND \"ClaimType\" = %CLAIMTYPE% AND \"ClaimValue\" = %CLAIMVALUE%";
            RoleTable = "IdentityRole";
            UserTable = "IdentityUser";
            UserClaimTable = "IdentityUserClaim";
            UserRoleTable = "IdentityUserRole";
            UserLoginTable = "IdentityLogin";
        }
    }
}
