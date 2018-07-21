using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StorageIdentityService
{
    public class StorageIdentityUserManager<TUser> : UserManager<TUser> where TUser : StorageIdentityUser
    {
        //private readonly StorageIdentityContext _db;
        //private readonly IStorageHelper<TUser> Storage;

        public StorageIdentityUserManager(
            IOptions<StorageConfigurations> configs,
            IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger,
            IStorageHelper<TUser> Storage
            ) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            //_db = new StorageIdentityContext(configs.Value.ConnectionString, configs.Value.PrefixTable);
            //this.Storage = Storage;
        }

        //public TUser GetUser(ClaimsPrincipal principal)
        //{
        //    if (principal == null)
        //    {
        //        return null;
        //        //throw new ArgumentNullException(nameof(principal));
        //    }
        //    string username = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        //    TUser user = Storage.SelectUserFromDbAsync(username).GetAwaiter().GetResult();
        //    return user;
        //}
        //public override Task<bool> IsEmailConfirmedAsync(TUser user) => Task.FromResult(user.EmailConfirmed);
        //public override async Task<TUser> FindByEmailAsync(string email) => await Storage.SelectUserFromDbAsync(email);
        //public override async Task<TUser> FindByIdAsync(string userId) => await Storage.SelectUserFromDbAsync(userId);
        //public override Task<string> GetEmailAsync(TUser user) => Task.FromResult(user.Email);
        //public override async Task<IdentityResult> ConfirmEmailAsync(TUser user, string token)
        //{
        //    bool result = token.Equals(user.EmailConfirmToken);
        //    if (result)
        //    {
        //        user.EmailConfirmed = true;
        //        await UpdateUserAsync(user);
        //    }
        //    return new AppIdentityResult(result);
        //}
        //public override async Task<string> GeneratePasswordResetTokenAsync(TUser user)
        //{
        //    await Task.Delay(1);
        //    byte[] arr = GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new TokenModel { Obj = user, Time = DateTime.UtcNow }));
        //    string token = Convert.ToBase64String(arr);
        //    return token;
        //}
        //public bool VerifyResetToken(string token)
        //{
        //    try
        //    {
        //        byte[] arr = Convert.FromBase64String(token);
        //        string json = GetString(arr);
        //        TokenModel tokenModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(json);
        //        if (DateTime.UtcNow.AddMinutes(-15) > tokenModel.Time)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }

        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}

        //public override async Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword)
        //{
        //    try
        //    {
        //        byte[] arr = Convert.FromBase64String(token);
        //        string json = GetString(arr);
        //        TokenModel tokenModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(json);
        //        TUser userOld = tokenModel.Obj as TUser;
        //        if (userOld.Id.Equals(user.Id))
        //        {
        //            user.PasswordHash = PasswordHasher.HashPassword(user, newPassword);
        //            return await Storage.UpdateUserAsync(user);
        //        }
        //        else
        //        {
        //            return new AppIdentityResult(false);
        //        }

        //    }
        //    catch
        //    {
        //        return new AppIdentityResult(false);
        //    }

        //}
        //public override async Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
        //{
        //    var x = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        //    if (PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword) == PasswordVerificationResult.Success)
        //    {
        //        user.PasswordHash = PasswordHasher.HashPassword(user, newPassword);
        //        return await Storage.UpdateUserAsync(user);
        //    }
        //    else
        //    {

        //        return IdentityResult.Failed(new IdentityError { Code = String.Empty, Description = "รหัสผ่านไม่ถูกต้อง" });
        //    }
        //}


        //public override Task<string> GetPhoneNumberAsync(TUser user) => Task.FromResult(user.PhoneNumber);
        //public override async Task<string> GenerateChangePhoneNumberTokenAsync(TUser user, string phoneNumber)
        //{
        //    int _min = 0;
        //    int _max = 9999;
        //    Random _rdm = new Random();
        //    int x = _rdm.Next(_min, _max);
        //    string code = x.ToString().PadLeft(4, '0');
        //    user.PhoneConfirmToken = code;
        //    user.PhoneWaitConfirm = phoneNumber;
        //    await UpdateUserAsync(user);
        //    return code;
        //}
        //public override async Task<IdentityResult> ChangePhoneNumberAsync(TUser user, string phoneNumber, string token)
        //{
        //    if (user.PhoneWaitConfirm.Equals(phoneNumber) && user.PhoneConfirmToken.Equals(token))
        //    {
        //        user.PhoneNumber = phoneNumber;
        //        user.PhoneNumberConfirmed = true;
        //        user.PhoneWaitConfirm = String.Empty;
        //        user.PhoneConfirmToken = String.Empty;
        //        await UpdateUserAsync(user);
        //        return IdentityResult.Success;
        //    }
        //    else
        //    {
        //        return IdentityResult.Failed(new IdentityError { Code = String.Empty, Description = "รหัสไม่ถูกต้อง" });
        //    }
        //}

        //public override async Task<IdentityResult> SetTwoFactorEnabledAsync(TUser user, bool enabled)
        //{
        //    user.TwoFactorEnabled = enabled;
        //    return await UpdateUserAsync(user);
        //}




        //#region Helper
        //public class AppIdentityResult : IdentityResult
        //{
        //    public AppIdentityResult(bool result)
        //    {
        //        this.Succeeded = result;
        //    }
        //}
        //public class TokenModel
        //{
        //    public TUser Obj { get; set; }
        //    public DateTime Time { get; set; }

        //}


        //private byte[] GetBytes(string str)
        //{
        //    byte[] bytes = new byte[str.Length * sizeof(char)];
        //    System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        //    return bytes;
        //}

        //private string GetString(byte[] bytes)
        //{
        //    char[] chars = new char[bytes.Length / sizeof(char)];
        //    System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        //    return new string(chars);
        //}


        //#endregion
    }
}
