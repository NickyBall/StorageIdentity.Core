using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
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
    public static class StorageIdentityServiceExtension
    {
        public static IServiceCollection AddStorageIdentityService(this IServiceCollection services, string ConnectionString, string TablePrefix)
        {
            return services.AddStorageIdentityService<StorageIdentityUser, StorageIdentityRole, StorageIdentityUserClaim, StorageIdentityUserLogin, StorageIdentityUserToken>(ConnectionString, TablePrefix);
        }

        public static IServiceCollection AddStorageIdentityService<TUser>(this IServiceCollection services, string ConnectionString, string TablePrefix)
            where TUser : StorageIdentityUser, new()
        {
            return services.AddStorageIdentityService<TUser, StorageIdentityRole, StorageIdentityUserClaim, StorageIdentityUserLogin, StorageIdentityUserToken>(ConnectionString, TablePrefix);
        }

        public static IServiceCollection AddStorageIdentityService<TUser, TRole>(this IServiceCollection services, string ConnectionString, string TablePrefix)
            where TUser : StorageIdentityUser, new()
            where TRole : StorageIdentityRole
        {
            return services.AddStorageIdentityService<TUser, TRole, StorageIdentityUserClaim, StorageIdentityUserLogin, StorageIdentityUserToken>(ConnectionString, TablePrefix);
        }

        public static IServiceCollection AddStorageIdentityService<TUser, TRole, TUserClaim, TUserLogin, TUserToken>(this IServiceCollection services, string ConnectionString, string TablePrefix) 
            where TUser : StorageIdentityUser, new() 
            where TRole : StorageIdentityRole
            where TUserClaim : StorageIdentityUserClaim, new()
            where TUserLogin : StorageIdentityUserLogin, new()
            where TUserToken : StorageIdentityUserToken, new()
        {
            services.Configure<StorageConfigurations>(options =>
            {
                options.ConnectionString = ConnectionString;
                options.PrefixTable = TablePrefix;
            });
            services.AddSingleton<IUserStore<TUser>, StorageIdentityUserStorage<TUser, TUserClaim, TUserLogin, TUserToken>>();
            services.AddSingleton<IUserRoleStore<TUser>, StorageIdentityUserStorage<TUser, TUserClaim, TUserLogin, TUserToken>>();
            services.AddSingleton<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.AddSingleton<IRoleStore<TRole>, StorageIdentityRoleStorage<TRole>>();
            services.AddSingleton<IUserClaimsPrincipalFactory<TUser>, StorageIdentityPrincipalFactory<TUser>>();
            services.AddSingleton<IUserLockoutStore<TUser>, StorageIdentityUserStorage<TUser, TUserClaim, TUserLogin, TUserToken>>();
            services.AddSingleton<IUserTwoFactorStore<TUser>, StorageIdentityUserStorage<TUser, TUserClaim, TUserLogin, TUserToken>>();
            services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();

            services.AddScoped<SignInManager<TUser>>();
            services.AddScoped<UserManager<TUser>>();
            services.AddScoped<RoleManager<TRole>>();

            return services;
        }

        
    }
}
