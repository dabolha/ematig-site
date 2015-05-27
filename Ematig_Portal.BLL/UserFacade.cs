using Ematig_Portal.DAL;
using Ematig_Portal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.BLL
{
    public class UserFacade : FacadeBase<long, Domain.User>
    {
        public UserFacade()
            : base()
        {
        }

        public UserFacade(UnitOfWork repository)
            : base(repository)
        {
        }

        public override long Add(User input, ref ActionResult actionResult)
        {
            if (actionResult == null)
                actionResult = new ActionResult();

            if (input == null)
                return -1;

            Domain.User user = new Domain.User()
            {
                AuthId = input.AuthId,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Gender = input.Gender,
                Address = input.Address,
                PostCode = input.PostCode,
                MobilePhoneNumber = input.MobilePhoneNumber,
                BirthDate = input.BirthDate,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now
            };

            this.Repository.UserRepository.Insert(user);

            if (actionResult.Success = this.Repository.Save())
            {
                return user.Id;
            }

            return -1;

        }

        public override void Update(User input, ref ActionResult actionResult)
        {
            if (actionResult == null)
                actionResult = new ActionResult();

            if (input == null)
                return;

            var user = this.Repository.UserRepository.FirstOrDefault(item => item.Id == input.Id);
            if (user == null)
            {
                return;
            }

            user.Email = (input.Email ?? "").Trim();
            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.Gender = input.Gender;
            user.Address = input.Address;
            user.PostCode = input.PostCode;
            user.MobilePhoneNumber = input.MobilePhoneNumber;
            user.BirthDate = input.BirthDate;
            user.PhoneNumber = input.PhoneNumber;
            user.ModificationDate = DateTime.Now;

            actionResult.Success = this.Repository.Save();

        }

        public override void Delete(User input, ref ActionResult actionResult)
        {
            throw new NotImplementedException();
        }

        public override User GetByKey(long key)
        {
            return this.Repository.UserRepository
                .FirstOrDefault
                    (
                        item =>
                            item.Id == key

                    );
        }

        public override User GetByCustom(Expression<Func<User, bool>> filter = null)
        {
            return this.Repository.UserRepository.FirstOrDefault(filter);
        }

        public override ICollection<User> Get()
        {
            return this.Repository.UserRepository.ToList();
        }
        
    }
}
