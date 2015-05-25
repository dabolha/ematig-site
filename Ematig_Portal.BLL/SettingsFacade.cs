using Ematig_Portal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.BLL
{
    public class SettingsFacade : FacadeBase<string, Domain.Settings>
    {
        public SettingsFacade() : base()
        {
        }

        public SettingsFacade(EmatigBDContext context)
            : base(context)
        {
        }

        public override string Add(Settings input, ref ActionResult actionResult)
        {
            throw new NotImplementedException();
        }

        public override void Update(Settings input, ref ActionResult actionResult)
        {
            if (actionResult == null)
                actionResult = new ActionResult();

            if (input == null)
                return;

            var setting = this.Context.Settings
                .FirstOrDefault(item => (item.Key ?? "").Trim().ToLower() == (input.Key ?? "").Trim().ToLower());

            if (setting == null)
                return;

            setting.Value = input.Value;

            actionResult.Success = this.Context.SaveChanges() > 0;
        }

        public override void Delete(Settings input, ref ActionResult actionResult)
        {
            throw new NotImplementedException();
        }

        public override Domain.Settings GetByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            return this.Context.Settings
                .FirstOrDefault(item => (item.Key ?? "").Trim().ToLower() == (key ?? "").Trim().ToLower());
        }

        public override ICollection<Domain.Settings> Get()
        {
            return this.Context.Settings.ToList();

        }
    }
}
