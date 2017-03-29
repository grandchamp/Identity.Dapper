using Identity.Dapper.Entities;
using Identity.Dapper.UnitOfWork.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Dapper.Repositories.Contracts
{
    public interface IUserRepository<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>
        where TUser : DapperIdentityUser<TKey, TUserClaim, TUserRole, TUserLogin>
        where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
        where TUserClaim : DapperIdentityUserClaim<TKey>
        where TUserLogin : DapperIdentityUserLogin<TKey>
        where TRole : DapperIdentityRole<TKey, TUserRole, TRoleClaim>
    {
        Task<TKey> Insert(TUser user, CancellationToken cancellationToken, IUnitOfWork uow = null);
        Task<bool> Remove(TKey id, CancellationToken cancellationToken, IUnitOfWork uow = null);
        Task<bool> Update(TUser user, CancellationToken cancellationToken, IUnitOfWork uow = null);
        Task<TUser> GetById(TKey id);
        Task<TUser> GetByUserName(string userName);
        Task<TUser> GetByEmail(string email);
        Task<IEnumerable<TUser>> GetAll();
        Task<TUser> GetByUserLogin(string loginProvider, string providerKey);

        Task<bool> InsertClaims(TKey id, IEnumerable<Claim> claims, CancellationToken cancellationToken, IUnitOfWork uow = null);
        Task<bool> InsertLoginInfo(TKey id, Microsoft.AspNetCore.Identity.UserLoginInfo loginInfo, CancellationToken cancellationToken, IUnitOfWork uow = null);
        Task<bool> AddToRole(TKey id, string roleName, CancellationToken cancellationToken, IUnitOfWork uow = null);

        Task<IList<Claim>> GetClaimsByUserId(TKey id);
        Task<IList<string>> GetRolesByUserId(TKey id);
        Task<IList<Microsoft.AspNetCore.Identity.UserLoginInfo>> GetUserLoginInfoById(TKey id);
        Task<IList<TUser>> GetUsersByClaim(Claim claim);
        Task<IList<TUser>> GetUsersInRole(string roleName);
        Task<bool> IsInRole(TKey id, string roleName);

        Task<bool> RemoveClaims(TKey id, IEnumerable<Claim> claims, CancellationToken cancellationToken, IUnitOfWork uow = null);
        Task<bool> RemoveFromRole(TKey id, string roleName, CancellationToken cancellationToken, IUnitOfWork uow = null);
        Task<bool> RemoveLogin(TKey id, string loginProvider, string providerKey, CancellationToken cancellationToken, IUnitOfWork uow = null);
        Task<bool> UpdateClaim(TKey id, Claim oldClaim, Claim newClaim, CancellationToken cancellationToken, IUnitOfWork uow = null);
    }
}
