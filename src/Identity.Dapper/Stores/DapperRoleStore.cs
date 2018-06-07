using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Models;
using Identity.Dapper.Repositories.Contracts;
using Identity.Dapper.UnitOfWork.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Dapper.Stores
{
    public class DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>
        : IRoleStore<TRole>, IRoleClaimStore<TRole>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        private DbConnection _connection;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>> _log;
        private readonly IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> _roleRepository;
        private readonly DapperIdentityOptions _dapperIdentityOptions;
        public DapperRoleStore(IConnectionProvider connProv,
                               ILogger<DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>> log,
                               IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> roleRepo,
                               IUnitOfWork uow,
                               DapperIdentityOptions dapperIdOpts)
        {
            _roleRepository = roleRepo;
            _log = log;
            _connectionProvider = connProv;
            _unitOfWork = uow;
            _dapperIdentityOptions = dapperIdOpts;
        }

        private async Task CreateTransactionIfNotExistsAsync(CancellationToken cancellationToken)
        {
            if (!_dapperIdentityOptions.UseTransactionalBehavior)
            {
                _connection = _connectionProvider.Create();
                await _connection.OpenAsync(cancellationToken);
            }
            else
            {
                _connection = _unitOfWork.CreateOrGetConnection();

                if (_connection.State == System.Data.ConnectionState.Closed)
                    await _connection.OpenAsync(cancellationToken);
            }
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            !_dapperIdentityOptions.UseTransactionalBehavior
                        ? Task.CompletedTask
                        : CommitTransactionAsync(cancellationToken);

        private Task CommitTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken != default(CancellationToken))
                cancellationToken.ThrowIfCancellationRequested();

            if (_dapperIdentityOptions.UseTransactionalBehavior)
            {
                try
                {
                    _unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    _log.LogError(ex.Message, ex);

                    _unitOfWork.DiscardChanges();
                }
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_dapperIdentityOptions.UseTransactionalBehavior)
                _unitOfWork?.Dispose();
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.InsertAsync(role, cancellationToken);

                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError{ Description = ex.Message }
                });
            }
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.RemoveAsync(role.Id, cancellationToken);

                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError{ Description = ex.Message }
                });
            }
        }

        public virtual TKey ConvertIdFromString(string id)
        {
            if (id == null)
                return default(TKey);

            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentNullException(nameof(roleId));

            try
            {
                var result = await _roleRepository.GetByIdAsync(ConvertIdFromString(roleId));

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            try
            {
                var result = await _roleRepository.GetByNameAsync(normalizedRoleName);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return role.Name;
        }

        public async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (role.Id.Equals(default(TKey)))
                return null;

            return role.Id.ToString();
        }

        public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return role.Name;
        }

        public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = normalizedName;
        }

        public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = roleName;
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.UpdateAsync(role, cancellationToken);

                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError{ Description = ex.Message }
                });
            }
        }

        public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.GetClaimsByRole(role, cancellationToken);

                return result?.Select(roleClaim => new Claim(roleClaim.ClaimType, roleClaim.ClaimValue))
                              .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.InsertClaimAsync(role, claim, cancellationToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExistsAsync(cancellationToken);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.RemoveClaimAsync(role, claim, cancellationToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }
    }
}
