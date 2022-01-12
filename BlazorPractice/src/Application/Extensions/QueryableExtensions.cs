using BlazorPractice.Application.Exceptions;
using BlazorPractice.Application.Specifications.Base;
using BlazorPractice.Domain.Contracts;
using BlazorPractice.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPractice.Application.Extensions
{
    /// <summary>
    /// IQueryableを拡張する
    /// </summary>
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            if (source == null) throw new ApiException();
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            int count = await source.CountAsync();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// 検索フィルタ仕様に従って絞り込む
        /// Where句をオブジェクトに保持しておいてSpecifyで適用する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="spec">絞り込み方法の情報</param>
        /// <returns></returns>
        public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class, IEntity
        {
            // Aggregateは第2引数に従って集計する。currentは現在の結果、includeは配列の次の値
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(query,
                    (current, include) => current.Include(include));    // 多分EFCoreのInclude処理。
            var secondaryResult = spec.IncludeStrings                   // 上の結果に対して更にInclude処理。文字列でInclude指定しているけどそんなことできるの？
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));
            return secondaryResult.Where(spec.Criteria);                // 仕様に従って絞り込む
        }
    }
}