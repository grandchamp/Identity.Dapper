using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Dapper.Stores
{
    public class DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>
        : IRoleStore<TRole>
        where TRole : DapperIdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        private readonly ILogger<DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>> _log;
        private readonly IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> _roleRepository;
        public DapperRoleStore(IConnectionProvider connProv,
                               ILogger<DapperRoleStore<TRole, TKey, TUserRole, TRoleClaim>> log, 
                               IRoleRepository<TRole, TKey, TUserRole, TRoleClaim> roleRepo)
        {
            _roleRepository = roleRepo;
            _log = log;
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.Insert(role, cancellationToken);

                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.Remove(role.Id, cancellationToken);

                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

        public void Dispose()
        {

        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentNullException(nameof(roleId));

            try
            {
                var result = await _roleRepository.GetById((TKey)Convert.ChangeType(roleId, typeof(TKey)));

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

            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            try
            {
                var result = await _roleRepository.GetByName(normalizedRoleName);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (role.Id.Equals(default(TKey)))
                return null;

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = roleName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleRepository.Update(role, cancellationToken);

                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }
    }
}
