using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StorageIdentityService
{
    public static class StorageIdentityServiceExtension
    {
        public static IServiceCollection AddStorageIdentityService<TUser, TRole>(this IServiceCollection services, string ConnectionString, string TablePrefix) 
            where TUser : StorageIdentityUser, new() 
            where TRole : StorageIdentityRole
        {
            services.Configure<StorageConfigurations>(options =>
            {
                options.ConnectionString = ConnectionString;
                options.PrefixTable = TablePrefix;
            });
            
            services.AddSingleton<IUserStore<TUser>, StorageIdentityUserStorage<TUser>>();
            services.AddSingleton<IUserRoleStore<TUser>, StorageIdentityUserStorage<TUser>>();
            services.AddSingleton<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.AddSingleton<IRoleStore<TRole>, StorageIdentityRoleStorage<TRole>>();
            services.AddSingleton<IUserClaimsPrincipalFactory<TUser>, StorageIdentityPrincipalFactory<TUser>>();
            services.AddSingleton<IUserLockoutStore<TUser>, StorageIdentityUserStorage<TUser>>();
            services.AddSingleton<IUserTwoFactorStore<TUser>, StorageIdentityUserStorage<TUser>>();
            //services.AddIdentity<TUser, TRole>().AddDefaultTokenProviders();

            services.AddScoped<SignInManager<TUser>>();
            services.AddScoped<UserManager<TUser>>();
            services.AddScoped<RoleManager<TRole>>();

            return services;
        }
    }
}
