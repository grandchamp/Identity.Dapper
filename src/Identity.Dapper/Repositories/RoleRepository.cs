using Dapper;
using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Factories.Contracts;
using Identity.Dapper.Models;
using Identity.Dapper.Queries;
using Identity.Dapper.Queries.Role;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.UnitOfWork.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Dapper.Repositories
{
    public class RoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        : IRoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> _log;
        private readonly SqlConfiguration _sqlConfiguration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryFactory _queryFactory;
        public RoleRepository(IConnectionProvider connectionProvider, ILogger<RoleRepository<TRole, TKey, TUserRole, TRoleClaim>> log, SqlConfiguration sqlConfiguration, IUnitOfWork unitOfWork, IQueryFactory queryFactory)
        {
            _connectionProvider = connectionProvider;
            _log = log;
            _sqlConfiguration = sqlConfiguration;
            _unitOfWork = unitOfWork;
            _queryFactory = queryFactory;
        }

        public async Task<TRole> GetById(TKey id)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TRole>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _queryFactory.GetQuery<SelectRoleByIdQuery>();

                    return await x.QueryFirstOrDefaultAsync<TRole>(sql: query,
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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<TRole> GetByName(string roleName)
        {
            try
            {
                var selectFunction = new Func<DbConnection, Task<TRole>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Name", roleName);

                    var query = _queryFactory.GetQuery<SelectRoleByNameQuery>();

                    return await x.QueryFirstOrDefaultAsync<TRole>(sql: query,
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
                _log.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task<bool> Insert(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                var insertFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters(role);

                    var query = _queryFactory.GetInsertQuery<InsertRoleQuery, TRole>(role);

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                    return result > 0;
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

        public async Task<bool> Remove(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var removeFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("Id", id);

                    var query = _queryFactory.GetDeleteQuery<DeleteRoleQuery>();

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                    return result > 0;
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

        public async Task<bool> Update(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                var updateFunction = new Func<DbConnection, Task<bool>>(async x =>
                {
                    var dynamicParameters = new DynamicParameters(role);

                    var query = _queryFactory.GetUpdateQuery<UpdateRoleQuery, TRole>(role);

                    var result = await x.ExecuteAsync(query, dynamicParameters, _unitOfWork.Transaction);

                    return result > 0;
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
    }
}
