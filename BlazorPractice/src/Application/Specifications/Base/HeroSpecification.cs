using BlazorPractice.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BlazorPractice.Application.Specifications.Base
{
    /// <summary>
    /// このアプリの検索仕様
    /// QueryableExtensionsでIQueryableを拡張しているので、このインタフェースで絞り込み処理を行える
    /// </summary>
    /// <typeparam name="T">検索をするEntityクラス</typeparam>
    public abstract class HeroSpecification<T> : ISpecification<T> where T : class, IEntity
    {
        /// <summary>
        /// 絞り込み基準
        /// Tに対して、各フィールドに対するWhere句の処理が定義されている
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; set; }

        /// <summary>
        /// EFCoreのIncludeするテーブルリスト？
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
    }
}