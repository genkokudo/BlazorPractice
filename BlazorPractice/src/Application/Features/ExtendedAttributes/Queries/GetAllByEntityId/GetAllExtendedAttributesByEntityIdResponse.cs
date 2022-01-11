#nullable enable
using BlazorPractice.Domain.Contracts;
using BlazorPractice.Domain.Enums;
using System;

namespace BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId
{
    // TIdはAuditableEntityのID、TEntityIdはIEntityのIDなので両方とも一意だと思うんだけど、何故両方持たせるのか分からない

    public class GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId> : IEntityAuditableExtendedAttribute
    {
        public TId Id { get; set; }
        public TEntityId EntityId { get; set; }
        public EntityExtendedAttributeType Type { get; set; }
        public string Key { get; set; }
        public string? Text { get; set; }
        public decimal? Decimal { get; set; }
        public DateTime? DateTime { get; set; }
        public string? Json { get; set; }
        public string? ExternalId { get; set; }
        public string? Group { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}