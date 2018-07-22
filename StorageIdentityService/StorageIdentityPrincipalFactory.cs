using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StorageIdentityService
{
    public class StorageIdentityPrincipalFactory<TUser> : IUserClaimsPrincipalFactory<TUser> where TUser : StorageIdentityUser
    {
        private readonly IUserRoleStore<TUser> UserRoleStore;
        public StorageIdentityPrincipalFactory(IUserRoleStore<TUser> UserRoleStore)
        {
            this.UserRoleStore = UserRoleStore;
        }

        public async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            ClaimsIdentity identity = new ClaimsIdentity("Microsoft.AspNet.Identity.Application");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

            var Roles = await UserRoleStore.GetRolesAsync(user, CancellationToken.None);

            if (Roles != null)
            {
                foreach (var item in Roles.ToList())
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, item));
                }
            }


            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
