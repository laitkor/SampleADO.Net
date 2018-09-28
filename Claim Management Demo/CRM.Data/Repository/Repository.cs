using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CRM.Core.Repository;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using CRM.Common.Security;

namespace CRM.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly CRMDbContext _context;
        public Repository(CRMDbContext context)
        {
            _context = context;
        }
        public TEntity Get(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }
        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }
        public IQueryable<TEntity> GetAllInclude(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> retVal = _context.Set<TEntity>();
            if (includes != null)
            {
                retVal = includes.Aggregate(retVal, (current, include) => current.Include(include));
            }
            return retVal;
        }
        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }
        public void Add(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Added;
        }
        public void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Added;
            }
        }
        public void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        public void UpdateFields(TEntity entity, IEnumerable<string> fields)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            foreach (var name in entry.CurrentValues.PropertyNames.Except(fields))
            {
                entry.Property(name).IsModified = false;
            }
        }
        public void Delete(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }
        public void Delete(IList<TEntity> entities)
        {
            if (entities == null || !entities.Any())
                return;
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }
        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                var outputLines = new List<string>();
                foreach (var error in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format(
                        "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
                        DateTime.Now, error.Entry.Entity.GetType().Name, error.Entry.State));
                    outputLines.AddRange(error.ValidationErrors.Select(ve => string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage)));
                }
                throw new Exception(string.Join(";", outputLines));
            }
        }
        public bool Activate(int id)
        {


            ObjectContext objectContext = ((IObjectContextAdapter)_context).ObjectContext;
            string sql = objectContext.CreateObjectSet<TEntity>().ToTraceString();
            Regex regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value.Replace("[dbo].[", "").Replace("]", "");
            var oMyString = new ObjectParameter("result", typeof(int));

            _context.CheckForActivate(table, id, oMyString);
            return int.Parse(oMyString.Value.ToString()) == 0 ? false : true;

        }
        //GEt all claim permission on basis of userid and client id
        public IEnumerable<CRM.Core.Model.Claim> ClaimListPermissionWise()
        {
            var clm = _context.GetUserAssignedClaim(AuthUser.User.Roles, AuthUser.User.UserId, AuthUser.User.PortalId);

            List<int> AllAssignedClaim = new List<int>();

            List<CRM.Core.Model.GetUserAssignedClaim_Result> Resultcpy = new List<CRM.Core.Model.GetUserAssignedClaim_Result>(clm);

            AllAssignedClaim = Resultcpy.Select(x => x.ClaimId).ToList();


            List<CRM.Core.Model.Claim> AllClaimList = _context.Claims.Where(x => AllAssignedClaim.Contains(x.ClaimID)).ToList();



            List<CRM.Core.Model.Claim> ClaimList = AllClaimList.ToList();

            return ClaimList;
        }




        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
