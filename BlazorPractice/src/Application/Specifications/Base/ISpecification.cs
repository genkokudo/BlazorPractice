using BlazorPractice.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BlazorPractice.Application.Specifications.Base
{
    /// <summary>
    /// 検索仕様を定義するためのインタフェース
    /// QueryableExtensionsでIQueryableを拡張しているので、このインタフェースで絞り込み処理を行える
    /// （要するにWhere句に入れるものを保持している）
    /// </summary>
    /// <typeparam name="T">検索をするEntityクラス</typeparam>
    public interface ISpecification<T> where T : class, IEntity
    {
        /// <summary>
        /// 絞り込み基準
        /// Tに対して、各フィールドに対するWhere句の処理が定義されている
        /// Whereに突っ込んで使う
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// EFCoreのIncludeするテーブルリスト？
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// 文字列によるInclude指定だけどそんなことできるの？
        /// </summary>
        List<string> IncludeStrings { get; }
    }
}