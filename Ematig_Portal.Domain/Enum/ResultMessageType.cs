﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ematig_Portal.Domain.Enum
{
    public enum ResultMessageType
    {
        SentMessageSuccess,
        ChangeInfoSuccess,
        RemoveLoginSuccess,
        RegisterUserSuccess,
        EmailAlreadyExists,
        OperationSuccess,
        Error,
        InvalidCredentials,
        InvalidUser
    }
}