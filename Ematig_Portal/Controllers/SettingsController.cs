using Ematig_Portal.Models;
using Ematig_Portal.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ematig_Portal.Controllers
{
    public class SettingsController : Controller
    {
        #region Properties

        private EmatigBbContext _BbContext { get; set; }

        #endregion

        #region Constructor

        public SettingsController()
        {
            this._BbContext = new EmatigBbContext();
        }

        #endregion

        #region Get

        //
        // GET: /Settings/All
        public ActionResult All()
        {
            using (var context = this._BbContext)
            {
                var settingsList = context.Settings.ToList();
                if (settingsList == null || settingsList.Count == 0)
                {
                    return View();
                }

                IEnumerable<SettingsViewModel> list = settingsList
                    .Select(item => new SettingsViewModel()
                    {
                        Key = item.Key,
                        Name = item.Name,
                        Value = item.Value
                    })
                    .ToList();

                return View(list);
            }
        }

        #endregion

        //
        // GET: /Settings/Edit
        public ActionResult Edit(string key)
        {
            throw new NotImplementedException();
        }

        //
        // POST: /Settings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SettingsViewModel model)
        {
            throw new NotImplementedException();
        }

	}
}