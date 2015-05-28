using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Ematig_Portal.Models;
using System.Data.Entity;
using Ematig_Portal.Domain.Enum;
using Ematig_Portal.Helpers.Filter;
using Microsoft.AspNet.Identity.Owin;
using Ematig_Portal.BLL;

namespace Ematig_Portal.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                if (HttpContext != null)
                {
                    var owinContext = HttpContext.GetOwinContext();
                    if (owinContext != null)
                    {
                        return owinContext.Authentication;
                    }
                }

                return null;
            }
        }

        #region Login

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateModelStateAttribute]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            this.IdentityService.AuthenticationManager = this.AuthenticationManager;
            var result = await this.IdentityService.PasswordSignIn(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:

                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout"); //TODO:

                //case SignInStatus.RequiresTwoFactorAuthentication:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.IdentityService.AuthenticationManager = this.AuthenticationManager;
            this.IdentityService.LogOff();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Register

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateModelStateAttribute]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            //TODO: RF
            ((UserFacade)this.UserService).UserName = model.Email;
            ((UserFacade)this.UserService).Password = model.Password;
            ((UserFacade)this.UserService).AuthenticationManager = this.AuthenticationManager;

            Domain.User user = new Domain.User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Address = model.Address,
                PostCode = model.PostCode,
                MobilePhoneNumber = model.MobilePhoneNumber,
                BirthDate = model.BirthDate,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            
            Domain.ActionResult actionResult = null;
            
            this.UserService.Add(user, ref actionResult);

            if (actionResult == null || ! actionResult.Success)
            {
                ProcessResult(actionResult);
                LogError();
            }
            else
            {
                ProcessResult(actionResult);
                return RedirectToAction("All");
            }
            
            return View(model);
        }

        #endregion

        #region Manage

        //
        // GET: /Account/Manage/id
        public ActionResult Edit(long? id)
        {
            ViewBag.ReturnUrl = Url.Action("All");

            if (User == null || User.Identity == null)
            {
                ProcessResult(null, ResultMessageType.Error);
                LogError();
                return View();
            }

            Domain.User user = null;

            if (id == null)
            {
                string authId = User.Identity.GetUserId();
                user = this.UserService.GetByCustom(item => (item.AuthId ?? "").Trim() == (authId ?? "").Trim());
            }
            else
            {
                user = this.UserService.GetByKey(id.Value);
            }
            
            if (user == null)
            {
                ProcessResult(null, ResultMessageType.Error);
                LogError();
                return View();
            }

            ManageUserViewModel model = new ManageUserViewModel
            {
                Id = user.Id,
                AuthId = user.AuthId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Address = user.Address,
                PostCode = user.PostCode,
                MobilePhoneNumber = user.MobilePhoneNumber,
                BirthDate = user.BirthDate,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelStateAttribute]
        public async Task<ActionResult> Edit(ManageUserViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            Domain.User user = this.UserService.GetByKey(model.Id);
            if (user == null)
            {
                ProcessResult(null, ResultMessageType.InvalidUser);
                return View(model);
            }

            if ((user.Email ?? "").Trim().ToLower() != (model.Email ?? "").Trim().ToLower())
            {
                user.Email = (model.Email ?? "").Trim();
            }

            #region ChangeUserInfo

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;
            user.Address = model.Address;
            user.PostCode = model.PostCode;
            user.MobilePhoneNumber = model.MobilePhoneNumber;
            user.BirthDate = model.BirthDate;
            user.PhoneNumber = model.PhoneNumber;

            Domain.ActionResult actionResult = null;

            //TODO: RF
            ((UserFacade)this.UserService).NewPassword = model.NewPassword;
            ((UserFacade)this.UserService).OldPassword = model.OldPassword;
            ((UserFacade)this.UserService).AuthenticationManager = this.AuthenticationManager;

            this.UserService.Update(user, ref actionResult);

            if (actionResult == null || ! actionResult.Success)
            {
                ProcessResult(null, ResultMessageType.InvalidUser);
                return View(model);
            }

            #endregion

            ProcessResult(actionResult, ResultMessageType.ChangeInfoSuccess);
            return RedirectToAction("All");
        }

        #endregion

        #region Get

        //
        // GET: /Account/All
        public ActionResult All()
        {
            var userList = this.UserService.Get();
            if (userList == null || userList.Count == 0)
            {
                return View();
            }

            IEnumerable<ManageUserViewModel> list = userList
                .Select(item => new ManageUserViewModel()
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Gender = item.Gender,
                    Address = item.Address,
                    PostCode = item.PostCode,
                    MobilePhoneNumber = item.MobilePhoneNumber,
                    BirthDate = item.BirthDate,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber
                })
                .ToList();

            return View(list);
        }

        #endregion

        ////
        //// POST: /Account/Disassociate
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //{
        //    ResultMessageType? message = null;
        //    IdentityResult result = await this._UserIdentityManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        message = ResultMessageType.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ResultMessageType.Error;
        //    }
        //    return RedirectToAction("Manage", new { Message = ProcessResultMessage(message) });
        //}

        ////
        //// POST: /Account/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        //}

        ////
        //// GET: /Account/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await _AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Manage", new { Message = ProcessResultMessage(ResultMessageType.Error) });
        //    }
        //    var result = await this._UserIdentityManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Manage");
        //    }
        //    return RedirectToAction("Manage", new { Message = ProcessResultMessage(ResultMessageType.Error) });
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = this._UserIdentityManager.GetLogins(User.Identity.GetUserId());
        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && this._UserIdentityManager != null)
        //    {
        //        this._UserIdentityManager.Dispose();
        //        this._UserIdentityManager = null;
        //    }
        //    base.Dispose(disposing);
        //}

        //// Used for XSRF protection when adding external logins
        //private const string XsrfKey = "XsrfId";

        

        //private async Task SignInAsync(ApplicationUser identityUser, User user, bool isPersistent)
        //{
        //    if (identityUser == null)
        //        return;

        //    if (user != null)
        //    {
        //        identityUser.UserName = user.FirstName;
        //    }

        //    _AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await this._UserIdentityManager.CreateIdentityAsync(identityUser, DefaultAuthenticationTypes.ApplicationCookie);
        //    _AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}

        //private bool HasPassword()
        //{
        //    var user = this._UserIdentityManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PasswordHash != null;
        //    }
        //    return false;
        //}

        //private class ChallengeResult : HttpUnauthorizedResult
        //{
        //    public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
        //    {
        //    }

        //    public ChallengeResult(string provider, string redirectUri, string userId)
        //    {
        //        LoginProvider = provider;
        //        RedirectUri = redirectUri;
        //        UserId = userId;
        //    }

        //    public string LoginProvider { get; set; }
        //    public string RedirectUri { get; set; }
        //    public string UserId { get; set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
        //        if (UserId != null)
        //        {
        //            properties.Dictionary[XsrfKey] = UserId;
        //        }
        //        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        //    }
        //}
    }
}