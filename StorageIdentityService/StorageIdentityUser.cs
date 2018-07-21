using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage;

namespace StorageIdentityService
{
    public class StorageIdentityUser : IdentityUser, ITableEntity
    {
        public string EmailConfirmToken { get; set; }
        public string PhoneConfirmToken { get; set; }
        public string PhoneWaitConfirm { get; set; }
        public string TwoFactorToken { get; set; }


        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }
        public override string Id { get => RowKey; set => RowKey = value; }
        public override string UserName { get => RowKey; set => RowKey = value; }
        public override string NormalizedUserName { get => RowKey; set => RowKey = value; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            TableEntity.ReadUserObject(this, properties, operationContext);
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return TableEntity.WriteUserObject(this, operationContext);
        }
    }
}
