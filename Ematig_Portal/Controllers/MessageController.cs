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

        #region Send

        //
        // GET: /Message/Send
        [AllowAnonymous]
        public ActionResult Send()
        {
            return PartialView("_Send");
        }

        //
        // POST: /Message/Send
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Send(MessageModel model)
        {
            if (!ModelState.IsValid)
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("Contact", "Home");
            }

            using (var context = this._BbContext)
            {
                Message message = ProcessMessage(context, model);
                if (message == null)
                {
                    Error(ProcessResultMessage(ResultMessageType.Error));
                    return RedirectToAction("Contact", "Home");
                }

                context.Message.Add(message);

                if (context.SaveChanges() == 0)
                {
                    Error(ProcessResultMessage(ResultMessageType.Error));
                    return RedirectToAction("Contact", "Home");
                }

                Success(ProcessResultMessage(ResultMessageType.SentMessageSuccess));
                return RedirectToAction("Contact", "Home");
            }
        }

        #endregion

        #region Get

        //
        // GET: /Message/All
        public ActionResult All()
        {
            using (var context = this._BbContext)
            {
                var messageList = context.Message
                    .ToList();
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
        }

        #endregion

        #region Edit

        //
        // GET: /Message/Edit/Id
        public ActionResult Edit(long id)
        {
            using (var context = this._BbContext)
            {
                var message = context.Message
                    .FirstOrDefault(item => item.Id == id);

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

            using (var context = this._BbContext)
            {
                var message = context.Message
                    .FirstOrDefault(item => item.Id == model.Message.Id);

                if (message == null)
                {
                    Error(ProcessResultMessage(ResultMessageType.Error));
                    return RedirectToAction("All");
                }

                message.CreationDate = model.Message.CreationDate;

                if (context.SaveChanges() > 0)
                {
                    Success(ProcessResultMessage(ResultMessageType.OperationSuccess), true);
                    return RedirectToAction("All");
                }
                else
                {
                    Error(ProcessResultMessage(ResultMessageType.Error));
                }
            }

            return RedirectToAction("All");
        }

        #endregion

        private Message ProcessMessage(EmatigBbContext context, MessageModel model)
        {
            MessageTypeDestination messageTypeDestination = GetMessageDestination(context, MessageTypeEnum.ContactRequest);
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
            Settings settings = new SettingsController().Get(SettingKey.EmailFrom);
            return settings == null ? string.Empty : settings.Value;
        }

        private MessageTypeDestination GetMessageDestination(EmatigBbContext context, MessageTypeEnum messageTypeEnum)
        {
            int messageType = (int)messageTypeEnum;
         
            return context.MessageTypeDestination
                .FirstOrDefault(item => item.MessageTypeId == messageType);
        }
	}
}