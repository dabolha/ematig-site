using Ematig_Portal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.Domain.Interface
{
    public interface IControllerService<_EntityKey, _DomainObject>
    {
        _EntityKey Add(_DomainObject input, ref ActionResult actionResult);

        void Update(_DomainObject input, ref ActionResult actionResult);

        void Delete(_DomainObject input, ref ActionResult actionResult);

        _DomainObject GetByKey(_EntityKey key);

        _DomainObject GetByCustom(Expression<Func<_DomainObject, bool>> filter = null);

        ICollection<_DomainObject> Get();

        void Dispose();
    }
}
