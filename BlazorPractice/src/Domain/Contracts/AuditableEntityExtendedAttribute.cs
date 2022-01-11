#nullable enable
using BlazorPractice.Domain.Enums;
using System;

namespace BlazorPractice.Domain.Contracts
{
    /// <summary>
    /// TEntityを型引数に、拡張項目を追加する
    /// 
    /// AuditableEntityを継承しているので、これを使うことで監査項目に関する処理を統一している
    /// （※で、ここに定義されているもの自体は何に使っているのかまだちゃんと見ていない）
    /// </summary>
    /// <typeparam name="TId">AuditableEntity（監査項目）のID</typeparam>
    /// <typeparam name="TEntityId">IEntityのID</typeparam>
    /// <typeparam name="TEntity">DBテーブルになっているEntityクラス</typeparam>
    public abstract class AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>
        : AuditableEntity<TId>, IEntityAuditableExtendedAttribute<TId, TEntityId, TEntity>
            where TEntity : IEntity<TEntityId>
    {
        /// <inheritdoc/>
        public TEntityId EntityId { get; set; }

        /// <summary>
        /// Extended attribute's related entity
        /// </summary>
        public virtual TEntity Entity { get; set; }

        /// <inheritdoc/>
        public EntityExtendedAttributeType Type { get; set; }

        /// <inheritdoc/>
        public string Key { get; set; }

        /// <inheritdoc/>
        public string? Text { get; set; }

        /// <inheritdoc/>
        public decimal? Decimal { get; set; }

        /// <inheritdoc/>
        public DateTime? DateTime { get; set; }

        /// <inheritdoc/>
        public string? Json { get; set; }

        /// <inheritdoc/>
        public string? ExternalId { get; set; }

        /// <inheritdoc/>
        public string? Group { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public bool IsActive { get; set; } = true;
    }
}