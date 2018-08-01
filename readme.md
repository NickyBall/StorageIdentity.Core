# Storage Identity

This is a library for ASP.NET CORE Identity using Azure Table Storage.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

To use this library, the followings are required.

```
.NET Core 2.1 or later
```

### Installing

You can use this library by downloading the zip file of this project or using NuGet.Org.

NuGet Package Manager

```
Install-Package StorageIdentity.Core
```

.NET CLI

```
dotnet add package StorageIdentity.Core
```

## Sample Usage

This library provides a simple extension of services.

There are 5 model classes involved in the identity.

* StorageIdentityUser
* StorageIdentityRole
* StorageIdentityUserClaim
* StorageIdentityUserLogin
* StorageIdentityUserToken

### Default

If you want to use the default of all classes above. Simply use this.

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddStorageIdentityService("<ConnectionString>", "<TablePrefix>");
}
```

### Extend StorageIdentityUser

You can implement an inherited class of StorageIdentityUser and pass through the extension like this.

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddStorageIdentityService<TUser>("<ConnectionString>", "<TablePrefix>");
}
```

### Extend All Classes

If you want to write your own StorageIdentity class, you can pass through the extension by the followings order.

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddStorageIdentityService<TUser, TRole, TUserClaim, TUserLogin, TUserToken>("<ConnectionString>", "<TablePrefix>");
}
```

## Authors

* **Jakkrit Junrat** - *Appswin Limited Partnership* - [NickyBall](https://github.com/NickyBall)

## License

This project is licensed under the MIT License.