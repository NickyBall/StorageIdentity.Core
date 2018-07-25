using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageIdentity.Models;
using StorageIdentityService;

[assembly: HostingStartup(typeof(StorageIdentity.Areas.Identity.IdentityHostingStartup))]
namespace StorageIdentity.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                //services.AddDbContext<StorageIdentityContextModel>(options =>
                //    options.UseSqlServer(
                //        context.Configuration.GetConnectionString("StorageIdentityContextModelConnection")));

                //services.AddDefaultIdentity<StorageIdentityUser>()
                //    .AddEntityFrameworkStores<StorageIdentityContextModel>()
                //    ;
            });
        }
    }
}