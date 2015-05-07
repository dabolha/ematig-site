using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ematig_Portal.Models
{
    public class UserIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public UserIdentityDbContext()
            : base("DefaultConnection")
        {
        }
    }

    public class Context : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public Context()
            : base("DefaultConnection")
        {
        }
    }
}