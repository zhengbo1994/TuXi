using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public abstract class InfoEarthFrameRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<InfoEarthFrameDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected InfoEarthFrameRepositoryBase(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }


        public DbContext DbContext
        {
            get
            {
                return base.GetDbContext();
            }
        }
        //private virtual void RemoveHoldingEntityInContext(T entity)
        //{
        //    var objContext = InfoEarthFrameDbContext.ObjectContext;
        //    var objSet = objContext.CreateObjectSet<T>();
        //    var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

        //    Object foundEntity;
        //    var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);

        //    if (exists)
        //    {
        //        objContext.Detach(foundEntity);
        //    }
        //}

        //add common methods for all repositories
    }

    public abstract class InfoEarthFrameRepositoryBase<TEntity> : InfoEarthFrameRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected InfoEarthFrameRepositoryBase(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public DbContext DbContext
        {
            get
            {
                return base.GetDbContext();
            }
        }
        //do not add any method here, add to the class above (since this inherits it)
    }
}
