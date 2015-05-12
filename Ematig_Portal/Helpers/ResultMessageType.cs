using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ematig_Portal.Helpers
{
    public enum ResultMessageType
    {
        SentMessageSuccess,
        ChangeInfoSuccess,
        RemoveLoginSuccess,
        RegisterUserSuccess,
        Error,
        InvalidCredentials,
        InvalidUser
    }
}