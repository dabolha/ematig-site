using Ematig_Portal.DAL;
using Ematig_Portal.Domain;
using Ematig_Portal.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.BLL
{
    public abstract class FacadeBase<_EntityKey, _DomainObject> : IControllerService<_EntityKey, _DomainObject>
        where _DomainObject : DomainObject, new()
    {
        protected UnitOfWork Repository { get; set; }

        public FacadeBase()
        {
            this.Repository = new UnitOfWork();
        }

        public FacadeBase(UnitOfWork repository)
        {
            this.Repository = repository;
        }

        public abstract _EntityKey Add(_DomainObject input, ref ActionResult actionResult);

        public abstract void Update(_DomainObject input, ref ActionResult actionResult);

        public abstract void Delete(_DomainObject input, ref ActionResult actionResult);

        public abstract _DomainObject GetByKey(_EntityKey key);

        public abstract _DomainObject GetByCustom(Expression<Func<_DomainObject, bool>> filter = null);

        public abstract ICollection<_DomainObject> Get();

        public void Dispose()
        {
            if (this.Repository != null)
            {
                this.Repository.Dispose();
            }
        }

    }
}
