using LMS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repository
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, string includeProperties = null);
        TEntity GetById(object id);
        TEntity GetByIdNoTracking(object id);

        void Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(object id);
        void DeleteMany(List<object> ids);
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        internal LmsDbContext _context;
        internal DbSet<TEntity> _dbSet;

        public Repository(LmsDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, string includeProperties = null) //, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
        {
            IQueryable<TEntity> query = _dbSet.Where(m => m.IsActive == true).OrderByDescending(m => m.CreatedAt);

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);

            return query.ToList();
        }
        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }
        public virtual TEntity GetByIdNoTracking(object id)
        {
            return _dbSet.AsNoTracking().First(x => x.Id == (int)id);
        }

        public virtual void Create(TEntity entity)
        {
            entity.CreatedAt = DateTime.Now;
            _dbSet.Add(entity);
        }
        public virtual void Update(TEntity entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _dbSet.Update(entity);
        }
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }
        public virtual void Delete(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }
        public void DeleteMany(List<object> ids)
        {
            List<TEntity> entities = new List<TEntity>();
            foreach (object id in ids)
            {
                TEntity entity = _dbSet.Find(id);
                entities.Add(entity);
            }
            _dbSet.RemoveRange(entities);
        }
    }
}
