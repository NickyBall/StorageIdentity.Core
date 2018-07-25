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
    /*
     * Copyright 2018 Jakkrit Junrat
     *
     * Licensed under the Apache License, Version 2.0 (the "License");
     * you may not use this file except in compliance with the License.
     * You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */
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
