using Ematig_Portal.Domain;
using Ematig_Portal.Domain.Constants;
using Ematig_Portal.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.BLL
{
    public class MessageFacade : FacadeBase<long, Domain.Message>
    {
        public MessageFacade()
            : base()
        {
        }

        public MessageFacade(EmatigBDContext context)
            : base(context)
        {
        }

        public override long Add(Message input, ref ActionResult actionResult)
        {
            if (actionResult == null)
                actionResult = new ActionResult();

            if (input == null)
                return -1;

            Message message = new Message()
            {
                Body = input.Body,
                MessageTypeId = input.MessageTypeId
            };

            message = ProcessMessage(this.Context, message);

            this.Context.Message.Add(message);

            if (actionResult.Success = this.Context.SaveChanges() > 0)
            {
                return message.Id;
            }

            return -1;
        }

        public override void Update(Message input, ref ActionResult actionResult)
        {
            if (actionResult == null)
                actionResult = new ActionResult();

            if (input == null)
                return;

            var message = this.Context.Message
                .FirstOrDefault(item => item.Id == input.Id);

            if (message == null)
                return;

            message.SentDate = input.SentDate;

            actionResult.Success = this.Context.SaveChanges() > 0;
        }

        public override void Delete(Message input, ref ActionResult actionResult)
        {
            throw new NotImplementedException();
        }

        public override Message GetByKey(long key)
        {
            return this.Context.Message
                .FirstOrDefault(item => item.Id == key);
        }

        public override ICollection<Message> Get()
        {
            return this.Context.Message.ToList();
        }

        private Message ProcessMessage(EmatigBDContext context, Message input)
        {
            if (input == null)
                return null;

            MessageTypeDestination messageTypeDestination = GetMessageDestination(context, MessageTypeEnum.ContactRequest);
            if (messageTypeDestination == null)
                return null;
            
            input.FromEmail = GetMessageFromEmail(context);
            input.ToEmail = messageTypeDestination.ToEmail;
            input.Subject = messageTypeDestination.Subject;
            input.CreationDate = DateTime.Now;

            return input;
        }

        private string GetMessageFromEmail(EmatigBDContext context)
        {
            var setting = new SettingsFacade(context).GetByKey(SettingKey.EmailFrom);
            return setting == null ? string.Empty : setting.Value;
        }

        private MessageTypeDestination GetMessageDestination(EmatigBDContext context, MessageTypeEnum messageTypeEnum)
        {
            int messageType = (int)messageTypeEnum;

            return context.MessageTypeDestination
                .FirstOrDefault(item => item.MessageTypeId == messageType);
        }
    }
}
