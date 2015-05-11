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
using Ematig_Portal.Models.Data;

namespace Ematig_Portal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Properties

        //UserIdentityDbContext
        private UserStore<ApplicationUser> _UserStore { get; set; }
        private UserIdentityDbContext _UserIdentityContext
        {
            get
            {
                if (this._UserStore != null)
                    return _UserStore.Context as UserIdentityDbContext;

                return new UserIdentityDbContext();
            }
        }
        private UserManager<ApplicationUser> _UserIdentityManager { get; set; }

        //EmatigBbContext
        private EmatigBbContext _BbContext { get; set; }

        #endregion

        #region Constructor

        //public AccountController()
        //    : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new UserIdentityDbContext())))
        //{
        //    this._BbContext = new EmatigBbContext();
        //}

        //public AccountController(UserManager<ApplicationUser> userManager)
        //{
        //    this._UserIdentityManager = userManager;
        //}

        public AccountController()
        {
            this._BbContext = new EmatigBbContext();

            this._UserStore = new UserStore<ApplicationUser>(new UserIdentityDbContext());
            this._UserIdentityManager = new UserManager<ApplicationUser>(this._UserStore);
        }

        #endregion

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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await this._UserIdentityManager.FindAsync(model.Email, model.Password);
                if (identityUser == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }

                using (var context = this._BbContext)
                {
                    var user = context.User.FirstOrDefault(item => item.AuthId == identityUser.Id);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                        return View(model);
                    }

                    await SignInAsync(identityUser, user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
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
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: error
                return View(model);
            }
            var identityUser = new ApplicationUser()
            {
                UserName = model.Email
                //Email = model.Email,
            };
            var result = await this._UserIdentityManager.CreateAsync(identityUser, model.Password);
            if (! result.Succeeded)
            {
                AddErrors(result);
            }

            using (var context = this._BbContext)
            {
                var user = new User()
                {
                    AuthId = identityUser.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    Address = model.Address,
                    PostCode = model.PostCode,
                    MobilePhoneNumber = model.MobilePhoneNumber,
                    BirthDate = model.BirthDate,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CreationDate = DateTime.Now,
                    ModificationDate = DateTime.Now
                };

                context.User.Add(user);

                if (context.SaveChanges() > 0)
                {
                    //await SignInAsync(identityUser, user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Manage

        //
        // GET: /Account/Manage/id
        public ActionResult Manage(long? id)
        {
            //ViewBag.StatusMessage =
            //    message == ManageMessageId.ChangeInfoSuccess ? "Your information has been changed."
            //    : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
            //    : message == ManageMessageId.Error ? "An error has occurred."
            //    : "";

            ViewBag.ReturnUrl = Url.Action("All");

            string authId = User.Identity.GetUserId();

            using (var context = this._BbContext)
            {
                var user = context
                    .User.FirstOrDefault
                        (
                            item => 
                                (id != null) 
                                ? (item.Id == id)
                                : ((item.AuthId ?? "").Trim() == (authId ?? "").Trim())

                        );

                if (user == null)
                {
                    //TODO: error 
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
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            if (model != null)
            {
                bool emailChanged = false;

                #region ChangeUserInfo
                
                using (var context = this._BbContext)
                {
                    var user = context.User.FirstOrDefault(item => item.Id == model.Id);
                    if (user == null)
                    {
                        //TODO: error
                        ModelState.AddModelError("", "Invalid user.");
                        return View(model);
                    }

                    #region Email changed
                    if ((user.Email ?? "").Trim().ToLower() != (model.Email ?? "").Trim().ToLower())
                    {
                        emailChanged = true;
                        user.Email = (model.Email ?? "").Trim();
                    }
                    #endregion

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Gender = model.Gender;
                    user.Address = model.Address;
                    user.PostCode = model.PostCode;
                    user.MobilePhoneNumber = model.MobilePhoneNumber;
                    user.BirthDate = model.BirthDate;
                    user.PhoneNumber = model.PhoneNumber;
                    user.ModificationDate = DateTime.Now;

                    if (context.SaveChanges() <= 0)
                    {
                        //TODO: error
                        ModelState.AddModelError("", "Invalid user.");
                        return View(model);
                    }
                }

                #endregion

                #region Email changed
                if (emailChanged)
                {
                    using (var context = this._UserIdentityContext)
                    {
                        var identityUser = context.Users.FirstOrDefault(item => item.Id == model.AuthId);
                        if (identityUser == null)
                        {
                            //TODO: error
                            ModelState.AddModelError("", "Invalid user.");
                            return View(model);
                        }

                        identityUser.UserName = (model.Email ?? "").Trim();

                        if (context.SaveChanges() <= 0)
                        {
                            //TODO: error
                            ModelState.AddModelError("", "Invalid user.");
                            return View(model);
                        }
                    }
                }
                #endregion

                #region Password changed
                if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    if (ModelState.IsValid)
                    {
                        IdentityResult result = await this._UserIdentityManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                        if (!result.Succeeded)
                        {
                            AddErrors(result);
                        }
                    }
                }
                #endregion

                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangeInfoSuccess });

                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Get

        //
        // GET: /Account/All
        public ActionResult All()
        {
            using (var context = this._BbContext)
            {
                var userList = context.User.ToList();
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
        }

        #endregion

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await this._UserIdentityManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await this._UserIdentityManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = this._UserIdentityManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this._UserIdentityManager != null)
            {
                this._UserIdentityManager.Dispose();
                this._UserIdentityManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser identityUser, User user, bool isPersistent)
        {
            if (identityUser == null)
                return;

            if (user != null)
            {
                identityUser.UserName = user.FirstName;
            }

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await this._UserIdentityManager.CreateIdentityAsync(identityUser, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = this._UserIdentityManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangeInfoSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}