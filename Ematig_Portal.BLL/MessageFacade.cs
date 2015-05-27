using Ematig_Portal.DAL;
using Ematig_Portal.Domain;
using Ematig_Portal.Domain.Constants;
using Ematig_Portal.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public MessageFacade(UnitOfWork repository)
            : base(repository)
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

            message = ProcessMessage(message);

            this.Repository.MessageRepository.Insert(message);

            if (actionResult.Success = this.Repository.Save())
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

            var message = this.Repository.MessageRepository
                .FirstOrDefault(item => item.Id == input.Id);

            if (message == null)
                return;

            message.SentDate = input.SentDate;

            actionResult.Success = this.Repository.Save();
        }

        public override void Delete(Message input, ref ActionResult actionResult)
        {
            throw new NotImplementedException();
        }

        public override Message GetByKey(long key)
        {
            return this.Repository.MessageRepository
                .FirstOrDefault(item => item.Id == key);
        }

        public override Domain.Message GetByCustom(Expression<Func<Domain.Message, bool>> filter = null)
        {
            return this.Repository.MessageRepository.FirstOrDefault(filter);
        }

        public override ICollection<Message> Get()
        {
            return this.Repository.MessageRepository.ToList();
        }

        private Message ProcessMessage(Message input)
        {
            if (input == null)
                return null;

            MessageTypeDestination messageTypeDestination = GetMessageDestination(MessageTypeEnum.ContactRequest);
            if (messageTypeDestination == null)
                return null;
            
            input.FromEmail = GetMessageFromEmail();
            input.ToEmail = messageTypeDestination.ToEmail;
            input.Subject = messageTypeDestination.Subject;
            input.CreationDate = DateTime.Now;

            return input;
        }

        private string GetMessageFromEmail()
        {
            var setting = new SettingsFacade(this.Repository).GetByKey(SettingKey.EmailFrom);
            return setting == null ? string.Empty : setting.Value;
        }

        private MessageTypeDestination GetMessageDestination(MessageTypeEnum messageTypeEnum)
        {
            int messageType = (int)messageTypeEnum;

            return this.Repository.MessageTypeDestinationRepository
                .FirstOrDefault(item => item.MessageTypeId == messageType);
        }
    }
}
