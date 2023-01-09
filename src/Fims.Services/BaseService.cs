using System.Linq;

using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Fims.Data;


namespace Fims.Services
{
    public abstract class BaseService<TEntity>
        where TEntity : class
    {
        protected BaseService(FimsDbContext dbContext, IMapper mapper)
        {
            //JBH: All services under Fims.Services will be given automatically by DI
            //     - the DB to work with
            //     - the Mapper
            this.TheDbContext = dbContext;
            this.Mapper = mapper;
        }

        protected FimsDbContext TheDbContext { get; }

        protected IMapper Mapper { get; }

        protected IQueryable<TEntity> All() => this.TheDbContext.Set<TEntity>();

        protected IQueryable<TEntity> AllAsNoTracking() => this.All().AsNoTracking();
    }
}
