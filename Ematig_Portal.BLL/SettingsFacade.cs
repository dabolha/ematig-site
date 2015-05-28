using Ematig_Portal.DAL;
using Ematig_Portal.Domain;
using Ematig_Portal.Domain.Enum.ActionResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.BLL
{
    public class SettingsFacade : FacadeBase<string, Domain.Settings>
    {
        public SettingsFacade() : base()
        {
        }

        public SettingsFacade(UnitOfWork repository)
            : base(repository)
        {
        }

        public override string Add(Settings input, ref ActionResult actionResult)
        {
            throw new NotImplementedException();
        }

        public override void Update(Settings input, ref ActionResult actionResult)
        {
            if (actionResult == null)
                actionResult = new ActionResult() { OperationStatus = SettingsEnum.Error };

            if (input == null)
                return;

            var setting = this.Repository.SettingsRepository
                .FirstOrDefault(filter: item => (item.Key ?? "").Trim().ToLower() == (input.Key ?? "").Trim().ToLower());

            if (setting == null)
                return;

            setting.Value = input.Value;

            if (actionResult.Success = this.Repository.Save())
            {
                actionResult.OperationStatus = SettingsEnum.Success;
            }
        }

        public override void Delete(Settings input, ref ActionResult actionResult)
        {
            throw new NotImplementedException();
        }

        public override Domain.Settings GetByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            return this.Repository.SettingsRepository
                .FirstOrDefault(item => (item.Key ?? "").Trim().ToLower() == (key ?? "").Trim().ToLower());
        }

        public override Domain.Settings GetByCustom(Expression<Func<Domain.Settings, bool>> filter = null)
        {
            return this.Repository.SettingsRepository.FirstOrDefault(filter);
        }

        public override ICollection<Domain.Settings> Get()
        {
            return this.Repository.SettingsRepository.ToList();

        }
    }
}
