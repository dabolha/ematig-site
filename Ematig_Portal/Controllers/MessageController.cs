using Ematig_Portal.Domain.Enum;
using Ematig_Portal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ematig_Portal.Domain.Constants;
using Ematig_Portal.Helpers.Filter;

namespace Ematig_Portal.Controllers
{
    public class MessageController : BaseController
    {
        #region Send

        //
        // GET: /Message/Send
        [AllowAnonymous]
        public ActionResult ContactRequest()
        {
            return RedirectToAction("Contact", "Home");
        }

        //
        // POST: /Message/Send
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ContactRequest(MessageModel model)
        {
            if (!ModelState.IsValid)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("Contact", "Home");
            }

            var message = new Domain.Message()
            {
                Body = string.Format(MessageTypeTemplate.ContactRequest, model.FirstName, model.LastName, model.Email, model.PhoneNumber, model.Title, model.Message),
                MessageTypeId = (short)MessageTypeEnum.ContactRequest
            };

            Domain.ActionResult actionResult = null;
            this.MessageService.Add(message, ref actionResult);

            if (actionResult == null || !actionResult.Success)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("Contact", "Home");
            }

            Success(ProcessResultMessage(ResultMessageType.SentMessageSuccess));
            return RedirectToAction("Contact", "Home");
        }

        #endregion

        #region Get

        //
        // GET: /Message/All
        public ActionResult All()
        {
            var messageList = this.MessageService.Get();
            if (messageList == null || messageList.Count == 0)
            {
                return View();
            }

            MessageViewModel messageViewModel = new MessageViewModel();
            messageViewModel.MessageList = messageList.Select(item => new ViewMessageModel()
                {
                    Id = item.Id,
                    MessageTypeID = item.MessageTypeId,
                    ToEmail = item.ToEmail,
                    Subject = item.Subject,
                    Message = item.Body,
                    SentDate = item.SentDate,
                    CreationDate = item.CreationDate
                })
                .ToList();

            return View(messageViewModel);
        }

        #endregion

        #region Edit

        //
        // GET: /Message/Edit/Id
        public ActionResult Edit(long id)
        {
            var message = this.MessageService.GetByKey(id);
            if (message == null)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("All");
            }

            MessageViewModel messageViewModel = new MessageViewModel();
            messageViewModel.Message = new ViewMessageModel()
            {
                Id = message.Id,
                MessageTypeID = message.MessageTypeId,
                ToEmail = message.ToEmail,
                Subject = message.Subject,
                Message = message.Body,
                SentDate = message.SentDate,
                CreationDate = message.CreationDate
            };

            return PartialView("_Edit", messageViewModel);
        }

        //
        // POST: /Message/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MessageViewModel model)
        {
            if (!ModelState.IsValid
                || model == null
                || model.Message == null)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("All");
            }

            var message = this.MessageService.GetByKey(model.Message.Id);
            if (message == null)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("All");
            }

            message.SentDate = model.Message.SentDate;

            Domain.ActionResult actionResult = null;

            this.MessageService.Update(message, ref actionResult);

            if (actionResult == null || !actionResult.Success)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("All");
            }
            else
            {
                Success(ProcessResultMessage(ResultMessageType.OperationSuccess), true);
                return RedirectToAction("All");
            }
        }

        #endregion
	}
}