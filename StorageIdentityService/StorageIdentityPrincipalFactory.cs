using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StorageIdentityService
{
    public class StorageIdentityPrincipalFactory<TUser> : IUserClaimsPrincipalFactory<TUser> where TUser : StorageIdentityUser
    {
        public Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            ClaimsIdentity identity = new ClaimsIdentity("Microsoft.AspNet.Identity.Application");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

            if (user.Role != null)
            {
                foreach (var item in user.Role)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, item));
                }
            }


            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            return Task.FromResult(principal);
        }
    }
}
