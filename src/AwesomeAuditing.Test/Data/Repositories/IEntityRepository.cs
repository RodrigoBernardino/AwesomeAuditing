using AwesomeAuditing.Test.Data.Utils;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AwesomeAuditing.Test.Data.Repositories
{
    public interface IEntityRepository<TEntity>
        where TEntity : class, IIdentifiableEntity, new()
    {
        [Audit(AuditOperation.Create), MethodImpl(MethodImplOptions.NoInlining)]
        TEntity Add(TEntity entity, [MethodIsAuditableParam]bool methodIsAuditable = true);

        [Audit(AuditOperation.Remove), MethodImpl(MethodImplOptions.NoInlining)]
        TEntity Remove(TEntity entity, [MethodIsAuditableParam]bool methodIsAuditable = true);

        [Audit(AuditOperation.Update), MethodImpl(MethodImplOptions.NoInlining)]
        TEntity Update(TEntity entity, [MethodIsAuditableParam]bool methodIsAuditable = true);

        IEnumerable<TEntity> FindAll();
    }
}
