using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.UnitOfWork.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Dapper.Repositories
{
    public class UserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>
        : IUserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>
        where TUser : DapperIdentityUser<TKey, TUserClaim, TUserRole, TUserLogin>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
        where TUserClaim : DapperIdentityUserClaim<TKey>
        where TUserLogin : DapperIdentityUserLogin<TKey>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
    {

        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<UserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>> _log;
        private readonly SqlConfiguration _sqlConfiguration;
        private readonly IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UserRepository(IConnectionProvider connProv,
                              ILogger<UserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>> log,
                              SqlConfiguration sqlConf,
                              IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> roleRepo,
                              IUnitOfWork uow)
        {
            _connectionProvider = connProv;
            _log = log;
            _sqlConfiguration = sqlConf;
            _roleRepository = roleRepo;
            _unitOfWork = uow;
        }

        public Task<IEnumerable<TUser>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<TUser> GetByEmail(string email)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TUser>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Email", email);

                    var query = _sqlConfiguration.SelectUserByEmailQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.UserTable,
                                                                         _sqlConfiguration.ParameterNotation,
                                                                         new string[] { "%EMAIL%" },
                                                                         new string[] { "Email" });

                    return await x.QueryFirstOrDefaultAsync<TUser>(sql: query,
                                                                   param: dynamicParameters,
                                                                   transaction: _unitOfWork.Transaction);
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
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
                var selectFunction = new Func<DbConnection, Task<TUser>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _sqlConfiguration.SelectUserByIdQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.UserTable,
                                                                         _sqlConfiguration.ParameterNotation,
                                                                         new string[] { "%ID%" },
                                                                         new string[] { "Id" });

                    return await x.QueryFirstOrDefaultAsync<TUser>(sql: query,
                                                                   param: dynamicParameters,
                                                                   transaction: _unitOfWork.Transaction);
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
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
                var selectFunction = new Func<DbConnection, Task<TUser>>(async x =>
                {
                    try
                    {
                        var dynamicParameters = new DynamicParameters();
                        dynamicParameters.Add("User", userName);

                        var query = _sqlConfiguration.SelectUserByUserNameQuery
                                                     .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                             _sqlConfiguration.UserTable,
                                                                             _sqlConfiguration.ParameterNotation,
                                                                             new string[] { "%USERNAME%" },
                                                                             new string[] { "User" });

                        return await x.QuerySingleOrDefaultAsync<TUser>(sql: query,
                                                                        param: dynamicParameters,
                                                                       transaction: _unitOfWork.Transaction);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex.Message, ex);

                        return null;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(3), ex.Message, ex);

                return null;
            }
        }

        public async Task<TKey> Insert(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<TKey>>(async x =>
                {
                    try
                    {
                        var columnsBuilder = new StringBuilder();
                        var dynamicParameters = new DynamicParameters(user);

                        var userProperties = user.GetType()
                                                 .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                && !y.Name.Equals("Id"));

                        if (_sqlConfiguration.UseQuotationMarks)
                            userProperties = userProperties.Select(y => string.Concat("\"", y, "\""));
                        else
                            userProperties = userProperties.Select(y => string.Concat(_sqlConfiguration.TableFieldNotation, y, _sqlConfiguration.TableFieldNotation));

                        var valuesArray = new List<string>(userProperties.Count());

                        if (!user.Id.Equals(default(TKey)))
                        {
                            columnsBuilder.Append("\"Id\", ");
                            valuesArray.Add($"{_sqlConfiguration.ParameterNotation}Id");
                        }

                        columnsBuilder.Append(string.Join(",", userProperties));

                        valuesArray = valuesArray.InsertQueryValuesFragment(_sqlConfiguration.ParameterNotation, userProperties);

                        var query = _sqlConfiguration.InsertUserQuery
                                                     .ReplaceInsertQueryParameters(_sqlConfiguration.SchemaName,
                                                                                   _sqlConfiguration.UserTable,
                                                                                   columnsBuilder.ToString(),
                                                                                   string.Join(", ", valuesArray));

                        var result = await x.ExecuteScalarAsync<TKey>(query, dynamicParameters, _unitOfWork.Transaction);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(4), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(4), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> InsertClaims(TKey id, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var valuesArray = new string[] {
                                                         $"{_sqlConfiguration.ParameterNotation}UserId",
                                                         $"{_sqlConfiguration.ParameterNotation}ClaimType",
                                                         $"{_sqlConfiguration.ParameterNotation}ClaimValue"
                                                       };

                        var query = _sqlConfiguration.InsertUserClaimQuery
                                                     .ReplaceInsertQueryParameters(_sqlConfiguration.SchemaName,
                                                                                   _sqlConfiguration.UserClaimTable,
                                                                                   $"{_sqlConfiguration.TableFieldNotation}UserId{_sqlConfiguration.TableFieldNotation}, " +
                                                                                   $"{_sqlConfiguration.TableFieldNotation}ClaimType{_sqlConfiguration.TableFieldNotation}, " +
                                                                                   $"{_sqlConfiguration.TableFieldNotation}ClaimValue{_sqlConfiguration.TableFieldNotation}",
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
                                                                },
                                                                _unitOfWork.Transaction) > 0);
                        }

                        return resultList.TrueForAll(y => y);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(5), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();

                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(5), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> InsertLoginInfo(TKey id, UserLoginInfo loginInfo, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var valuesArray = new string[] {
                                                         $"{_sqlConfiguration.ParameterNotation}UserId",
                                                         $"{_sqlConfiguration.ParameterNotation}LoginProvider",
                                                         $"{_sqlConfiguration.ParameterNotation}ProviderKey",
                                                         $"{_sqlConfiguration.ParameterNotation}Name"
                                                       };

                        var query = _sqlConfiguration.InsertUserLoginQuery
                                                     .ReplaceInsertQueryParameters(_sqlConfiguration.SchemaName,
                                                                                   _sqlConfiguration.UserLoginTable,
                                                                                   $"{_sqlConfiguration.TableFieldNotation}UserId{_sqlConfiguration.TableFieldNotation}, " +
                                                                                   $"{_sqlConfiguration.TableFieldNotation}LoginProvider{_sqlConfiguration.TableFieldNotation}, " +
                                                                                   $"{_sqlConfiguration.TableFieldNotation}ProviderKey{_sqlConfiguration.TableFieldNotation}, " +
                                                                                   $"{_sqlConfiguration.TableFieldNotation}Name{_sqlConfiguration.TableFieldNotation}",
                                                                                   string.Join(", ", valuesArray));

                        var result = await x.ExecuteAsync(query, new
                        {
                            UserId = id,
                            LoginProvider = loginInfo.LoginProvider,
                            ProviderKey = loginInfo.ProviderKey,
                            Name = loginInfo.ProviderDisplayName
                        }, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(6), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();

                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(6), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> AddToRole(TKey id, string roleName, CancellationToken cancellationToken)
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
                                                         $"{_sqlConfiguration.ParameterNotation}UserId",
                                                         $"{_sqlConfiguration.ParameterNotation}RoleId"
                                                       };

                        var query = _sqlConfiguration.InsertUserRoleQuery
                                                     .ReplaceInsertQueryParameters(_sqlConfiguration.SchemaName,
                                                                                   _sqlConfiguration.UserRoleTable,
                                                                                   $"{_sqlConfiguration.TableFieldNotation}UserId{_sqlConfiguration.TableFieldNotation}, " +
                                                                                   $"{_sqlConfiguration.TableFieldNotation}RoleId{_sqlConfiguration.TableFieldNotation}",
                                                                                   string.Join(", ", valuesArray));

                        var result = await x.ExecuteAsync(query, new
                        {
                            UserId = id,
                            RoleId = role.Id
                        }, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(7), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await insertFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await insertFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(7), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> Remove(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var dynamicParameters = new DynamicParameters();
                        dynamicParameters.Add("Id", id);

                        var query = _sqlConfiguration.DeleteUserQuery
                                                     .ReplaceDeleteQueryParameters(_sqlConfiguration.SchemaName,
                                                                                   _sqlConfiguration.UserTable,
                                                                                   $"{_sqlConfiguration.ParameterNotation}Id");

                        var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(8), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();

                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(8), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> Update(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                var updateFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var dynamicParameters = new DynamicParameters(user);

                        var userProperties = user.GetType()
                                                 .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                && !y.Name.Equals("Id"));

                        if (_sqlConfiguration.UseQuotationMarks)
                            userProperties = userProperties.Select(y => string.Concat("\"", y, "\""));
                        else
                            userProperties = userProperties.Select(y => string.Concat(_sqlConfiguration.TableFieldNotation, y, _sqlConfiguration.TableFieldNotation));

                        var setFragment = userProperties.UpdateQuerySetFragment(_sqlConfiguration.ParameterNotation);

                        var query = _sqlConfiguration.UpdateUserQuery
                                                     .ReplaceUpdateQueryParameters(_sqlConfiguration.SchemaName,
                                                                                   _sqlConfiguration.UserTable,
                                                                                   setFragment,
                                                                                   $"{_sqlConfiguration.ParameterNotation}Id");

                        var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(9), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);
                        return await updateFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();

                    return await updateFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(9), ex.Message, ex);

                throw;
            }
        }

        public async Task<TUser> GetByUserLogin(string loginProvider, string providerKey)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TUser>>(async x =>
                {
                    try
                    {
                        var defaultUser = Activator.CreateInstance<TUser>();
                        var userProperties = defaultUser.GetType()
                                                        .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                       && !y.Name.Equals("Id"));

                        if (_sqlConfiguration.UseQuotationMarks)
                            userProperties = userProperties.Select(y => string.Concat("\"", y, "\""));

                        var query = _sqlConfiguration.GetUserLoginByLoginProviderAndProviderKeyQuery
                                                     .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                             string.Empty,
                                                                             _sqlConfiguration.ParameterNotation,
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
                                                                                        userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable,_sqlConfiguration.UseQuotationMarks, _sqlConfiguration.TableFieldNotation),
                                                                                        _sqlConfiguration.UserTable,
                                                                                        _sqlConfiguration.UserLoginTable,
                                                                                          }
                                                                             );

                        return await x.QueryFirstOrDefaultAsync<TUser>(sql: query,
                                                                       param: new
                                                                       {
                                                                           LoginProvider = loginProvider,
                                                                           ProviderKey = providerKey
                                                                       },
                                                                       transaction: _unitOfWork.Transaction);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(12), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();

                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(10), ex.Message, ex);

                throw;
            }
        }

        public async Task<IList<Claim>> GetClaimsByUserId(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<Claim>>>(async x =>
                {
                    var query = _sqlConfiguration.GetClaimsByUserIdQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.UserClaimTable,
                                                                         _sqlConfiguration.ParameterNotation,
                                                                         new string[] { "%ID%" },
                                                                         new string[] { "UserId" });

                    var result = await x.QueryAsync(query, new { UserId = id }, _unitOfWork.Transaction);
                    return result?.Select(y => new Claim(y.ClaimType, y.ClaimValue))
                                  .ToList();
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(11), ex.Message, ex);

                throw;
            }
        }

        public async Task<IList<string>> GetRolesByUserId(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<string>>>(async x =>
                {
                    var query = _sqlConfiguration.GetRolesByUserIdQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         string.Empty,
                                                                         _sqlConfiguration.ParameterNotation,
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
                                                                                        _sqlConfiguration.RoleTable,
                                                                                        _sqlConfiguration.UserRoleTable
                                                                                      }
                                                                        );

                    var result = await x.QueryAsync<string>(query, new { UserId = id }, _unitOfWork.Transaction);

                    return result.ToList();
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(12), ex.Message, ex);

                throw;
            }
        }

        public async Task<IList<UserLoginInfo>> GetUserLoginInfoById(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<UserLoginInfo>>>(async x =>
                {
                    var query = _sqlConfiguration.GetUserLoginInfoByIdQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.UserLoginTable,
                                                                         _sqlConfiguration.ParameterNotation,
                                                                         new string[] { "%ID%" },
                                                                         new string[] { "UserId" });

                    var result = await x.QueryAsync(query, new { UserId = id }, _unitOfWork.Transaction);

                    return result?.Select(y => new UserLoginInfo(y.LoginProvider, y.ProviderKey, y.Name))
                                  .ToList();
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(13), ex.Message, ex);

                throw;
            }
        }

        public async Task<IList<TUser>> GetUsersByClaim(Claim claim)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<TUser>>>(async x =>
                {
                    var defaultUser = Activator.CreateInstance<TUser>();
                    var userProperties = defaultUser.GetType()
                                                    .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                   && !y.Name.Equals("Id"));

                    if (_sqlConfiguration.UseQuotationMarks)
                        userProperties = userProperties.Select(y => string.Concat("\"", y, "\""));

                    var query = _sqlConfiguration.GetUsersByClaimQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.UserTable,
                                                                         _sqlConfiguration.ParameterNotation,
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
                                                                                        userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable,_sqlConfiguration.UseQuotationMarks, _sqlConfiguration.TableFieldNotation),
                                                                                        _sqlConfiguration.UserTable,
                                                                                        _sqlConfiguration.UserClaimTable,
                                                                                      }
                                                                         );

                    var result = await x.QueryAsync<TUser>(sql: query,
                                                           param: new
                                                           {
                                                               ClaimValue = claim.Value,
                                                               ClaimType = claim.Type
                                                           },
                                                           transaction: _unitOfWork.Transaction);

                    return result.ToList();
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(14), ex.Message, ex);

                throw;
            }
        }

        public async Task<IList<TUser>> GetUsersInRole(string roleName)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<TUser>>>(async x =>
                {
                    var defaultUser = Activator.CreateInstance<TUser>();
                    var userProperties = defaultUser.GetType()
                                                    .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                   && !y.Name.Equals("Id"));

                    if (_sqlConfiguration.UseQuotationMarks)
                        userProperties = userProperties.Select(y => string.Concat("\"", y, "\""));

                    var query = _sqlConfiguration.GetUsersInRoleQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         string.Empty,
                                                                         _sqlConfiguration.ParameterNotation,
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
                                                                                        userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable,_sqlConfiguration.UseQuotationMarks, _sqlConfiguration.TableFieldNotation),
                                                                                        _sqlConfiguration.UserTable,
                                                                                        _sqlConfiguration.UserRoleTable,
                                                                                        _sqlConfiguration.RoleTable
                                                                                      }
                                                                         );

                    var result = await x.QueryAsync<TUser>(sql: query,
                                                           param: new
                                                           {
                                                               RoleName = roleName
                                                           },
                                                           transaction: _unitOfWork.Transaction);

                    return result.ToList();
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(15), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> IsInRole(TKey id, string roleName)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var defaultUser = Activator.CreateInstance<TUser>();
                    var userProperties = defaultUser.GetType()
                                                    .GetPublicPropertiesNames(y => !y.Name.Equals("ConcurrencyStamp")
                                                                                   && !y.Name.Equals("Id"));

                    if (_sqlConfiguration.UseQuotationMarks)
                        userProperties = userProperties.Select(y => string.Concat("\"", y, "\""));

                    var query = _sqlConfiguration.IsInRoleQuery
                                                 .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                         _sqlConfiguration.UserTable,
                                                                         _sqlConfiguration.ParameterNotation,
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
                                                                                        _sqlConfiguration.UserTable,
                                                                                        _sqlConfiguration.UserRoleTable,
                                                                                        _sqlConfiguration.RoleTable
                                                                                      }
                                                                         );

                    var result = await x.QueryAsync(sql: query,
                                                    param: new
                                                    {
                                                        RoleName = roleName,
                                                        UserId = id
                                                    },
                                                    transaction: _unitOfWork.Transaction);

                    return result.Count() > 0;
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync();

                        return await selectFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await selectFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(16), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> RemoveClaims(TKey id, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var query = _sqlConfiguration.RemoveClaimsQuery
                                                     .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                             _sqlConfiguration.UserClaimTable,
                                                                             _sqlConfiguration.ParameterNotation,
                                                                             new string[] {
                                                                                            "%ID%",
                                                                                            "%CLAIMVALUE%",
                                                                                            "%CLAIMTYPE%"
                                                                                          },
                                                                             new string[] {
                                                                                            "Id",
                                                                                            "ClaimValue",
                                                                                            "ClaimType"
                                                                                          }
                                                                             );

                        var resultList = new List<bool>(claims.Count());
                        foreach (var claim in claims)
                        {
                            resultList.Add(await x.ExecuteAsync(query, new
                            {
                                Id = id,
                                ClaimValue = claim.Value,
                                ClaimType = claim.Type
                            }, _unitOfWork.Transaction) > 0);
                        }

                        return resultList.TrueForAll(y => y);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(17), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(17), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> RemoveFromRole(TKey id, string roleName, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var query = _sqlConfiguration.RemoveUserFromRoleQuery
                                                     .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                             _sqlConfiguration.UserRoleTable,
                                                                             _sqlConfiguration.ParameterNotation,
                                                                             new string[] {
                                                                                            "%USERID%",
                                                                                            "%ROLENAME%"
                                                                                          },
                                                                             new string[] {
                                                                                            "UserId",
                                                                                            "RoleName"
                                                                                          },
                                                                             new string[] {
                                                                                            "%USERROLETABLE%",
                                                                                            "%ROLETABLE%"
                                                                                          },
                                                                             new string[] {
                                                                                            _sqlConfiguration.UserRoleTable,
                                                                                            _sqlConfiguration.RoleTable
                                                                                          }
                                                                             );

                        var result = await x.ExecuteAsync(query, new
                        {
                            UserId = id,
                            RoleName = roleName
                        }, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(18), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();
                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(18), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> RemoveLogin(TKey id, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var query = _sqlConfiguration.RemoveLoginForUserQuery
                                                     .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                             _sqlConfiguration.UserLoginTable,
                                                                             _sqlConfiguration.ParameterNotation,
                                                                             new string[] {
                                                                                            "%USERID%",
                                                                                            "%LOGINPROVIDER%",
                                                                                            "%PROVIDERKEY%"
                                                                                          },
                                                                             new string[] {
                                                                                            "UserId",
                                                                                            "LoginProvider",
                                                                                            "ProviderKey"
                                                                                          }
                                                                             );

                        var result = await x.ExecuteAsync(query, new
                        {
                            UserId = id,
                            LoginProvider = loginProvider,
                            ProviderKey = providerKey
                        }, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(19), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();

                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(19), ex.Message, ex);

                throw;
            }
        }

        public async Task<bool> UpdateClaim(TKey id, Claim oldClaim, Claim newClaim, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var query = _sqlConfiguration.UpdateClaimForUserQuery
                                                     .ReplaceQueryParameters(_sqlConfiguration.SchemaName,
                                                                             _sqlConfiguration.UserClaimTable,
                                                                             _sqlConfiguration.ParameterNotation,
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
                                                                                          }
                                                                             );

                        var result = await x.ExecuteAsync(query, new
                        {
                            NewClaimType = newClaim.Type,
                            NewClaimValue = newClaim.Value,
                            UserId = id,
                            ClaimType = oldClaim.Type,
                            ClaimValue = oldClaim.Value
                        }, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(new EventId(20), ex.Message, ex);

                        throw;
                    }
                });

                DbConnection conn = null;
                if (_unitOfWork?.Connection == null)
                {
                    using (conn = _connectionProvider.Create())
                    {
                        await conn.OpenAsync(cancellationToken);

                        return await removeFunction(conn);
                    }
                }
                else
                {
                    conn = _unitOfWork.CreateOrGetConnection();

                    return await removeFunction(conn);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(new EventId(20), ex.Message, ex);

                throw;
            }
        }
    }
}
