using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;

namespace StorageIdentityService
{
    public class StorageIdentityUserStorage<TUser> : 
        IUserRoleStore<TUser>,
        //IPasswordHasher<TUser>,
        IUserPasswordStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>
        where TUser : StorageIdentityUser, new()
    {
        private readonly StorageIdentityContext _db;

        public StorageIdentityUserStorage(IOptions<StorageConfigurations> configs)
        {
            _db = new StorageIdentityContext(configs.Value.ConnectionString, configs.Value.PrefixTable);
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            await _db.RoleData.ExecuteAsync(TableOperation.Insert(new StorageIdentityRole($"RoleUser_{roleName}", user.RowKey)));

            await _db.RoleData.ExecuteAsync(TableOperation.Insert(new StorageIdentityRole($"UserRole_{user.RowKey}", roleName)));
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            user.PartitionKey = user.PartitionKey ?? "UserData";
            TableResult InsertResult = await _db.UserData.ExecuteAsync(TableOperation.Insert(user));
            return InsertResult.HttpStatusCode == HttpStatusCode.NoContent.GetHashCode() ? IdentityResult.Success : IdentityResult.Failed(new IdentityError()
            {
                Code = InsertResult.HttpStatusCode.ToString(),
                Description = "Insert Failed."
            });
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            TUser User = await FindByIdAsync(user.RowKey, cancellationToken);
            if (User == null) return IdentityResult.Failed(new IdentityError()
            {
                Code = HttpStatusCode.NotFound.ToString(),
                Description = "User Not Found."
            });

            User.ETag = "*";
            TableResult DeleteResult = await _db.UserData.ExecuteAsync(TableOperation.Delete(User));
            return DeleteResult.HttpStatusCode == HttpStatusCode.NoContent.GetHashCode() ? IdentityResult.Success : IdentityResult.Failed(new IdentityError()
            {
                Code = DeleteResult.HttpStatusCode.ToString(),
                Description = "Delete User Failed."
            });
        }

        public void Dispose()
        {
            
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            TableQuerySegment<TUser> Segment = await _db.UserData.ExecuteQuerySegmentedAsync(new TableQuery<TUser>().Where($"Email eq '{normalizedEmail}'"), null);
            if (Segment.Count() > 0) return Segment.FirstOrDefault();
            return null;
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            TableResult RetrieveResult = await _db.UserData.ExecuteAsync(TableOperation.Retrieve<TUser>("UserData", userId));
            return RetrieveResult.HttpStatusCode == HttpStatusCode.OK.GetHashCode() ? (TUser)RetrieveResult.Result : null;
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => FindByIdAsync(normalizedUserName, cancellationToken);

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.EmailConfirmed);

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedEmail);

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PhoneNumber);

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PhoneNumberConfirmed);

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            TableQuerySegment Segment = await _db.RoleData.ExecuteQuerySegmentedAsync(new TableQuery().Where($"PartitionKey eq 'UserRole_{user.RowKey}'"), null);
            return Segment.Count() > 0 ? (IList<string>)Segment.Select(role => role.RowKey) : null;
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            TableQuerySegment<TUser> Segment = await _db.RoleData.ExecuteQuerySegmentedAsync(new TableQuery<TUser>().Where($"PartitionKey eq 'UserRole_{roleName}'"), null);
            return Segment.Count() > 0 ? (IList<TUser>)Segment : null;
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            TableResult RetrieveResult = await _db.RoleData.ExecuteAsync(TableOperation.Retrieve<TUser>($"RoleUser_{roleName}", user.RowKey));
            return RetrieveResult.HttpStatusCode == HttpStatusCode.OK.GetHashCode();
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            TableResult RoleUserData = await _db.RoleData.ExecuteAsync(TableOperation.Retrieve<TUser>($"RoleUser_{roleName}", user.RowKey));
            if (RoleUserData.HttpStatusCode == HttpStatusCode.OK.GetHashCode())
            {
                TUser User = (TUser)RoleUserData.Result;
                User.ETag = "*";
                await _db.RoleData.ExecuteAsync(TableOperation.Delete(User));
            }

            TableResult UserRoleData = await _db.RoleData.ExecuteAsync(TableOperation.Retrieve<TUser>($"UserRole_{user.RowKey}", roleName));
            if (UserRoleData.HttpStatusCode == HttpStatusCode.OK.GetHashCode())
            {
                TUser User = (TUser)UserRoleData.Result;
                User.ETag = "*";
                await _db.RoleData.ExecuteAsync(TableOperation.Delete(User));
            }
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.Email = email);

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.EmailConfirmed = confirmed);

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.NormalizedEmail = normalizedEmail);

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.NormalizedUserName = normalizedName);

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.PasswordHash = passwordHash);

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.PhoneNumber = phoneNumber);

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.PhoneNumberConfirmed = confirmed);

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.UserName = userName);

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            TableResult UpdateResult = await _db.UserData.ExecuteAsync(TableOperation.InsertOrReplace(user));
            return UpdateResult.HttpStatusCode == HttpStatusCode.NoContent.GetHashCode() ? IdentityResult.Success : IdentityResult.Failed(new IdentityError()
            {
                Code = UpdateResult.HttpStatusCode.ToString(),
                Description = "Update User Failed."
            });

            //// Retrieve
            //TUser User = await FindByIdAsync(user.RowKey, cancellationToken);
            //if (User == null) return IdentityResult.Failed(new IdentityError()
            //{
            //    Code = HttpStatusCode.NotFound.ToString(),
            //    Description = "User Not Found."
            //});

            //// Insert
            //IdentityResult Result = await CreateAsync(user, cancellationToken);
            //if (Result != IdentityResult.Success) return IdentityResult.Failed(Result.Errors.FirstOrDefault());

            //// Delete
            //return await DeleteAsync(User, cancellationToken);
        }
    }
}
