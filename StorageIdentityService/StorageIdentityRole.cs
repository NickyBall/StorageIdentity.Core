using Microsoft.AspNetCore.Identity;
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
    public class StorageIdentityRole : IdentityRole, ITableEntity
    {
        // PK = "RoleData", RK = {RoleName}

        // PK = "RoleUser_{RoleName}", RK = {UserName}

        // PK = "UserRole_{UserName}, RK = {RoleName}"

        public StorageIdentityRole(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

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
