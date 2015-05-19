using Ematig_Portal.Helpers;
using Ematig_Portal.Models;
using Ematig_Portal.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ematig_Portal.Controllers
{
    public class SettingsController : BaseController
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

                SettingsViewModel settingsModel = new SettingsViewModel();

                settingsModel.SettingList = settingsList
                    .Select(item => new SettingsModel()
                    {
                        Key = item.Key,
                        Name = item.Name,
                        Value = item.Value
                    })
                    .ToList();

                return View(settingsModel);
            }
        }

        public Settings Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            using (var context = this._BbContext)
            {
                return context.Settings
                    .FirstOrDefault(item => (item.Key ?? "").Trim().ToLower() == (key ?? "").Trim().ToLower());
            }
        }


        #endregion

        #region Edit

        //
        // GET: /Settings/Edit/key
        public ActionResult Edit(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("All");
            }

            using (var context = this._BbContext)
            {
                var setting = context.Settings
                    .FirstOrDefault(item => (item.Key ?? "").Trim().ToLower() == (key ?? "").Trim().ToLower());
                
                if (setting == null)
                {
                    Error(ProcessResultMessage(ResultMessageType.Error));
                    return RedirectToAction("All");
                }

                SettingsViewModel settingsModel = new SettingsViewModel();
                settingsModel.Setting = new SettingsModel()
                {
                    Key = setting.Key,
                    Name = setting.Name,
                    Value = setting.Value
                };

                return PartialView("_Edit", settingsModel);
            }
        }

        //
        // POST: /Settings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SettingsViewModel model)
        {
            if (!ModelState.IsValid 
                || model == null 
                || model.Setting == null 
                || string.IsNullOrEmpty(model.Setting.Key) 
                || string.IsNullOrEmpty(model.Setting.Value))
            {
                Error(ProcessResultMessage(ResultMessageType.Error));
                return RedirectToAction("All");
            }

            using (var context = this._BbContext)
            {
                var setting = context.Settings
                    .FirstOrDefault(item => (item.Key ?? "").Trim().ToLower() == (model.Setting.Key ?? "").Trim().ToLower());

                if (setting == null)
                {
                    Error(ProcessResultMessage(ResultMessageType.Error));
                    return RedirectToAction("All");
                }

                setting.Value = model.Setting.Value;

                if (context.SaveChanges() > 0)
                {
                    Success(ProcessResultMessage(ResultMessageType.OperationSuccess), true);
                    return RedirectToAction("All");
                }
                else
                {
                    Error(ProcessResultMessage(ResultMessageType.Error));
                }
            }

            return RedirectToAction("All");
        }

        #endregion
    }
}