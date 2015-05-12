using Ematig_Portal.Helpers;
using Ematig_Portal.Models;
using Ematig_Portal.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ematig_Portal.Constants;

namespace Ematig_Portal.Controllers
{
    public class MessageController : BaseController
    {
        #region Properties

        private EmatigBbContext _BbContext { get; set; }

        #endregion

        #region Constructor

        public MessageController()
        {
            this._BbContext = new EmatigBbContext();
        }

        #endregion

        //
        // GET: /Message/Send
        [AllowAnonymous]
        public ActionResult Send()
        {
            return PartialView("_SendMessage");
        }

        //
        // POST: /Message/Send
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Send(SendMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return PartialView("_SendMessage", model);
            }

            Message message = ProcessMessage(model);
            if (message == null)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return PartialView("_SendMessage", model);
            }

            using (var context = this._BbContext)
            {
                context.Message.Add(message);

                if (context.SaveChanges() == 0)
                {
                    Error(ProcessResultMessage(ResultMessageType.Error));
                    return PartialView("_SendMessage", model);
                }

                Success(ProcessResultMessage(ResultMessageType.SentMessageSuccess));
                return PartialView("_SendMessage");
            }
        }

        private Message ProcessMessage(SendMessageViewModel model)
        {
            MessageTypeDestination messageTypeDestination = GetMessageDestination(MessageTypeEnum.ContactRequest);
            if (messageTypeDestination == null)
            {
                return null;
            }

            return new Message()
            {
                FromEmail = GetMessageFromEmail(),
                ToEmail = messageTypeDestination.ToEmail,
                Body = string.Format(MessageTypeTemplate.ContactRequest, model.FirstName, model.LastName, model.Email, model.PhoneNumber, model.Title, model.Message),
                Subject = messageTypeDestination.Subject,
                MessageTypeId = (short)MessageTypeEnum.ContactRequest,
                CreationDate = DateTime.Now
            };
        }

        private string GetMessageFromEmail()
        {
            throw new NotImplementedException();
        }

        private MessageTypeDestination GetMessageDestination(MessageTypeEnum messageTypeEnum)
        {
            throw new NotImplementedException();
        }
	}
}