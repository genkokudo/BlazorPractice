using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Application.Interfaces.Services;
using BlazorPractice.Domain.Contracts;
using BlazorPractice.Infrastructure.Contexts;
using LazyCache;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorPractice.Infrastructure.Repositories
{
    /// <summary>
    /// CRUD統一の際、DBの代わりにこれを呼んでいるけど、どういう仕組み？
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntityId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class ExtendedAttributeUnitOfWork<TId, TEntityId, TEntity> : IExtendedAttributeUnitOfWork<TId, TEntityId, TEntity> where TEntity : AuditableEntity<TEntityId>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly BlazorHeroContext _dbContext;
        private bool _disposed;
        private Hashtable _repositories;    // Entity名をキーとしたDbContextのインスタンス
        private readonly IAppCache _cache;  // LazyCacheというライブラリでキャッシュをしている

        public ExtendedAttributeUnitOfWork(BlazorHeroContext dbContext, ICurrentUserService currentUserService, IAppCache cache)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _currentUserService = currentUserService;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">監査用に拡張したEntity</typeparam>
        /// <returns></returns>
        public IRepositoryAsync<T, TId> Repository<T>() where T : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(RepositoryAsync<,>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T), typeof(TId)), _dbContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepositoryAsync<T, TId>)_repositories[type];
        }

        /// <summary>
        /// SaveChangesAsyncをする
        /// CommitAndRemoveCacheを使ってるので、こっちは呼ばれていない
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> Commit(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// SaveChangesAsyncをしてキャッシュをクリア
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="cacheKeys">クリアするキャッシュ</param>
        /// <returns></returns>
        public async Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            foreach (var cacheKey in cacheKeys)
            {
                _cache.Remove(cacheKey);
            }
            return result;
        }

        public Task Rollback()
        {
            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _dbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            _disposed = true;
        }
    }
}