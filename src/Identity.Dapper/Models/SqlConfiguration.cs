using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Dapper.Models
{
    public class SqlConfiguration
    {
        public SqlConfiguration()
        {
            ParameterNotation = "@";
            RoleTable = "IdentityRole";
            SchemaName = "[dbo]";
        }

        public string SchemaName { get; set; }
        public string ParameterNotation { get; set; }

        #region Table Names
        public string RoleTable { get; set; }
        public string UserTable { get; set; }
        public string UserClaimTable { get; set; }
        public string UserLoginTable { get; set; }
        public string UserRoleTable { get; set; }
        #endregion

        #region Role Queries
        public string InsertRoleQuery { get; set; }
        public string DeleteRoleQuery { get; set; }
        public string UpdateRoleQuery { get; set; }
        public string SelectRoleByNameQuery { get; set; }
        public string SelectRoleByIdQuery { get; set; }
        #endregion

        #region User Queries
        public string InsertUserQuery { get; set; }
        public string DeleteUserQuery { get; set; }
        public string UpdateUserQuery { get; set; }
        public string SelectUserByUserNameQuery { get; set; }
        public string InsertUserClaimQuery { get; set; }
        public string InsertUserLoginQuery { get; set; }
        public string GetUserLoginByLoginProviderAndProviderKey { get; set; }
        public string GetClaimsByUserIdQuery { get; set; }
        public string GetUserLoginInfoByIdQuery { get; set; }
        public string GetUsersByClaimQuery { get; set; }
        public string GetUsersInRoleQuery { get; set; }
        public string IsInRoleQuery { get; set; }
        #endregion
    }
}
