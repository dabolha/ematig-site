using Ematig_Portal.DAL;
using Ematig_Portal.Domain;
using Ematig_Portal.Domain.Enum.ActionResult;
using Microsoft.Owin.Security;
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
        //TODO: RF
        public IAuthenticationManager AuthenticationManager { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        

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
                actionResult = new ActionResult() { OperationStatus = UserEnum.Error };

            if (input == null || string.IsNullOrEmpty(input.Email))
                return -1;

            if (this.Repository.UserRepository.Any(item => (item.Email ?? "").Trim().ToLower() == (input.Email ?? "").Trim().ToLower()))
            {
                actionResult.OperationStatus = UserEnum.EmailAlreadyExists;
                return -1;
            }

            IdentityFacade identityService = new IdentityFacade();
            identityService.AuthenticationManager = this.AuthenticationManager;

            var identityUser = new Domain.ApplicationUser()
            {
                UserName = this.UserName
            };

            string id = identityService.Add(identityUser, this.Password);
            if (string.IsNullOrEmpty(id))
            {
                actionResult.OperationStatus = UserEnum.Error;
                return -1;
            }

            Domain.User user = new Domain.User()
            {
                AuthId = id,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Gender = input.Gender,
                Address = input.Address,
                PostCode = input.PostCode,
                MobilePhoneNumber = input.MobilePhoneNumber,
                BirthDate = input.BirthDate,
                Email = input.Email.Trim().ToLower(),
                PhoneNumber = input.PhoneNumber,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now
            };

            this.Repository.UserRepository.Insert(user);

            if (actionResult.Success = this.Repository.Save())
            {
                actionResult.OperationStatus = UserEnum.Success;
                return user.Id;
            }

            return -1;

        }

        public override void Update(User input, ref ActionResult actionResult)
        {
            if (actionResult == null)
                actionResult = new ActionResult() { OperationStatus = UserEnum.Error };

            if (input == null)
                return;

            var user = this.Repository.UserRepository.FirstOrDefault(item => item.Id == input.Id);
            if (user == null)
            {
                actionResult.OperationStatus = UserEnum.InvalidUser;
                return;
            }

            bool identityDataChanged = (user.Email ?? "").Trim() != (input.Email ?? "").Trim();

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

            #region Email / Password changed
            if (identityDataChanged)
            {
                IdentityFacade identityService = new IdentityFacade();
                identityService.AuthenticationManager = this.AuthenticationManager;
                
                var identityUser = identityService.GetByKey(user.AuthId);
                if (identityUser == null)
                {
                    return;
                }

                identityUser.UserName = (input.Email ?? "").Trim();

                bool result = identityService.Update(identityUser, this.OldPassword, this.NewPassword);
                if (! result)
                {
                    return;
                }
            }
            #endregion

            if (actionResult.Success = this.Repository.Save())
            {
                actionResult.OperationStatus = UserEnum.Success;
            }
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
