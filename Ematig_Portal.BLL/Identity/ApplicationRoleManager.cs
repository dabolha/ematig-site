using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.BLL.Identity
{
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        //public static ApplicationRoleManager Create(
        //    IdentityFactoryOptions<ApplicationRoleManager> options,
        //    IOwinContext context)
        //{
        //    var manager = new ApplicationRoleManager(
        //        new RoleStore<IdentityRole>(
        //            context.Get<UserIdentityDbContext>()));

        //    return manager;
        //}
    }

}
