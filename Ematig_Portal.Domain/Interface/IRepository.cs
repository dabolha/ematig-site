using System.Collections.Generic;
namespace Ematig_Portal.Domain.Interface
{
    public interface IRepository
    {
        void Add(object entity);
        void Update(object entity);
        void Delete(object entityKey);
        void Get(object entityKey);
        void Commit();
        void Rollback();
    }
}
