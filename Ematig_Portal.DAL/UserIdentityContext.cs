using Ematig_Portal.Domain;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ematig_Portal.Domain
{
    public class UserIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public UserIdentityContext() : base("UserIdentityDbContext")
        {
        }
    }
}