using Ematig_Portal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.DAL
{
    public class UnitOfWork : IDisposable
    {
        private EmatigBDContext Context = new EmatigBDContext();

        private Repository<Message> _messageRepository;
        public Repository<Message> MessageRepository
        {
            get
            {

                if (this._messageRepository == null)
                {
                    this._messageRepository = new Repository<Message>(this.Context);
                }
                return _messageRepository;
            }
        }

        private Repository<Settings> _settingsRepository;
        public Repository<Settings> SettingsRepository
        {
            get
            {

                if (this._settingsRepository == null)
                {
                    this._settingsRepository = new Repository<Settings>(this.Context);
                }
                return _settingsRepository;
            }
        }

        private Repository<User> _userRepository;
        public Repository<User> UserRepository
        {
            get
            {

                if (this._userRepository == null)
                {
                    this._userRepository = new Repository<User>(this.Context);
                }
                return _userRepository;
            }
        }

        private Repository<MessageTypeDestination> _messageTypeDestinationRepository;
        public Repository<MessageTypeDestination> MessageTypeDestinationRepository
        {
            get
            {

                if (this._messageTypeDestinationRepository == null)
                {
                    this._messageTypeDestinationRepository = new Repository<MessageTypeDestination>(this.Context);
                }
                return _messageTypeDestinationRepository;
            }
        }

        public bool Save()
        {
            return this.Context.SaveChanges() > 0;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
