using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageIdentityService
{
    public class StorageIdentityContext : IDisposable
    {
        public CloudTable UserData { get { ThrowIfDisposed(); return _UserData; } set { _UserData = value; } }
        private CloudTable _UserData;

        public CloudTable RoleData { get { ThrowIfDisposed(); return _RoleData; } set { _RoleData = value; } }
        private CloudTable _RoleData;

        public CloudTable UserTokenData { get { ThrowIfDisposed(); return _UserTokenData; } set { _UserTokenData = value; } }
        private CloudTable _UserTokenData;

        #region Initialization
        private CloudTableClient CloudStorageClient = null;
        public StorageIdentityContext(string ConnectionString, string PrefixTable)
        {
            CloudStorageClient = CloudStorageAccount.Parse(ConnectionString).CreateCloudTableClient();
            _UserData = CloudStorageClient.GetTableReference(PrefixTable + "StorageUser");
            _RoleData = CloudStorageClient.GetTableReference(PrefixTable + "StorageRole");
            _UserTokenData = CloudStorageClient.GetTableReference(PrefixTable + "StorageUserToken");

            _UserData.CreateIfNotExistsAsync();
            _RoleData.CreateIfNotExistsAsync();
            _UserTokenData.CreateIfNotExistsAsync();
        }
        private bool _disposed = false;
        ~StorageIdentityContext()
        {
            this.Dispose(false);
        }
        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                CloudStorageClient = null;
                //Insert Remove on this!!!!!
                _disposed = true;
                _UserData = null;
                _RoleData = null;
            }
        }
        #endregion 
    }
}
