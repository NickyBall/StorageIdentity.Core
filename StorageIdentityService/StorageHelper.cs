using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace StorageIdentityService
{
    public class StorageHelper<TUser> : IStorageHelper<TUser> where TUser : StorageIdentityUser
    {
        private readonly StorageIdentityContext _db;
        public StorageHelper(IOptions<StorageConfigurations> configs)
        {
            _db = new StorageIdentityContext(configs.Value.ConnectionString, configs.Value.PrefixTable);
        }

        public Task<IdentityResult> CreateUserAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteUserFromDbAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> SelectUserFromDbAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateUserAsync(TUser user)
        {
            throw new NotImplementedException();
        }
    }
}
