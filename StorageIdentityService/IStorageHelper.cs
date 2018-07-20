using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageIdentityService
{
    public interface IStorageHelper<o> where o : StorageIdentityUser
    {
        Task<o> SelectUserFromDbAsync(string username);
        Task<IdentityResult> CreateUserAsync(o user);
        Task<IdentityResult> UpdateUserAsync(o user);
        Task<IdentityResult> DeleteUserFromDbAsync(o user);
    }
}
