using BlazorPractice.Domain.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorPractice.Application.Interfaces.Repositories
{
    // LazyCacheを使用
    // データベース呼び出しのキャッシュに使用する

    /// <summary>
    /// UnitOfWorkという実装パターン
    /// 複数のDB・DbContext（リポジトリ）等がある場合に
    /// それらを揃えて更新の整合性を保つ感じ。（片方が更新失敗しても大丈夫なようにする）
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IUnitOfWork<TId> : IDisposable
    {
        IRepositoryAsync<T, TId> Repository<T>() where T : AuditableEntity<TId>;    // TはAuditableEntityという監査拡張項目を追加するインタフェース

        /// <summary>
        /// SaveChangesAsync呼ぶだけ
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> Commit(CancellationToken cancellationToken);

        /// <summary>
        /// コミットして、
        /// LazyCacheというライブラリを使用したキャッシュをクリアする
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="cacheKeys">BlazorPractice.Shared.Constants.Application.ApplicationConstants.Cache</param>
        /// <returns></returns>
        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        /// <summary>
        /// dbContextのChangeTrackerを全てReloadする
        /// </summary>
        /// <returns></returns>
        Task Rollback();
    }
}