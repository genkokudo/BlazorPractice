using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Domain.Contracts;
using BlazorPractice.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPractice.Infrastructure.Repositories
{
    /// <summary>
    /// 各テーブルのCRUDを行うdbContextラッパー
    /// 型引数を使うことで、CRUDのコードを統一する
    /// コミットは行っていない
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <typeparam name="TId">IDの型</typeparam>
    public class RepositoryAsync<T, TId> : IRepositoryAsync<T, TId> where T : AuditableEntity<TId>
    {
        private readonly BlazorHeroContext _dbContext;

        public RepositoryAsync(BlazorHeroContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();

        /// <summary>
        /// レコード追加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// DBから対象レコードを削除
        /// </summary>
        /// <param name="entity">削除対象エンティティ</param>
        /// <returns></returns>
        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 全取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext
                .Set<T>()
                .ToListAsync();
        }

        /// <summary>
        /// IDを指定してデータ取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync(TId id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// ページングに対応したデータ取得、絞り込みは考慮していない
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// レコード更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task UpdateAsync(T entity)
        {
            T exist = _dbContext.Set<T>().Find(entity.Id);
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }
    }
}