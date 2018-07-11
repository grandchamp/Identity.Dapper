using Identity.Dapper.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.Dapper.Claims
{
    public class IdentityDapperUserClaimsPrincipalFactory<TUser> : UserClaimsPrincipalFactory<TUser>
        where TUser : DapperIdentityUser
    {
        private readonly IPropertyMapper<TUser> _propertyMapper;
        public IdentityDapperUserClaimsPrincipalFactory(IPropertyMapper<TUser> propertyMapper, UserManager<TUser> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
            _propertyMapper = propertyMapper;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var mappings = _propertyMapper.GetAllMappings();

            var principal = await base.CreateAsync(user);

            ((ClaimsIdentity)principal.Identity).AddClaims(mappings.Select(mapping => new Claim(mapping.Key, mapping.Value.Value.ToString())));

            return principal;
        }
    }
}
