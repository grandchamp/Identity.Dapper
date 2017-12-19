using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Factories.Contracts;
using Identity.Dapper.Models;
using Identity.Dapper.Queries.User;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.UnitOfWork.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
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
        private readonly IQueryFactory _queryFactory;
        public UserRepository(IConnectionProvider connProv,
                              ILogger<UserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>> log,
                              SqlConfiguration sqlConf,
                              IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> roleRepo,
                              IUnitOfWork uow,
                              IQueryFactory queryFactory)
        {
            _connectionProvider = connProv;
            _log = log;
            _sqlConfiguration = sqlConf;
            _roleRepository = roleRepo;
            _unitOfWork = uow;
            _queryFactory = queryFactory;
        }

        public Task<IEnumerable<TUser>> GetAllAsync() => throw new NotImplementedException();

        public async Task<TUser> GetByEmailAsync(string email)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TUser>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Email", email);

                    var query = _queryFactory.GetQuery<SelectUserByEmailQuery>();

                    var userDictionary = new Dictionary<TKey, TUser>();
                    var result = await x.QueryAsync<TUser, TUserRole, TUser>(sql: query,
                                                                             param: dynamicParameters,
                                                                             transaction: _unitOfWork.Transaction,
                                                                             map: UserRoleMapping(userDictionary),
                                                                             splitOn: "UserId");

                    if (userDictionary.Count > 0)
                        return userDictionary.FirstOrDefault().Value;

                    return result.FirstOrDefault();
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
                _log.LogError(ex, ex.Message);

                return null;
            }
        }

        public async Task<TUser> GetByIdAsync(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TUser>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _queryFactory.GetQuery<SelectUserByIdQuery>();

                    var userDictionary = new Dictionary<TKey, TUser>();
                    var result = await x.QueryAsync<TUser, TUserRole, TUser>(sql: query,
                                                                             param: dynamicParameters,
                                                                             transaction: _unitOfWork.Transaction,
                                                                             map: UserRoleMapping(userDictionary),
                                                                             splitOn: "UserId");

                    if (userDictionary.Count > 0)
                        return userDictionary.FirstOrDefault().Value;

                    return result.FirstOrDefault();
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
                _log.LogError(ex, ex.Message);

                return null;
            }
        }

        public async Task<TUser> GetByUserNameAsync(string userName)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TUser>>(async x =>
                {
                    try
                    {
                        var dynamicParameters = new DynamicParameters();
                        dynamicParameters.Add("UserName", userName);

                        var query = _queryFactory.GetQuery<SelectUserByUserNameQuery>();

                        var userDictionary = new Dictionary<TKey, TUser>();
                        var result = await x.QueryAsync(sql: query,
                                                        param: dynamicParameters,
                                                        transaction: _unitOfWork.Transaction,
                                                        map: UserRoleMapping(userDictionary),
                                                        splitOn: "UserId");

                        if (userDictionary.Count > 0)
                            return userDictionary.FirstOrDefault().Value;

                        return result.FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                return null;
            }
        }

        private static Func<TUser, TUserRole, TUser> UserRoleMapping(Dictionary<TKey, TUser> userDictionary)
        {
            return new Func<TUser, TUserRole, TUser>((user, role) =>
            {
                var dictionaryUser = default(TUser);

                if (role != null)
                {
                    if (userDictionary.TryGetValue(user.Id, out dictionaryUser))
                    {
                        dictionaryUser.Roles.Add(role);
                    }
                    else
                    {
                        user.Roles.Add(role);
                        userDictionary.Add(user.Id, user);

                        dictionaryUser = user;
                    }
                }
                else
                {
                    dictionaryUser = user;
                }

                return dictionaryUser;
            });
        }

        public async Task<TKey> InsertAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<TKey>>(async x =>
                {
                    try
                    {
                        var dynamicParameters = new DynamicParameters(user);

                        var query = _queryFactory.GetInsertQuery<InsertUserQuery, TUser>(user);

                        var result = await x.ExecuteScalarAsync<TKey>(query, dynamicParameters, _unitOfWork.Transaction);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> InsertClaimsAsync(TKey id, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var resultList = new List<bool>(claims.Count());
                        foreach (var claim in claims)
                        {
                            var userClaim = Activator.CreateInstance<TUserClaim>();
                            userClaim.UserId = id;
                            userClaim.ClaimType = claim.Type;
                            userClaim.ClaimValue = claim.Value;

                            var query = _queryFactory.GetInsertQuery<InsertUserClaimQuery, TUserClaim>(userClaim);

                            resultList.Add(await x.ExecuteAsync(query, userClaim, _unitOfWork.Transaction) > 0);
                        }

                        return resultList.TrueForAll(y => y);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> InsertLoginInfoAsync(TKey id, UserLoginInfo loginInfo, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        dynamic userLogin = new
                        {
                            UserId = id,
                            loginInfo.LoginProvider,
                            loginInfo.ProviderKey,
                            Name = loginInfo.ProviderDisplayName
                        };

                        var query = (string)_queryFactory.GetInsertQuery<InsertUserLoginQuery, dynamic>(userLogin);

                        var result = await x.ExecuteAsync(query, (object)userLogin, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> AddToRoleAsync(TKey id, string roleName, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var role = await _roleRepository.GetByNameAsync(roleName);
                        if (role == null)
                            return false;

                        var userRole = Activator.CreateInstance<TUserRole>();
                        userRole.RoleId = role.Id;
                        userRole.UserId = id;

                        var query = _queryFactory.GetInsertQuery<InsertUserRoleQuery, TUserRole>(userRole);

                        var result = await x.ExecuteAsync(query, userRole, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> RemoveAsync(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var dynamicParameters = new DynamicParameters();
                        dynamicParameters.Add("Id", id);

                        var query = _queryFactory.GetDeleteQuery<DeleteUserQuery>();

                        var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                var updateFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var dynamicParameters = new DynamicParameters(user);

                        var query = _queryFactory.GetUpdateQuery<UpdateUserQuery, TUser>(user);

                        var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<TUser> GetByUserLoginAsync(string loginProvider, string providerKey)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TUser>>(async x =>
                {
                    try
                    {
                        var defaultUser = Activator.CreateInstance<TUser>();

                        var query = _queryFactory.GetQuery<GetUserLoginByLoginProviderAndProviderKeyQuery, TUser>(defaultUser);

                        var userDictionary = new Dictionary<TKey, TUser>();
                        var result = await x.QueryAsync(sql: query,
                                                        param: new
                                                        {
                                                            LoginProvider = loginProvider,
                                                            ProviderKey = providerKey
                                                        },
                                                        transaction: _unitOfWork.Transaction,
                                                        map: UserRoleMapping(userDictionary),
                                                        splitOn: "UserId");

                        if (userDictionary.Count > 0)
                            return userDictionary.FirstOrDefault().Value;

                        return result.FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<IList<Claim>> GetClaimsByUserIdAsync(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<Claim>>>(async x =>
                {
                    var query = _queryFactory.GetQuery<GetClaimsByUserIdQuery>();

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<IList<string>> GetRolesByUserIdAsync(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<string>>>(async x =>
                {
                    var query = _queryFactory.GetQuery<GetRolesByUserIdQuery>();

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<IList<UserLoginInfo>> GetUserLoginInfoByIdAsync(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<UserLoginInfo>>>(async x =>
                {
                    var query = _queryFactory.GetQuery<GetUserLoginInfoByIdQuery>();

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<IList<TUser>> GetUsersByClaimAsync(Claim claim)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<TUser>>>(async x =>
                {
                    var defaultUser = Activator.CreateInstance<TUser>();
                    var query = _queryFactory.GetQuery<GetUsersByClaimQuery, TUser>(defaultUser);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<IList<TUser>>>(async x =>
                {
                    var defaultUser = Activator.CreateInstance<TUser>();

                    var query = _queryFactory.GetQuery<GetUsersInRoleQuery, TUser>(defaultUser);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> IsInRoleAsync(TKey id, string roleName)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var defaultUser = Activator.CreateInstance<TUser>();

                    var query = _queryFactory.GetQuery<IsInRoleQuery, TUser>(defaultUser);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> RemoveClaimsAsync(TKey id, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var query = _queryFactory.GetDeleteQuery<RemoveClaimsQuery>();

                        var resultList = new List<bool>(claims.Count());
                        foreach (var claim in claims)
                        {
                            resultList.Add(await x.ExecuteAsync(query, new
                            {
                                UserId = id,
                                ClaimValue = claim.Value,
                                ClaimType = claim.Type
                            }, _unitOfWork.Transaction) > 0);
                        }

                        return resultList.TrueForAll(y => y);
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> RemoveFromRoleAsync(TKey id, string roleName, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var query = _queryFactory.GetDeleteQuery<RemoveUserFromRoleQuery>();

                        var result = await x.ExecuteAsync(query, new
                        {
                            UserId = id,
                            RoleName = roleName
                        }, _unitOfWork.Transaction);

                        return result > 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> RemoveLoginAsync(TKey id, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        var query = _queryFactory.GetDeleteQuery<RemoveLoginForUserQuery>();

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
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> UpdateClaimAsync(TKey id, Claim oldClaim, Claim newClaim, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    try
                    {
                        //I don't need to make a new GetUpdateQuery that don't pass a TEntity object just for use
                        //on this method, so i'll just pass null because i don't use the TEntity object on this method
                        var query = _queryFactory.GetUpdateQuery<UpdateClaimForUserQuery, TUserClaim>(null);

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
                        _log.LogError(ex, ex.Message);

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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }
    }
}
