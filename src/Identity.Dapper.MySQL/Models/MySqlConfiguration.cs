using Identity.Dapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Dapper.MySQL.Models
{
    public class MySqlConfiguration : SqlConfiguration
    {
        public MySqlConfiguration()
        {
            ParameterNotation = "@";
            SchemaName = "identity";
            UseQuotationMarks = false;
            InsertRoleQuery = "INSERT INTO `%SCHEMA%`.`%TABLENAME%` %COLUMNS% VALUES(%VALUES%)";
            DeleteRoleQuery = "DELETE FROM `%SCHEMA%`.`%TABLENAME%` WHERE `id` = %ID%";
            UpdateRoleQuery = "UPDATE `%SCHEMA%`.`%TABLENAME%` %SETVALUES% WHERE `id` = %ID%";
            SelectRoleByNameQuery = "SELECT * FROM `%SCHEMA%`.`%TABLENAME%` WHERE `name` = %NAME%";
            SelectRoleByIdQuery = "SELECT * FROM `%SCHEMA%`.`%TABLENAME%` WHERE `id` = %ID%";
            InsertUserQuery = "INSERT INTO `%SCHEMA%`.`%TABLENAME%` %COLUMNS% VALUES(%VALUES%);SELECT LAST_INSERT_ID();";
            DeleteUserQuery = "DELETE FROM `%SCHEMA%`.`%TABLENAME%` WHERE `id` = %ID%";
            UpdateUserQuery = "UPDATE `%SCHEMA%`.`%TABLENAME%` %SETVALUES% WHERE `id` = %ID%";
            SelectUserByUserNameQuery = "SELECT * FROM `%SCHEMA%`.`%TABLENAME%` WHERE `username` = %USERNAME%";
            SelectUserByEmailQuery = "SELECT * FROM `%SCHEMA%`.`%TABLENAME%` WHERE `email` = %EMAIL%";
            SelectUserByIdQuery = "SELECT * FROM `%SCHEMA%`.`%TABLENAME%` WHERE `id` = %ID%";
            InsertUserClaimQuery = "INSERT INTO `%SCHEMA%`.`%TABLENAME%` %COLUMNS% VALUES(%VALUES%)";
            InsertUserLoginQuery = "INSERT INTO `%SCHEMA%`.`%TABLENAME%` %COLUMNS% VALUES(%VALUES%)";
            InsertUserRoleQuery = "INSERT INTO `%SCHEMA%`.`%TABLENAME%` %COLUMNS% VALUES(%VALUES%)";
            GetUserLoginByLoginProviderAndProviderKeyQuery = "SELECT %USERFILTER% FROM `%SCHEMA%`.`%USERTABLE%`, `%SCHEMA%`.`%USERLOGINTABLE%` WHERE `loginprovider` = %LOGINPROVIDER% AND `providerkey` = %PROVIDERKEY% LIMIT 1";
            GetClaimsByUserIdQuery = "SELECT `claimtype`, `claimvalue` FROM `%SCHEMA%`.`%TABLENAME%` WHERE `userid` = %ID%";
            GetRolesByUserIdQuery = "SELECT `name` FROM `%SCHEMA%`.`%ROLETABLE%`, `%SCHEMA%`.`%USERROLETABLE%` WHERE `userid` = %ID%";
            GetUserLoginInfoByIdQuery = "SELECT `loginprovider`, `providerkey`, `name` FROM `%SCHEMA%`.`%TABLENAME%` WHERE `userid` = %ID%";
            GetUsersByClaimQuery = "SELECT %USERFILTER% FROM `%SCHEMA%`.`%USERTABLE%`, `%SCHEMA%`.`%USERCLAIMTABLE%` WHERE `claimvalue` = %CLAIMVALUE% AND `claimtype` = %CLAIMTYPE%";
            GetUsersInRoleQuery = "SELECT %USERFILTER% FROM `%SCHEMA%`.`%USERTABLE%`, `%SCHEMA%`.`%USERROLETABLE%`, `%SCHEMA%`.`%ROLETABLE%` WHERE `%SCHEMA%`.`%ROLETABLE%`.`name` = %ROLENAME% AND `%SCHEMA%`.`%USERROLETABLE%`.`roleid` = `%SCHEMA%`.`%ROLETABLE%`.`id` AND `%SCHEMA%`.`%USERROLETABLE%`.`userid` = `%SCHEMA%`.`%USERTABLE%`.`id`";
            IsInRoleQuery = "SELECT 1 FROM `%SCHEMA%`.`%USERTABLE%`, `%SCHEMA%`.`%USERROLETABLE%`, `%SCHEMA%`.`%ROLETABLE%` WHERE `%SCHEMA%`.`%ROLETABLE%`.`name` = %ROLENAME% AND `%SCHEMA%`.`%USERTABLE%`.`id` = %USERID% AND `%SCHEMA%`.`%USERROLETABLE%`.`roleid` = `%SCHEMA%`.`%ROLETABLE%`.`id` AND `%SCHEMA%`.`%USERROLETABLE%`.`userid` = `%SCHEMA%`.`%USERTABLE%`.`id`";
            RemoveClaimsQuery = "DELETE FROM `%SCHEMA%`.`%TABLENAME%` WHERE `userid` = %ID% AND `claimtype` = %CLAIMTYPE% AND `claimvalue` = %CLAIMVALUE%";
            RemoveUserFromRoleQuery = "DELETE FROM `%SCHEMA%`.`%USERROLETABLE%`, `%SCHEMA%`.`%ROLETABLE%` WHERE `userid` = %USERID% AND `roleid` = Id AND `name` = %ROLENAME%";
            RemoveLoginForUserQuery = "DELETE FROM `%SCHEMA%`.`%TABLENAME%` WHERE `userid` = %USERID% AND `loginprovider` = %LOGINPROVIDER% AND `providerkey` = %PROVIDERKEY%";
            UpdateClaimForUserQuery = "UPDATE `%SCHEMA%`.`%TABLENAME%` SET `claimtype` = %NEWCLAIMTYPE% AND `claimvalue` = %NEWCLAIMVALUE% WHERE `userid` = %USERID% AND `claimtype` = %CLAIMTYPE% AND `claimvalue` = %CLAIMVALUE%";
            RoleTable = "identityrole";
            UserTable = "identityuser";
            UserClaimTable = "identityuserclaim";
            UserRoleTable = "identityuserrole";
            UserLoginTable = "identitylogin";
        }
    }
}
