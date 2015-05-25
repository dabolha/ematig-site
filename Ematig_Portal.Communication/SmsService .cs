using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Ematig_Portal.Communication
{
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //TODO:Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

}