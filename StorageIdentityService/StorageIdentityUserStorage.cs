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
using System.Security.Claims;

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
    public class StorageIdentityUserStorage<TUser, TUserClaim, TUserLogin, TUserToken> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>,
        IQueryableUserStore<TUser>
        where TUser : StorageIdentityUser, new()
        where TUserClaim : StorageIdentityUserClaim, new()
        where TUserLogin : StorageIdentityUserLogin, new()
        where TUserToken : StorageIdentityUserToken, new()
    {
        private readonly StorageIdentityContext _db;

        private const string InternalLoginProvider = "[AspNetUserStore]";

        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";

        private const string RecoveryCodeTokenName = "RecoveryCodes";

        private bool _disposed = false;

        public StorageIdentityUserStorage(IOptions<StorageConfigurations> configs)
        {
            _db = new StorageIdentityContext(configs.Value.ConnectionString, configs.Value.PrefixTable);
        }

        public IQueryable<TUser> Users
        {
            get
            {
                TableQuerySegment<TUser> Segment = _db.UserData.ExecuteQuerySegmentedAsync(new TableQuery<TUser>().Where($"PartitionKey eq 'UserData'"), null).GetAwaiter().GetResult();
                if (Segment.Count() > 0) return Segment.AsQueryable();
                return null;
            }
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            await _db.RoleData.ExecuteAsync(TableOperation.Insert(new StorageIdentityRole($"RoleUser_{roleName}", user.RowKey)));

            await _db.RoleData.ExecuteAsync(TableOperation.Insert(new StorageIdentityRole($"UserRole_{user.RowKey}", roleName)));
        }

        public async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();



            if (user == null)

            {

                throw new ArgumentNullException(nameof(user));

            }

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";

            if (mergedCodes.Length > 0)

            {

                return mergedCodes.Split(';').Length;

            }

            return 0;
        }

        protected void ThrowIfDisposed()
        {
            //if (_disposed)throw new ObjectDisposedException(GetType().Name);
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
            _disposed = true;
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

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.AccessFailedCount);

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken) => GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.EmailConfirmed);

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.LockoutEnabled);

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.LockoutEnd);

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedEmail);

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PhoneNumber);

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.PhoneNumberConfirmed);

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            TableQuerySegment Segment = await _db.RoleData.ExecuteQuerySegmentedAsync(new TableQuery().Where($"PartitionKey eq 'UserRole_{user.RowKey}'"), null);
            //if (Segment.Count() > 0) { IList<string> x = Segment.Select(role => role.RowKey).ToList(); }
            return Segment.Count() > 0 ? Segment.Select(role => role.RowKey).ToList() : null;
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.SecurityStamp);

        public async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();

            if (user == null)

            {

                throw new ArgumentNullException(nameof(user));

            }

            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken);

            return entry?.Value;
        }

        private async Task<TUserToken> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null) throw new ArgumentNullException(nameof(user));
            
            TableResult RetrieveResult = await _db.UserTokenData.ExecuteAsync(TableOperation.Retrieve<StorageIdentityUserToken>($"UserTokenData_{loginProvider}", user.Id));
            return RetrieveResult.HttpStatusCode == HttpStatusCode.OK.GetHashCode() ? (TUserToken)RetrieveResult.Result : null;
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.TwoFactorEnabled);

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            TableQuerySegment<TUser> Segment = await _db.RoleData.ExecuteQuerySegmentedAsync(new TableQuery<TUser>().Where($"PartitionKey eq 'UserRole_{roleName}'"), null);
            return Segment.Count() > 0 ? (IList<TUser>)Segment : null;
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) => Task.FromResult(++user.AccessFailedCount);

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            TableResult RetrieveResult = await _db.RoleData.ExecuteAsync(TableOperation.Retrieve<TUser>($"RoleUser_{roleName}", user.RowKey));
            return RetrieveResult.HttpStatusCode == HttpStatusCode.OK.GetHashCode();
        }

        public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null) throw new ArgumentNullException(nameof(user));

            if (code == null) throw new ArgumentNullException(nameof(code));

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";

            var splitCodes = mergedCodes.Split(';');

            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }

            return false;
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

        public async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)throw new ArgumentNullException(nameof(user));

            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken);

            if (entry != null) await RemoveUserTokenAsync(entry);
        }

        private async Task RemoveUserTokenAsync(TUserToken entry)
        {
            entry.ETag = "*";
            TableResult DeleteResult = await _db.UserTokenData.ExecuteAsync(TableOperation.Delete(entry));
        }

        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            var mergedCodes = string.Join(";", recoveryCodes);

            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.AccessFailedCount = 0);

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken) => SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.Email = email);

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.EmailConfirmed = confirmed);

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.LockoutEnabled = enabled);

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.LockoutEnd = lockoutEnd);

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.NormalizedEmail = normalizedEmail);

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.NormalizedUserName = normalizedName);

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.PasswordHash = passwordHash);

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.PhoneNumber = phoneNumber);

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.PhoneNumberConfirmed = confirmed);

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.SecurityStamp = stamp);

        public async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken);

            if (token == null) await AddUserTokenAsync(new TUserToken()
            {
                PartitionKey = $"UserTokenData_{loginProvider}",
                RowKey = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                UserId = user.Id
            });

            else token.Value = value;
        }

        private async Task AddUserTokenAsync(TUserToken UserToken)
        {
            TableResult InsertResult = await _db.UserTokenData.ExecuteAsync(TableOperation.Insert(UserToken));
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken) => Task.Factory.StartNew(() => user.TwoFactorEnabled = enabled);

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

        public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }
            TableResult InsertResult = await _db.UserLoginData.ExecuteAsync(TableOperation.Insert(new TUserLogin()
            {
                PartitionKey = $"user.Id_{login.LoginProvider}",
                RowKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey
            }));
        }

        public async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var UserLogin = await FindByLoginAsync(loginProvider, providerKey, cancellationToken);

            if (UserLogin == null) return;

            UserLogin.ETag = "*";
            TableResult DeleteResult = await _db.UserLoginData.ExecuteAsync(TableOperation.Delete(UserLogin));
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            TableQuerySegment<TUserLogin> Segment = await _db.UserLoginData.ExecuteQuerySegmentedAsync(new TableQuery<TUserLogin>().Where($"UserId eq '{user.Id}'"), null);
            return Segment.Count() > 0 ? Segment.Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey, ul.ProviderDisplayName)).ToList() : null;
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            TableResult RetrieveResult = await _db.UserLoginData.ExecuteAsync(TableOperation.Retrieve<TUserLogin>(loginProvider, providerKey));
            TUserLogin UserLogin = RetrieveResult.HttpStatusCode == HttpStatusCode.OK.GetHashCode() ? (TUserLogin)RetrieveResult.Result : null;
            if (UserLogin == null) return null;
            var User = await FindByIdAsync(UserLogin.UserId, cancellationToken);
            return User;
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            TableQuerySegment<TUserClaim> Segment = await _db.UserClaimData.ExecuteQuerySegmentedAsync(new TableQuery<TUserClaim>().Where($"PartitionKey eq '{user.Id}'"), null);
            return Segment.Count() > 0 ? Segment.Select(uc => uc.ToClaim()).ToList() : null;
        }

        public async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            TableBatchOperation Batch = new TableBatchOperation();
            foreach (Claim claim in claims)
            {
                Batch.Add(TableOperation.Insert(new TUserClaim()
                {
                    PartitionKey = user.Id,
                    RowKey = claim.Type,
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                }));
            }
            var Results = await _db.UserClaimData.ExecuteBatchAsync(Batch);
        }

        public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            TableResult ReplaceResult = await _db.UserClaimData.ExecuteAsync(TableOperation.InsertOrReplace(new TUserClaim()
            {
                PartitionKey = user.Id,
                RowKey = newClaim.Type,
                UserId = user.Id,
                ClaimType = newClaim.Type,
                ClaimValue = newClaim.Value
            }));
        }

        public async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            TableBatchOperation Batch = new TableBatchOperation();
            foreach (Claim claim in claims)
            {
                TableResult RetrieveResult = await _db.UserClaimData.ExecuteAsync(TableOperation.Retrieve<TUserClaim>(user.Id, claim.Type));
                if (RetrieveResult.HttpStatusCode == HttpStatusCode.OK.GetHashCode())
                {
                    TUserClaim Claim = (TUserClaim)RetrieveResult.Result;
                    Claim.ETag = "*";
                    Batch.Add(TableOperation.Delete(Claim));
                }
            }
            var Results = await _db.UserClaimData.ExecuteBatchAsync(Batch);
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            TableQuerySegment<TUserClaim> Segment = await _db.UserClaimData.ExecuteQuerySegmentedAsync(new TableQuery<TUserClaim>().Where($"ClaimType eq '{claim.Type}' and ClaimValue eq '{claim.Value}'"), null);
            List<TUser> Users = new List<TUser>();
            foreach (var uc in Segment)
            {
                var User = await FindByIdAsync(uc.UserId, cancellationToken);
                Users.Add(User);
            }
            return Users;
        }
    }
}
