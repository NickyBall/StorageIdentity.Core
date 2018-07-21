using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageIdentityService
{
    public class StorageIdentityRoleManager<TRole> : RoleManager<TRole> where TRole : StorageIdentityRole
    {
        public StorageIdentityRoleManager (
            IRoleStore<TRole> store, 
            IEnumerable<IRoleValidator<TRole>> roleValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            ILogger<RoleManager<TRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
