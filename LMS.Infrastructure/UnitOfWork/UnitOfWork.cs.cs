using LMS.Domain.Common;
using LMS.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : BaseEntity;
        void Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly LmsDbContext _dbContext;
        public UnitOfWork(LmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            return new Repository<T>(_dbContext);
        }
        public void Commit()
        {
            _dbContext.SaveChanges();
        }
    }
}
