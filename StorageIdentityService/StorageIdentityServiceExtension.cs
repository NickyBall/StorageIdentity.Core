using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageIdentityService
{
    public static class StorageIdentityServiceExtension
    {
        public static IServiceCollection AddStorageIdentityService<TUser, TRole>(this IServiceCollection services, string ConnectionString, string TablePrefix) 
            where TUser : StorageIdentityUser 
            where TRole : StorageIdentityRole
        {
            services.Configure<StorageConfigurations>(options =>
            {
                options.ConnectionString = ConnectionString;
                options.PrefixTable = TablePrefix;
            });
            
            services.AddSingleton<IUserStore<TUser>, StorageIdentityUserStorage<TUser>>();
            services.AddSingleton<IUserRoleStore<TUser>, StorageIdentityUserStorage<TUser>>();
            services.AddSingleton<IRoleStore<TRole>, StorageIdentityRoleStorage<TRole>>();
            services.AddSingleton<IUserClaimsPrincipalFactory<TUser>, StorageIdentityPrincipalFactory<TUser>>();
            services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();

            services.AddScoped<UserManager<TUser>>();
            services.AddScoped<RoleManager<TRole>>();

            return services;
        }
    }
}
