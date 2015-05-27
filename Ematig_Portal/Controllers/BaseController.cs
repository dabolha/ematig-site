using Ematig_Portal.BLL;
using Ematig_Portal.Domain.Enum;
using Ematig_Portal.Domain.Interface;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ematig_Portal.Controllers
{
    public class BaseController : Controller
    {
        #region Facade

        private IIdentityControllerService _identityService;
        public IIdentityControllerService IdentityService
        {
            get
            {
                if (this._identityService == null)
                {
                    this._identityService = new IdentityFacade();
                }
                return this._identityService;
            }
            set
            {
                _identityService = value;
            }
        }

        private IControllerService<string, Domain.Settings> _settingsService;
        public IControllerService<string, Domain.Settings> SettingsService
        {
            get
            {
                if (this._settingsService == null)
                {
                    this._settingsService = new SettingsFacade();
                }
                return this._settingsService;
            }
        }

        private IControllerService<long, Domain.Message> _messageService;
        public IControllerService<long, Domain.Message> MessageService
        {
            get
            {
                if (this._messageService == null)
                {
                    this._messageService = new MessageFacade();
                }
                return this._messageService;
            }
        }

        private IControllerService<long, Domain.User> _userService;
        public IControllerService<long, Domain.User> UserService
        {
            get
            {
                if (this._userService == null)
                {
                    this._userService = new UserFacade();
                }
                return this._userService;
            }
        }

        #endregion

        //protected void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        protected ActionResult RedirectToLocal(string returnUrl)
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

        protected string ProcessResultMessage(ResultMessageType? resultMessageType)
        {
            return resultMessageType == ResultMessageType.OperationSuccess ? "Operação realizada com sucesso."
                    : resultMessageType == ResultMessageType.SentMessageSuccess ? "Your message has been sent."
                    : resultMessageType == ResultMessageType.ChangeInfoSuccess ? "Your information has been changed."
                    : resultMessageType == ResultMessageType.RemoveLoginSuccess ? "The external login was removed."
                    : resultMessageType == ResultMessageType.InvalidCredentials ? "Invalid username or password."
                    : resultMessageType == ResultMessageType.InvalidUser? "Invalid user."
                    : resultMessageType == ResultMessageType.Error ? "An error has occurred."
                    : "";
        }

        public void Success(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Success, message, dismissable);
        }

        public void Information(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Information, message, dismissable);
        }

        public void Warning(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Warning, message, dismissable);
        }

        public void Error(string message, bool dismissable = true)
        {
            LogError(message);

            AddAlert(AlertStyles.Danger, message, dismissable);
        }

        private void AddAlert(string alertStyle, string message, bool dismissable)
        {
            var alerts = TempData.ContainsKey(Alert.TempDataKey)
                ? (List<Alert>)TempData[Alert.TempDataKey]
                : new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable
            });

            TempData[Alert.TempDataKey] = alerts;
        }

        public void LogError(string message)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(message));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SettingsService.Dispose();
                this.MessageService.Dispose();
                this.UserService.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}