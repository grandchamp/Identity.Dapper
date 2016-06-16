using Identity.Dapper.Entities;
using Identity.Dapper.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Dapper;
using Identity.Dapper.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Identity.Dapper.Models;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data.Common;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Identity.Dapper.Repositories
{
    public class UserRepository<TUser, TKey, TUserRole, TRoleClaim> : IUserRepository<TUser, TKey, TUserRole, TRoleClaim>
        where TUser : DapperIdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {

        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<UserRepository<TUser, TKey, TUserRole, TRoleClaim>> _log;
        private readonly IOptions<SqlConfiguration> _sqlConfiguration;
        private readonly IRoleRepository<DapperIdentityRole<TKey>, TKey, TUserRole, TRoleClaim> _roleRepository;
        public UserRepository(IConnectionProvider connProv, ILogger<UserRepository<TUser, TKey, TUserRole, TRoleClaim>> log,
                              IOptions<SqlConfiguration> sqlConf,
                              IRoleRepository<DapperIdentityRole<TKey>, TKey, TUserRole, TRoleClaim> roleRepo)
        {
            _connectionProvider = connProv;
            _log = log;
            _sqlConfiguration = sqlConf;
            _roleRepository = roleRepo;
        }

        public Task<IEnumerable<TUser>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<TUser> GetByEmail(string email)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Email", email);

                    var query = _sqlConfiguration.Value.SelectUserByEmailQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] { "%EMAIL%" },
                                                                               new string[] { "Email" });
                    return await conn.QuerySingleAsync<TUser>(sql: query,
                                                              param: dynamicParameters);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(1), ex.Message, ex);

                return null;
            }
        }

        public async Task<TUser> GetById(TKey id)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _sqlConfiguration.Value.SelectUserByIdQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] { "%ID%" },
                                                                               new string[] { "Id" });
                    return await conn.QuerySingleAsync<TUser>(sql: query,
                                                              param: dynamicParameters);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(2), ex.Message, ex);

                return null;
            }
        }

        public async Task<TUser> GetByUserName(string userName)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("User", userName);

                    var query = _sqlConfiguration.Value.SelectUserByUserNameQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] { "%USERNAME%" },
                                                                               new string[] { "User" });
                    return await conn.QuerySingleAsync<TUser>(sql: query,
                                                              param: dynamicParameters);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(3), ex.Message, ex);

                return null;
            }
        }

        public async Task<bool> Insert(TUser user, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var columnsBuilder = new StringBuilder();
                        var dynamicParameters = new DynamicParameters(user);

                        var userProperties = user.GetType()
                                                 .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                && !y.Name.Equals("Id"));

                        var valuesArray = new List<string>(userProperties.Count());

                        if (!user.Id.Equals(default(TKey)))
                        {
                            columnsBuilder.Append("Id, ");
                            valuesArray.Add($"{_sqlConfiguration.Value.ParameterNotation}Id");
                        }

                        columnsBuilder.Append(string.Join(",", userProperties));

                        valuesArray = valuesArray.InsertQueryValuesFragment(_sqlConfiguration.Value.ParameterNotation, userProperties);

                        var query = _sqlConfiguration.Value.InsertUserQuery.ReplaceInsertQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                                         _sqlConfiguration.Value.UserTable,
                                                                                                         columnsBuilder.ToString(),
                                                                                                         string.Join(", ", valuesArray));

                        var result = await x.ExecuteAsync(query, dynamicParameters, transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(4), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(4), ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> InsertClaims(TKey id, IEnumerable<Claim> claims, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var valuesArray = new string[] {
                                                         $"{_sqlConfiguration.Value.ParameterNotation}UserId",
                                                         $"{_sqlConfiguration.Value.ParameterNotation}ClaimType",
                                                         $"{_sqlConfiguration.Value.ParameterNotation}ClaimValue"
                                                       };

                        var query = _sqlConfiguration.Value.InsertUserClaimQuery
                                                           .ReplaceInsertQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                         _sqlConfiguration.Value.UserClaimTable,
                                                                                         "UserId, ClaimType, ClaimValue",
                                                                                         string.Join(", ", valuesArray));

                        var resultList = new List<bool>(claims.Count());
                        foreach (var claim in claims)
                        {
                            resultList.Add(await x.ExecuteAsync(query,
                                                                new
                                                                {
                                                                    UserId = id,
                                                                    ClaimType = claim.Type,
                                                                    ClaimValue = claim.Value
                                                                }, transaction) > 0);
                        }

                        return resultList.TrueForAll(y => y);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(5), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(5), ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> InsertLoginInfo(TKey id, UserLoginInfo loginInfo, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var valuesArray = new string[] {
                                                         $"{_sqlConfiguration.Value.ParameterNotation}UserId",
                                                         $"{_sqlConfiguration.Value.ParameterNotation}LoginProvider",
                                                         $"{_sqlConfiguration.Value.ParameterNotation}ProviderKey"
                                                       };

                        var query = _sqlConfiguration.Value.InsertUserLoginQuery
                                                           .ReplaceInsertQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                         _sqlConfiguration.Value.UserLoginTable,
                                                                                         "UserId, LoginProvider, ProviderKey",
                                                                                         string.Join(", ", valuesArray));

                        var result = await x.ExecuteAsync(query, new
                        {
                            UserId = id,
                            LoginProvider = loginInfo.LoginProvider,
                            ProviderKey = loginInfo.ProviderKey
                        }, transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(6), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(6), ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> AddToRole(TKey id, string roleName, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var role = await _roleRepository.GetByName(roleName);
                        if (role == null)
                            return false;

                        var valuesArray = new string[] {
                                                         $"{_sqlConfiguration.Value.ParameterNotation}UserId",
                                                         $"{_sqlConfiguration.Value.ParameterNotation}RoleId"
                                                       };

                        var query = _sqlConfiguration.Value.InsertUserRoleQuery
                                                           .ReplaceInsertQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                         _sqlConfiguration.Value.UserRoleTable,
                                                                                         "UserId, RoleId",
                                                                                         string.Join(", ", valuesArray));

                        var result = await x.ExecuteAsync(query, new
                        {
                            UserId = id,
                            RoleId = role.Id
                        }, transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(7), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(7), ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> Remove(TKey id, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        await x.OpenAsync(cancellationToken);

                        var dynamicParameters = new DynamicParameters();
                        dynamicParameters.Add("Id", id);

                        var query = _sqlConfiguration.Value.DeleteUserQuery
                                                           .ReplaceDeleteQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                         _sqlConfiguration.Value.UserTable,
                                                                                         $"{_sqlConfiguration.Value.ParameterNotation}Id");

                        var result = await x.ExecuteAsync(query, dynamicParameters, transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(8), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(8), ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> Update(TUser user, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var updateFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var dynamicParameters = new DynamicParameters(user);

                        var roleProperties = user.GetType()
                                                 .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                && !y.Name.Equals("Id"));

                        var setFragment = roleProperties.UpdateQuerySetFragment(_sqlConfiguration.Value.ParameterNotation);

                        var query = _sqlConfiguration.Value.UpdateUserQuery
                                                           .ReplaceUpdateQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                         _sqlConfiguration.Value.UserTable,
                                                                                         setFragment,
                                                                                         $"{_sqlConfiguration.Value.ParameterNotation}Id");

                        var result = await x.ExecuteAsync(query, dynamicParameters, transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(9), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);
                        return await updateFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await updateFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(9), ex.Message, ex);
                return false;
            }
        }

        public async Task<TUser> GetByUserLogin(string loginProvider, string providerKey)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var defaultUser = default(TUser);
                    var userProperties = defaultUser.GetType()
                                                    .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                   && !y.Name.Equals("Id"));

                    var query = _sqlConfiguration.Value.GetUserLoginByLoginProviderAndProviderKey
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] {
                                                                                                "%LOGINPROVIDER%",
                                                                                                "%PROVIDERKEY%"
                                                                                            },
                                                                               new string[] {
                                                                                                "LoginProvider",
                                                                                                "ProviderKey"
                                                                                            },
                                                                               new string[] {
                                                                                                "%USERFILTER%",
                                                                                                "%USERTABLE%",
                                                                                                "%USERLOGINTABLE%",
                                                                                            },
                                                                               new string[] {
                                                                                                userProperties.SelectFilterWithTableName(_sqlConfiguration.Value.UserTable),
                                                                                                _sqlConfiguration.Value.UserTable,
                                                                                                _sqlConfiguration.Value.UserLoginTable,
                                                                                            });
                    return await conn.QuerySingleAsync<TUser>(sql: query,
                                                              param: new
                                                              {
                                                                  LoginProvider = loginProvider,
                                                                  ProviderKey = providerKey
                                                              });
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(10), ex.Message, ex);

                return null;
            }
        }

        public async Task<IList<Claim>> GetClaimsByUserId(TKey id)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var query = _sqlConfiguration.Value.GetClaimsByUserIdQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserClaimTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] { "%ID%" },
                                                                               new string[] { "UserId" });

                    var result = await conn.QueryAsync(query, new { UserId = id });
                    return result?.Select(x => new Claim(x.ClaimType, x.ClaimValue))
                                  .ToList();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(11), ex.Message, ex);

                return null;
            }
        }

        public async Task<IList<string>> GetRolesByUserId(TKey id)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var query = _sqlConfiguration.Value.GetRolesByUserIdQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserRoleTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] {
                                                                                                "%ID%"
                                                                                            },
                                                                               new string[] {
                                                                                                "UserId"
                                                                                            },
                                                                               new string[] {
                                                                                                "%ROLETABLE%",
                                                                                                "%USERROLETABLE%"
                                                                                            },
                                                                               new string[] {
                                                                                                _sqlConfiguration.Value.RoleTable,
                                                                                                _sqlConfiguration.Value.UserRoleTable
                                                                                            });

                    var result = await conn.QueryAsync<string>(query, new { UserId = id });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(12), ex.Message, ex);

                return null;
            }
        }

        public async Task<IList<UserLoginInfo>> GetUserLoginInfoById(TKey id)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var query = _sqlConfiguration.Value.GetUserLoginInfoByIdQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserLoginTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] { "%ID%" },
                                                                               new string[] { "UserId" });

                    var result = await conn.QueryAsync(query, new { UserId = id });
                    return result?.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.Name))
                                  .ToList();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(13), ex.Message, ex);

                return null;
            }
        }

        public async Task<IList<TUser>> GetUsersByClaim(Claim claim)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var defaultUser = default(TUser);
                    var userProperties = defaultUser.GetType()
                                                    .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                   && !y.Name.Equals("Id"));

                    var query = _sqlConfiguration.Value.GetUsersByClaimQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] {
                                                                                                "%CLAIMVALUE%",
                                                                                                "%CLAIMTYPE%"
                                                                                            },
                                                                               new string[] {
                                                                                                "ClaimValue",
                                                                                                "ClaimType"
                                                                                            },
                                                                               new string[] {
                                                                                                "%USERFILTER%",
                                                                                                "%USERTABLE%",
                                                                                                "%USERCLAIMTABLE%",
                                                                                            },
                                                                               new string[] {
                                                                                                userProperties.SelectFilterWithTableName(_sqlConfiguration.Value.UserTable),
                                                                                                _sqlConfiguration.Value.UserTable,
                                                                                                _sqlConfiguration.Value.UserClaimTable,
                                                                                            });
                    var result = await conn.QueryAsync<TUser>(sql: query,
                                                              param: new
                                                              {
                                                                  ClaimValue = claim.Value,
                                                                  ClaimType = claim.Type
                                                              });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(14), ex.Message, ex);

                return null;
            }
        }

        public async Task<IList<TUser>> GetUsersInRole(string roleName)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var defaultUser = default(TUser);
                    var userProperties = defaultUser.GetType()
                                                    .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                   && !y.Name.Equals("Id"));

                    var query = _sqlConfiguration.Value.GetUsersInRoleQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] {
                                                                                                "%ROLENAME%"
                                                                                            },
                                                                               new string[] {
                                                                                                "RoleName"
                                                                                            },
                                                                               new string[] {
                                                                                                "%USERFILTER%",
                                                                                                "%USERTABLE%",
                                                                                                "%USERROLETABLE%",
                                                                                                "%ROLETABLE%"
                                                                                            },
                                                                               new string[] {
                                                                                                userProperties.SelectFilterWithTableName(_sqlConfiguration.Value.UserTable),
                                                                                                _sqlConfiguration.Value.UserTable,
                                                                                                _sqlConfiguration.Value.UserRoleTable,
                                                                                                _sqlConfiguration.Value.RoleTable
                                                                                            });
                    var result = await conn.QueryAsync<TUser>(sql: query,
                                                              param: new
                                                              {
                                                                  RoleName = roleName
                                                              });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(15), ex.Message, ex);

                return null;
            }
        }

        public async Task<bool> IsInRole(TKey id, string roleName)
        {
            try
            {
                using (var conn = _connectionProvider.Create())
                {
                    await conn.OpenAsync();

                    var defaultUser = default(TUser);
                    var userProperties = defaultUser.GetType()
                                                    .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                   && !y.Name.Equals("Id"));

                    var query = _sqlConfiguration.Value.IsInRoleQuery
                                                       .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                               _sqlConfiguration.Value.UserTable,
                                                                               _sqlConfiguration.Value.ParameterNotation,
                                                                               new string[] {
                                                                                                "%ROLENAME%",
                                                                                                "%USERID%"
                                                                                            },
                                                                               new string[] {
                                                                                                "RoleName",
                                                                                                "UserId"
                                                                                            },
                                                                               new string[] {
                                                                                                "%USERTABLE%",
                                                                                                "%USERROLETABLE%",
                                                                                                "%ROLETABLE%"
                                                                                            },
                                                                               new string[] {
                                                                                                _sqlConfiguration.Value.UserTable,
                                                                                                _sqlConfiguration.Value.UserRoleTable,
                                                                                                _sqlConfiguration.Value.RoleTable
                                                                                            });
                    var result = await conn.QueryAsync(sql: query,
                                                       param: new
                                                       {
                                                           RoleName = roleName,
                                                           UserId = id
                                                       });

                    return result.Count() > 0;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(16), ex.Message, ex);

                return false;
            }
        }

        public async Task<bool> RemoveClaims(TKey id, IEnumerable<Claim> claims, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        await x.OpenAsync(cancellationToken);

                        var query = _sqlConfiguration.Value.RemoveClaimsQuery
                                                           .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                   _sqlConfiguration.Value.UserClaimTable,
                                                                                   _sqlConfiguration.Value.ParameterNotation,
                                                                                   new string[] {
                                                                                                    "%ID%",
                                                                                                    "%CLAIMVALUE%",
                                                                                                    "%CLAIMTYPE"
                                                                                                },
                                                                                   new string[] {
                                                                                                    "Id",
                                                                                                    "ClaimValue",
                                                                                                    "ClaimType"
                                                                                                });

                        var resultList = new List<bool>(claims.Count());
                        foreach (var claim in claims)
                        {
                            resultList.Add(await x.ExecuteAsync(query, new
                            {
                                Id = id,
                                ClaimValue = claim.Value,
                                ClaimType = claim.Type
                            }, transaction) > 0);
                        }

                        return resultList.TrueForAll(y => y);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(17), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(17), ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> RemoveFromRole(TKey id, string roleName, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        await x.OpenAsync(cancellationToken);

                        var query = _sqlConfiguration.Value.RemoveUserFromRoleQuery
                                                           .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                   _sqlConfiguration.Value.UserClaimTable,
                                                                                   _sqlConfiguration.Value.ParameterNotation,
                                                                                   new string[] {
                                                                                                    "%ID%",
                                                                                                    "%ROLENAME%"
                                                                                                },
                                                                                   new string[] {
                                                                                                    "Id",
                                                                                                    "RoleName"
                                                                                                },
                                                                                   new string[] {
                                                                                                    "%USERROLETABLE",
                                                                                                    "%ROLETABLE%"
                                                                                                },
                                                                                   new string[] {
                                                                                                    _sqlConfiguration.Value.UserRoleTable,
                                                                                                    _sqlConfiguration.Value.RoleTable
                                                                                                });

                        var result = await x.ExecuteAsync(query, new
                        {
                            Id = id,
                            RoleName = roleName
                        }, transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(18), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(18), ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> RemoveLogin(TKey id, string loginProvider, string providerKey, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        await x.OpenAsync(cancellationToken);

                        var query = _sqlConfiguration.Value.RemoveLoginForUserQuery
                                                           .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                   _sqlConfiguration.Value.UserLoginTable,
                                                                                   _sqlConfiguration.Value.ParameterNotation,
                                                                                   new string[] {
                                                                                                    "%ID%",
                                                                                                    "%LOGINPROVIDER%",
                                                                                                    "%PROVIDERKEY%"
                                                                                                },
                                                                                   new string[] {
                                                                                                    "Id",
                                                                                                    "LoginProvider",
                                                                                                    "ProviderKey"
                                                                                                });

                        var result = await x.ExecuteAsync(query, new
                        {
                            Id = id,
                            LoginProvider = loginProvider,
                            ProviderKey = providerKey
                        }, transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(19), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(19), ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> UpdateClaim(TKey id, Claim oldClaim, Claim newClaim, CancellationToken cancellationToken, DbTransaction transaction = null)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        await x.OpenAsync(cancellationToken);

                        var query = _sqlConfiguration.Value.UpdateClaimForUserQuery
                                                           .ReplaceQueryParameters(_sqlConfiguration.Value.SchemaName,
                                                                                   _sqlConfiguration.Value.UserClaimTable,
                                                                                   _sqlConfiguration.Value.ParameterNotation,
                                                                                   new string[] {
                                                                                                    "%NEWCLAIMTYPE%",
                                                                                                    "%NEWCLAIMVALUE%",
                                                                                                    "%USERID%",
                                                                                                    "%CLAIMTYPE%",
                                                                                                    "%CLAIMVALUE%"
                                                                                                },
                                                                                   new string[] {
                                                                                                    "NewClaimType",
                                                                                                    "NewClaimValue",
                                                                                                    "UserId",
                                                                                                    "ClaimType",
                                                                                                    "ClaimValue"
                                                                                                });

                        var result = await x.ExecuteAsync(query, new
                        {
                            NewClaimType = newClaim.Type,
                            NewClaimValue = newClaim.Value,
                            UserId = id,
                            ClaimType = oldClaim.Type,
                            ClaimValue = oldClaim.Value
                        }, transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(20), ex.Message, ex);
                        return false;
                    }
                });

                DbConnection conn = null;
                if (transaction == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = transaction.Connection;
                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(20), ex.Message, ex);
                return false;
            }
        }
    }
}
