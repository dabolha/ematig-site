using Ematig_Portal.BLL;
using Ematig_Portal.Domain.Enum;
using Ematig_Portal.Domain.Enum.ActionResult;
using Ematig_Portal.Domain.Interface;
using Ematig_Portal.Helpers;
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

        protected void ProcessResult(Domain.ActionResult actionResult, ResultMessageType? resultMessageType = null)
        {
            string message = string.Empty;
            DisplayMessageType displayMessageType = DisplayMessageType.Error;

            if (resultMessageType == null)
            {
                ProcessResultMessageType(actionResult, out resultMessageType);
            }

            if (resultMessageType == null)
                resultMessageType = ResultMessageType.Error;

            ProcessMessageType(resultMessageType.Value, out displayMessageType, out message);

            DisplayMessage displayMessage = new DisplayMessage(TempData);
            displayMessage.Add(displayMessageType, message);
        }

        protected void ProcessResultMessageType(Domain.ActionResult actionResult, out ResultMessageType? resultMessageType)
        {
            resultMessageType = ResultMessageType.Error;

            if (actionResult == null)
            {
                resultMessageType = ResultMessageType.Error;
            }

            if (actionResult.OperationStatus is MessageEnum)
            {
                switch((MessageEnum)actionResult.OperationStatus)
                {
                    case MessageEnum.Success:
                        resultMessageType = ResultMessageType.OperationSuccess;
                        break;
                    
                    case MessageEnum.Error:
                    default:
                        resultMessageType = ResultMessageType.Error;
                        break;
                }
            }
            else if (actionResult.OperationStatus is SettingsEnum)
            {
                switch ((SettingsEnum)actionResult.OperationStatus)
                {
                    case SettingsEnum.Success:
                        resultMessageType = ResultMessageType.OperationSuccess;
                        break;

                    case SettingsEnum.KeyAlreadyExists:
                    case SettingsEnum.Error:
                    default:
                        resultMessageType = ResultMessageType.Error;
                        break;
                }
            }
            else if (actionResult.OperationStatus is UserEnum)
            {
                switch ((UserEnum)actionResult.OperationStatus)
                {
                    case UserEnum.Success:
                        resultMessageType = ResultMessageType.OperationSuccess;
                        break;

                    case UserEnum.EmailAlreadyExists:
                        resultMessageType = ResultMessageType.EmailAlreadyExists;
                        break;

                    case UserEnum.InvalidCredentials:
                        resultMessageType = ResultMessageType.InvalidCredentials;
                        break;

                    case UserEnum.InvalidUser:
                        resultMessageType = ResultMessageType.InvalidUser;
                        break;

                    case UserEnum.Error:
                    default:
                        resultMessageType = ResultMessageType.Error;
                        break;
                }
            }
        }

        protected void ProcessMessageType(ResultMessageType resultMessageType, out DisplayMessageType displayMessageType, out string message)
        {
            displayMessageType = DisplayMessageType.Error;
            message = string.Empty;

            switch (resultMessageType)
            {
                case ResultMessageType.OperationSuccess:
                    message = "Operação realizada com sucesso.";
                    break;

                case ResultMessageType.RegisterUserSuccess:
                    message = "Utilizador registado com sucesso.";
                    break;

                case ResultMessageType.EmailAlreadyExists:
                    message = "O Email indicado já se encontra registado.";
                    break;

                case ResultMessageType.SentMessageSuccess:
                    message = "Mensagem enviada com sucesso.";
                    break;

                case ResultMessageType.ChangeInfoSuccess:
                    message = "Informação atualizada com sucesso.";
                    break;

                case ResultMessageType.RemoveLoginSuccess:
                    message = "The external login was removed.";
                    break;

                case ResultMessageType.InvalidCredentials:
                    message = "Email ou Password inválido.";
                    break;

                case ResultMessageType.InvalidUser:
                    message = "Utilizador inválido.";
                    break;

                case ResultMessageType.Error:
                default:
                    message = "Ocorreu um erro com o seu pedido.<br/>Por favor tente mais tarde.";
                    break;
            }

        }

        public void LogError(string message = null)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception((message ?? "Ocorreu um erro com o seu pedido.<br/>Por favor tente mais tarde.")));
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