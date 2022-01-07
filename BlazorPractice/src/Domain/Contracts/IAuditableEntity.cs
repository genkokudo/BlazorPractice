using System;

namespace BlazorPractice.Domain.Contracts
{
    /// <summary>
    /// EntityにIDと監査項目を持たせるインタフェース
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
    {
    }

    /// <summary>
    /// 監査項目をEntityに持たせる
    /// </summary>
    public interface IAuditableEntity : IEntity
    {
        string CreatedBy { get; set; }

        DateTime CreatedOn { get; set; }

        string LastModifiedBy { get; set; }

        DateTime? LastModifiedOn { get; set; }
    }
}