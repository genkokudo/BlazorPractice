using BlazorPractice.Application.Specifications.Base;
using BlazorPractice.Infrastructure.Models.Audit;

namespace BlazorPractice.Infrastructure.Specifications
{
    /// <summary>
    /// 監査フィルター仕様
    /// 検索条件が無ければ全検索するという仕様を定義する
    /// </summary>
    public class AuditFilterSpecification : HeroSpecification<Audit>
    {
        /// <summary>
        /// 絞り込み基準を定義する
        /// userIdの中で、searchString,searchInOldValues,searchInNewValuesはOR条件
        /// </summary>
        /// <param name="userId">ユーザID</param>
        /// <param name="searchString">テーブル名に含む文字列</param>
        /// <param name="searchInOldValues">OldValuesに含む文字列</param>
        /// <param name="searchInNewValues">NewValuesに含む文字列</param>
        public AuditFilterSpecification(string userId, string searchString, bool searchInOldValues, bool searchInNewValues)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => (p.TableName.Contains(searchString) || searchInOldValues && p.OldValues.Contains(searchString) || searchInNewValues && p.NewValues.Contains(searchString)) && p.UserId == userId;
            }
            else
            {
                // 検索条件が無ければユーザIDに対して全検索
                Criteria = p => p.UserId == userId;
            }
        }
    }
}