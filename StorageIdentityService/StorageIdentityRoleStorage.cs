using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StorageIdentityService
{
    public class StorageIdentityRoleStorage<TRole> : IRoleStore<TRole> where TRole : StorageIdentityRole
    {
        private readonly StorageIdentityContext _db;
        public StorageIdentityRoleStorage(IOptions<StorageConfigurations> configs)
        {
            _db = new StorageIdentityContext(configs.Value.ConnectionString, configs.Value.PrefixTable);
        }
        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            TableResult InsertResult = await _db.RoleData.ExecuteAsync(TableOperation.Insert(role));

            return InsertResult.HttpStatusCode == HttpStatusCode.NoContent.GetHashCode() ? IdentityResult.Success : IdentityResult.Failed(new IdentityError()
            {
                Code = InsertResult.HttpStatusCode.ToString(),
                Description = "Create Role Failed."
            });
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            TRole Role = await FindByNameAsync(role.Name, cancellationToken);

            if (Role == null) return IdentityResult.Failed(new IdentityError()
            {
                Code = "404",
                Description = "Role Not Found."
            });

            
            Role.ETag = "*";
            TableResult DeleteResult = await _db.RoleData.ExecuteAsync(TableOperation.Delete(Role));
            return DeleteResult.HttpStatusCode == HttpStatusCode.NoContent.GetHashCode() ? IdentityResult.Success : IdentityResult.Failed(new IdentityError()
            {
                Code = DeleteResult.HttpStatusCode.ToString(),
                Description = "Delete Failed."
            });
        }

        public void Dispose()
        {
            
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) => FindByNameAsync(roleId, cancellationToken);

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            TableResult RetrieveResult = await _db.RoleData.ExecuteAsync(TableOperation.Retrieve<TRole>("RoleData", normalizedRoleName));
            return RetrieveResult.HttpStatusCode == HttpStatusCode.OK.GetHashCode() ? (TRole)RetrieveResult.Result : null;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(role.RowKey);

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(role.RowKey);

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) => Task.FromResult(role.RowKey);

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken) => Task.Factory.StartNew(() => role.NormalizedName = normalizedName);

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken) => Task.Factory.StartNew(() => role.Name = roleName);

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            // Retrieve
            TRole Role = await FindByIdAsync(role.RowKey, cancellationToken);
            if (Role == null) return IdentityResult.Failed(new IdentityError()
            {
                Code = HttpStatusCode.NotFound.ToString(),
                Description = "Role Not Found."
            });

            // Update (Insert New)
            TableResult InsertResult = await _db.RoleData.ExecuteAsync(TableOperation.InsertOrReplace(role));
            if (InsertResult.HttpStatusCode != HttpStatusCode.NoContent.GetHashCode()) return IdentityResult.Failed(new IdentityError()
            {
                Code = InsertResult.HttpStatusCode.ToString(),
                Description = "Update Failed."
            });

            // Delete
            Role.ETag = "*";
            TableResult DeleteResult = await _db.RoleData.ExecuteAsync(TableOperation.Delete(Role));
            return DeleteResult.HttpStatusCode == HttpStatusCode.NoContent.GetHashCode() ? IdentityResult.Success : IdentityResult.Failed(new IdentityError()
            {
                Code = DeleteResult.HttpStatusCode.ToString(),
                Description = "Update Failed."
            });
        }
    }
}
