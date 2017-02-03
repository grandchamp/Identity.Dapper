using Identity.Dapper.Connections;
using Identity.Dapper.Entities;
using Identity.Dapper.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Dapper.Stores
{
    public class DapperUserStore<TUser, TKey, TUserRole, TRoleClaim> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>
        where TUser : DapperIdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        private DbTransaction _transaction;
        private DbConnection _connection;

        private readonly IConnectionProvider _connectionProvider;
        private readonly ILogger<DapperUserStore<TUser, TKey, TUserRole, TRoleClaim>> _log;
        private readonly IUserRepository<TUser, TKey, TUserRole, TRoleClaim> _userRepository;
        public DapperUserStore(IConnectionProvider connProv,
                               ILogger<DapperUserStore<TUser, TKey, TUserRole, TRoleClaim>> log,
                               IUserRepository<TUser, TKey, TUserRole, TRoleClaim> roleRepo)
        {
            _userRepository = roleRepo;
            _connectionProvider = connProv;
            _log = log;
        }

        private async Task CreateTransactionIfNotExists(CancellationToken cancellationToken)
        {
            if (_connection == null)
            {
                _connection = _connectionProvider.Create();
                await _connection.OpenAsync(cancellationToken);
            }
            else
            {
                if (_connection.State == System.Data.ConnectionState.Closed)
                    await _connection.OpenAsync(cancellationToken);
            }

            if (_transaction == null)
                _transaction = _connection.BeginTransaction();
        }

        private void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction = null;
            }
        }

        private void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }
        }

        public IQueryable<TUser> Users
        {
            get
            {
                //Impossible to implement IQueryable with Dapper
                throw new NotImplementedException();
            }
        }

        public async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
           await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.InsertClaims(user.Id, claims, cancellationToken, _transaction);
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.InsertLoginInfo(user.Id, login, cancellationToken, _transaction);
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.AddToRole(user.Id, roleName, cancellationToken, _transaction);
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.Insert(user, cancellationToken, _transaction);

                if (!result.Equals(default(TKey)))
                {
                    CommitTransaction();
                    user.Id = result;

                    return IdentityResult.Success;
                }
                else
                {
                    RollbackTransaction();

                    return IdentityResult.Failed();
                }
            }
            catch (Exception ex)
            {
                RollbackTransaction();

                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.Remove(user.Id, cancellationToken, _transaction);

                if (result)
                {
                    CommitTransaction();
                    return IdentityResult.Success;
                }
                else
                {
                    RollbackTransaction();

                    return IdentityResult.Failed();
                }
            }
            catch (Exception ex)
            {
                RollbackTransaction();

                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

        public void Dispose()
        {
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedEmail))
                throw new ArgumentNullException(nameof(normalizedEmail));

            try
            {
                var result = await _userRepository.GetByEmail(normalizedEmail);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            try
            {
                var result = await _userRepository.GetById((TKey)Convert.ChangeType(userId, typeof(TKey)));

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = await _userRepository.GetByUserLogin(loginProvider, providerKey);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedUserName))
                throw new ArgumentNullException(nameof(normalizedUserName));

            try
            {
                var result = await _userRepository.GetByUserName(normalizedUserName);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.GetClaimsByUserId(user.Id);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnd);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.GetUserLoginInfoById(user.Id);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.GetRolesByUserId(user.Id);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            try
            {
                var result = await _userRepository.GetUsersByClaim(claim);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            try
            {
                var result = await _userRepository.GetUsersInRole(roleName);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            try
            {
                var result = await _userRepository.IsInRole(user.Id, roleName);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return false;
            }
        }

        public async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            try
            {
                var result = await _userRepository.RemoveClaims(user.Id, claims, cancellationToken, _transaction);
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            try
            {
                var result = await _userRepository.RemoveFromRole(user.Id, roleName, cancellationToken, _transaction);
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrEmpty(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            try
            {
                var result = await _userRepository.RemoveLogin(user.Id, loginProvider, providerKey, cancellationToken, _transaction);
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            try
            {
                var result = await _userRepository.UpdateClaim(user.Id, claim, newClaim, cancellationToken, _transaction);
                if (result)
                {
                    CommitTransaction();
                }
                else
                {
                    RollbackTransaction();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount = 0;

            return Task.FromResult(0);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnd = lockoutEnd;

            return Task.FromResult(0);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Email = normalizedEmail;

            return Task.FromResult(0);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.TwoFactorEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userRepository.Update(user, cancellationToken, _transaction);

                if (result)
                {
                    CommitTransaction();
                    return IdentityResult.Success;
                }
                else
                {
                    RollbackTransaction();

                    return IdentityResult.Failed();
                }
            }
            catch (Exception ex)
            {
                RollbackTransaction();

                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }
    }
}
