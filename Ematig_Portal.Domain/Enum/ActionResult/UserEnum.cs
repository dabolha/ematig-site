using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.Domain.Enum.ActionResult
{
    public enum UserEnum
    {
        Error,
        Success,
        EmailAlreadyExists,
        InvalidCredentials,
        InvalidUser
    }
}
