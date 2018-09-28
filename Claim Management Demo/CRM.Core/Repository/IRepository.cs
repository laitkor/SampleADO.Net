using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CRM.Core.Repository
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        TEntity Get(int id);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllInclude(params Expression<Func<TEntity, object>>[] includes);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void UpdateFields(TEntity entity, IEnumerable<string> fields);
        void Delete(IList<TEntity> entities);
        bool Activate(int id);
        IEnumerable<CRM.Core.Model.Claim> ClaimListPermissionWise();
        new void Dispose();
        void Save();
    }
}
