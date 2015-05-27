using Ematig_Portal.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.Domain.Interface
{
    public interface IIdentityControllerService
    {
        IAuthenticationManager AuthenticationManager { get; set; }

        Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser);

        Task<bool> SendTwoFactorCode(string provider);

        Task<string> GetVerifiedUserIdAsync();

        Task<bool> HasBeenVerified();

        Task<SignInStatus> PasswordSignIn(string userName, string password, bool isPersistent, bool shouldLockout);

        void LogOff();

        Task<string> Add(Domain.ApplicationUser identityUser, string password);

        Task<bool> Update(Domain.ApplicationUser identityUser, string oldPassword, string newPassword);

        Domain.ApplicationUser GetByKey(string key);

        Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUser user, UserManager<ApplicationUser> manager);
    }
}
