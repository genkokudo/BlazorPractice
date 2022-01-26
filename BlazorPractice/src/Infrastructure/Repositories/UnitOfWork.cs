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
    /// リポジトリが複数あるシステムの場合、片方が更新失敗しても大丈夫なように間に1クラス挟む
    /// </summary>
    /// <typeparam name="TId"></typeparam>

    public class UnitOfWork<TId> : IUnitOfWork<TId>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly BlazorHeroContext _dbContext;
        private bool disposed;
        private Hashtable _repositories;        // Key:Entityのクラス名, Value:対応するリポジトリクラスのインスタンス
        /// <summary>LazyCacheというライブラリを使用</summary>
        private readonly IAppCache _cache;  // BlazorPractice.Shared.Constants.Application.ApplicationConstants.Cacheで定義されているキーでキャッシュ

        public UnitOfWork(BlazorHeroContext dbContext, ICurrentUserService currentUserService, IAppCache cache)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _currentUserService = currentUserService;
            _cache = cache;
        }

        /// <summary>
        /// 型引数でEntityを指定して、そのリポジトリのインスタンスを作成し、キャッシュする
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IRepositoryAsync<TEntity, TId> Repository<TEntity>() where TEntity : AuditableEntity<TId>
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                // 指定されたリポジトリがない場合、インスタンスを作成してリポジトリリストに溜める
                var repositoryType = typeof(RepositoryAsync<,>);

                // _dbContextを持ったTEntityに対するリポジトリを作成
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TId)), _dbContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepositoryAsync<TEntity, TId>)_repositories[type];
        }

        public async Task<int> Commit(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

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
            if (!disposed)
            {
                if (disposing)
                {
                    // 管理しているリソースを破棄
                    _dbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}