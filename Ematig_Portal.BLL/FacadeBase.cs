using Ematig_Portal.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.BLL
{
    public abstract class FacadeBase<_EntityKey, _DomainObject>
        where _DomainObject : DomainObject, new()
    {
        public EmatigBDContext Context { get; set; }

        public FacadeBase()
        {
            this.Context = new EmatigBDContext();
        }

        public FacadeBase(EmatigBDContext context)
        {
            this.Context = context;
        }

        public abstract _EntityKey Add(_DomainObject input, ref ActionResult actionResult);

        public abstract void Update(_DomainObject input, ref ActionResult actionResult);

        public abstract void Delete(_DomainObject input, ref ActionResult actionResult);

        public abstract _DomainObject GetByKey(_EntityKey key);

        public abstract ICollection<_DomainObject> Get();

        public void Dispose()
        {
            if (this.Context != null)
            {
                this.Context.Dispose();
            }
        }
    }
}
