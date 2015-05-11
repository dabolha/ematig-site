using Ematig_Portal.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ematig_Portal.Controllers
{
    public class MessageController : Controller
    {
        #region Properties

        private EmatigBbContext _BbContext { get; set; }

        #endregion

        #region Constructor

        public MessageController()
        {
            this._BbContext = new EmatigBbContext();
        }

        #endregion
	}
}