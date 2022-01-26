using System;

namespace BlazorPractice.Domain.Contracts
{
    /// <summary>
    /// IDと監査項目を持たせているEntityの抽象クラス
    /// 
    /// でもこれだとDBのプライマリキー指定[Key]は？→IdがPKeyになるのがEFの仕様
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}