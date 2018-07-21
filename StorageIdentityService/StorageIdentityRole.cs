using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageIdentityService
{
    public class StorageIdentityRole : IdentityRole, ITableEntity
    {
        // PK = "RoleData", RK = {RoleName}

        // PK = "RoleUser_{Username}", RK = {RoleName}

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }
        public override string Id { get => RowKey; set => RowKey = value; }
        public override string Name { get => RowKey; set => RowKey = value; }
        public override string NormalizedName { get => RowKey; set => RowKey = value; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext) => TableEntity.ReadUserObject(this, properties, operationContext);

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext) => TableEntity.WriteUserObject(this, operationContext);
    }
}
