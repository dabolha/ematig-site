using Ematig_Portal.Domain.Enum;
using Ematig_Portal.Models;
using System.Linq;
using System.Web.Mvc;

namespace Ematig_Portal.Controllers
{
    public class SettingsController : BaseController
    {
        #region Get

        //
        // GET: /Settings/All
        public ActionResult All()
        {
            var settingsList = this.SettingsService.Get();
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

        #endregion

        #region Edit

        //
        // GET: /Settings/Edit/key
        public ActionResult Edit(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                ProcessResult(null, ResultMessageType.Error);
                LogError();
                return RedirectToAction("All");
            }

            var setting = this.SettingsService.GetByKey(key);
            if (setting == null)
            {
                ProcessResult(null, ResultMessageType.Error);
                LogError();
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
                ProcessResult(null, ResultMessageType.Error);
                LogError();
                return RedirectToAction("All");
            }

            var setting = this.SettingsService.GetByKey(model.Setting.Key);
            if (setting == null)
            {
                ProcessResult(null, ResultMessageType.Error);
                LogError();
                return RedirectToAction("All");
            }

            setting.Value = model.Setting.Value;

            Domain.ActionResult actionResult = null;

            this.SettingsService.Update(setting, ref actionResult);
            if (actionResult == null || ! actionResult.Success)
            {
                ProcessResult(null, ResultMessageType.Error);
                LogError();
                return RedirectToAction("All");
            }
            else
            {
                ProcessResult(actionResult, ResultMessageType.OperationSuccess);
                return RedirectToAction("All");
            }

            return RedirectToAction("All");
        }

        #endregion
    }
}