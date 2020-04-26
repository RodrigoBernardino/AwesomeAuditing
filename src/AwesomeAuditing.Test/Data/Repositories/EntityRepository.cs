using AwesomeAuditing.Test.Data.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AwesomeAuditing.Test.Data.Repositories
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IIdentifiableEntity, new()
    {
        public EntityRepository()
        {
            ContextCreator = () =>
            {
                var context = new Context();
                ConfigContext(context);
                return context;
            };
        }

        public Func<Context> ContextCreator { get; private set; }

        private static void ConfigContext(DbContext context)
        {
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Add entity to database without nested entity properties
        /// </summary>
        /// <param name="entity">the entity that will be inserted into the database</param>
        public virtual TEntity Add(TEntity entity, [MethodIsAuditableParam]bool methodIsAuditable = true)
        {
            using (var entityContext = ContextCreator())
            {
                TEntity addedEntity = entityContext.Set<TEntity>().Add(entity);
                entityContext.SaveChanges();

                return addedEntity.Id != 0 ? addedEntity : null;
            }
        }

        /// <summary>
        /// Remove entity from database
        /// </summary>
        /// <param name="entity">the entity that will be removed from the database</param>
        public virtual TEntity Remove(TEntity entity, [MethodIsAuditableParam]bool methodIsAuditable = true)
        {
            using (var entityContext = ContextCreator())
            {
                entityContext.Entry(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
                return entity;
            }
        }

        /// <summary>
        /// Update entity from database
        /// </summary>
        /// <param name="entity">the entity that will be updated into the database</param>
        public virtual TEntity Update(TEntity entity, [MethodIsAuditableParam]bool methodIsAuditable = true)
        {
            using (var entityContext = ContextCreator())
            {
                TEntity existingEntity =
                    entityContext.Set<TEntity>().FirstOrDefault(e => e.Id == entity.Id);

                if (existingEntity == null)
                    return null;

                SimpleMapper.PropertyMap(entity, existingEntity);

                entityContext.SaveChanges();
                return existingEntity;
            }
        }

        /// <summary>
        /// Fetch all the entities from the database table
        /// </summary>
        public virtual IEnumerable<TEntity> FindAll()
        {
            using (var entityContext = ContextCreator())
            {
                return entityContext.Set<TEntity>().ToList();
            }
        }
    }
}
