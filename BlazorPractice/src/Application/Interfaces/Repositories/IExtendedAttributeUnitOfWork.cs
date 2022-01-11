using BlazorPractice.Domain.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorPractice.Application.Interfaces.Repositories
{
    /// <summary>
    /// UnitOfWorkとは、複数のリポジトリの更新不整合を防止する仕組み
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntityId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface IExtendedAttributeUnitOfWork<TId, TEntityId, TEntity> : IDisposable where TEntity : AuditableEntity<TEntityId>
    {
        IRepositoryAsync<T, TId> Repository<T>() where T : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>;

        // 使ってない
        Task<int> Commit(CancellationToken cancellationToken);

        // DBの更新が行われるので、キャッシュを更新する
        /// <summary>
        /// DBのコミットと対象キャッシュの削除を行う
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="cacheKeys"></param>
        /// <returns></returns>
        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}