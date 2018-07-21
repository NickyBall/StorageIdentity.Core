using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace StorageIdentityService
{
    public class StorageIdentityUserStorage<TUser> :
        IUserStore<TUser>,
        IUserRoleStore<TUser>

        where TUser : StorageIdentityUser
    {
        private readonly IStorageHelper<TUser> Storage;

        public StorageIdentityUserStorage(IStorageHelper<TUser> Storage)
        {
            this.Storage = Storage;
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken) => Task.Factory.StartNew(() =>
        {
            if (user.Roles.Contains(roleName)) user.Roles.Add(roleName);
        });

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken) => Storage.CreateUserAsync(user);

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken) => Storage.DeleteUserFromDbAsync(user);

        public void Dispose()
        {
            
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken) => Storage.SelectUserFromDbAsync(userId);

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Storage.SelectUserFromDbAsync(normalizedUserName);

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult<IList<string>>(user.Roles);

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken) => Task.FromResult(user.Roles.Contains(roleName));

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken) => Task.Factory.StartNew(() =>
        {
            if (user.Roles.Contains(roleName)) user.Roles.Remove(roleName);
        });

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.NormalizedUserName = normalizedName);

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.UserName = userName);

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken) => Storage.UpdateUserAsync(user);
    }
}
