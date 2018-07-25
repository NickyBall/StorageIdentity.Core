using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

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
    public class StorageIdentityContext : IDisposable
    {
        public CloudTable UserData { get { ThrowIfDisposed(); return _UserData; } set { _UserData = value; } }
        private CloudTable _UserData;

        public CloudTable RoleData { get { ThrowIfDisposed(); return _RoleData; } set { _RoleData = value; } }
        private CloudTable _RoleData;

        public CloudTable UserTokenData { get { ThrowIfDisposed(); return _UserTokenData; } set { _UserTokenData = value; } }
        private CloudTable _UserTokenData;

        public CloudTable UserLoginData { get { ThrowIfDisposed(); return _UserLoginData; } set { _UserLoginData = value; } }
        private CloudTable _UserLoginData;

        public CloudTable UserClaimData { get { ThrowIfDisposed(); return _UserClaimData; } set { _UserClaimData = value; } }
        private CloudTable _UserClaimData;

        #region Initialization
        private CloudTableClient CloudStorageClient = null;
        public StorageIdentityContext(string ConnectionString, string PrefixTable)
        {
            CloudStorageClient = CloudStorageAccount.Parse(ConnectionString).CreateCloudTableClient();
            _UserData = CloudStorageClient.GetTableReference(PrefixTable + "StorageUser");
            _RoleData = CloudStorageClient.GetTableReference(PrefixTable + "StorageRole");
            _UserTokenData = CloudStorageClient.GetTableReference(PrefixTable + "StorageUserToken");
            _UserLoginData = CloudStorageClient.GetTableReference(PrefixTable + "StorageUserLogin");
            _UserClaimData = CloudStorageClient.GetTableReference(PrefixTable + "StorageUserClaim");

            _UserData.CreateIfNotExistsAsync();
            _RoleData.CreateIfNotExistsAsync();
            _UserTokenData.CreateIfNotExistsAsync();
            _UserLoginData.CreateIfNotExistsAsync();
            _UserClaimData.CreateIfNotExistsAsync();
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
