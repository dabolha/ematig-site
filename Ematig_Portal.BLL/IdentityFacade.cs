using Ematig_Portal.BLL.Identity;
using Ematig_Portal.Domain;
using Ematig_Portal.Domain.Constants;
using Ematig_Portal.Domain.Interface;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.BLL
{
    public class IdentityFacade : IIdentityControllerService
    {
        private ApplicationUserManager UserIdentityManager { get; set; }
        private UserStore<ApplicationUser> UserStore { get; set; }
        private UserIdentityContext UserIdentityContext
        {
            get
            {
                if (this.UserStore != null)
                    return UserStore.Context as UserIdentityContext;

                return new UserIdentityContext();
            }
        }
        private EmatigBDContext Context { get; set; }
        public IAuthenticationManager AuthenticationManager { get; set; }

        public IdentityFacade()
        {
            this.Context = new EmatigBDContext();
            this.UserStore = new UserStore<ApplicationUser>(new UserIdentityContext());
            this.UserIdentityManager = new ApplicationUserManager(this.UserStore);//new UserManager<ApplicationUser>(this._UserStore);
        }

        #region Login

        public async Task SignInAsync(
            ApplicationUser user,
            bool isPersistent,
            bool rememberBrowser)
        {
            // Clear any partial cookies from external or two factor partial sign ins
            AuthenticationManager.SignOut(
                DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.TwoFactorCookie);

            using (var context = this.Context)
            {
                var internalUser = context.User.FirstOrDefault(item => item.AuthId == user.Id);
                if (internalUser != null)
                {
                    user.UserName = internalUser.FirstName;
                }
            }

            var userIdentity = await GenerateUserIdentityAsync(user, UserIdentityManager);

            if (rememberBrowser)
            {
                var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(user.Id);
                AuthenticationManager.SignIn(
                    new AuthenticationProperties { IsPersistent = isPersistent },
                    userIdentity,
                    rememberBrowserIdentity);
            }
            else
            {
                AuthenticationManager.SignIn(
                    new AuthenticationProperties { IsPersistent = isPersistent },
                    userIdentity);
            }
        }

        public async Task<bool> SendTwoFactorCode(string provider)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return false;
            }

            var token = await UserIdentityManager.GenerateTwoFactorTokenAsync(userId, provider);

            // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
            await UserIdentityManager.NotifyTwoFactorTokenAsync(userId, provider, token);
            return true;
        }

        public async Task<string> GetVerifiedUserIdAsync()
        {
            var result = await AuthenticationManager.AuthenticateAsync(
                DefaultAuthenticationTypes.TwoFactorCookie);

            if (result != null && result.Identity != null
                && !String.IsNullOrEmpty(result.Identity.GetUserId()))
            {
                return result.Identity.GetUserId();
            }
            return null;
        }

        public async Task<bool> HasBeenVerified()
        {
            return await GetVerifiedUserIdAsync() != null;
        }

        //public async Task<SignInStatus> TwoFactorSignIn(
        //    string provider,
        //    string code,
        //    bool isPersistent,
        //    bool rememberBrowser)
        //{
        //    var userId = await GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return SignInStatus.Failure;
        //    }

        //    var user = await UserManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return SignInStatus.Failure;
        //    }

        //    if (await UserManager.IsLockedOutAsync(user.Id))
        //    {
        //        return SignInStatus.LockedOut;
        //    }

        //    if (await UserManager.VerifyTwoFactorTokenAsync(user.Id, provider, code))
        //    {
        //        // When token is verified correctly, clear the access failed 
        //        // count used for lockout
        //        await UserManager.ResetAccessFailedCountAsync(user.Id);
        //        await SignInAsync(user, isPersistent, rememberBrowser);
        //        return SignInStatus.Success;
        //    }

        //    // If the token is incorrect, record the failure which 
        //    // also may cause the user to be locked out
        //    await UserManager.AccessFailedAsync(user.Id);
        //    return SignInStatus.Failure;
        //}


        //public async Task<SignInStatus> ExternalSignIn(
        //    ExternalLoginInfo loginInfo,
        //    bool isPersistent)
        //{
        //    var user = await UserManager.FindAsync(loginInfo.Login);
        //    if (user == null)
        //    {
        //        return SignInStatus.Failure;
        //    }

        //    if (await UserManager.IsLockedOutAsync(user.Id))
        //    {
        //        return SignInStatus.LockedOut;
        //    }

        //    return await SignInOrTwoFactor(user, isPersistent);
        //}


        //private async Task<SignInStatus> SignInOrTwoFactor(
        //    ApplicationUser user,
        //    bool isPersistent)
        //{
        //    //TODO: RequiresTwoFactorAuthentication
        //    //if (await UserManager.GetTwoFactorEnabledAsync(user.Id) &&
        //    //    !await AuthenticationManager.TwoFactorBrowserRememberedAsync(user.Id))
        //    //{
        //    //    var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
        //    //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
        //    //    AuthenticationManager.SignIn(identity);
        //    //    return SignInStatus.RequiresTwoFactorAuthentication;
        //    //}
        //    await SignInAsync(user, isPersistent, false);
        //    return SignInStatus.Success;
        //}

        public async Task<SignInStatus> PasswordSignIn(
            string userName,
            string password,
            bool isPersistent,
            bool shouldLockout)
        {
            var user = await UserIdentityManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInStatus.Failure;
            }

            if (await UserIdentityManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }

            if (await UserIdentityManager.CheckPasswordAsync(user, password))
            {
                //return await SignInOrTwoFactor(user, isPersistent);
                await SignInAsync(user, isPersistent, false);

                return SignInStatus.Success;
            }

            if (shouldLockout)
            {
                // If lockout is requested, increment access failed 
                // count which might lock out the user
                await UserIdentityManager.AccessFailedAsync(user.Id);
                if (await UserIdentityManager.IsLockedOutAsync(user.Id))
                {
                    return SignInStatus.LockedOut;
                }
            }
            return SignInStatus.Failure;
        }

        #endregion 

        #region Logout

        public void LogOff()
        {
            this.AuthenticationManager.SignOut();
        }

        #endregion

        #region Add

        public string Add(Domain.ApplicationUser identityUser, string password)
        {
            if (identityUser == null || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var result = UserIdentityManager.CreateAsync(identityUser, password);
            if (identityUser != null)
            {
                return identityUser.Id;
            }

            //Send Email Confirmation
            //var code = await _UserIdentityManager.GenerateEmailConfirmationTokenAsync(identityUser.Id);
            //var callbackUrl = Url.Action("ConfirmEmail", "Account",
            //    new { userId = identityUser.Id, code = code }, protocol: Request.Url.Scheme);
            //await _UserIdentityManager.SendEmailAsync(identityUser.Id,
            //    "Confirm your account",
            //    "Please confirm your account by clicking this link: <a href=\""
            //    + callbackUrl + "\">link</a>");
            //ViewBag.Link = callbackUrl;
            //return View("DisplayEmail");

            return null;
        }

        #endregion

        #region Update

        public bool Update(Domain.ApplicationUser identityUser, string oldPassword, string newPassword)
        {
            if (identityUser == null)
            {
                return false;
            }

            Domain.ApplicationUser user = null;

            using (var context = this.UserIdentityContext)
            {
                user = context.Users.FirstOrDefault(item => item.Id == identityUser.Id);
                if (user == null)
                {
                    return false;
                }

                if ((user.UserName ?? "").Trim() != (identityUser.Email ?? "").Trim())
                {
                    user.UserName = (identityUser.Email ?? "").Trim();
                    if (context.SaveChanges() <= 0)
                    {
                        return false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword))
            {
                var result = this.UserIdentityManager.ChangePasswordAsync(user.Id, oldPassword, newPassword);
                if (result == null)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Get

        public Domain.ApplicationUser GetByKey(string key)
        {
            throw new NotImplementedException();

            //var identityUser = context.Users.FirstOrDefault(item => item.Id == model.AuthId);
        }

        #endregion

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUser user, UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one 
            // defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.System, Application.Name),
            };
            userIdentity.AddClaims(claims);

            //var claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
    }
}
