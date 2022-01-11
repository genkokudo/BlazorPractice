using AutoMapper;
using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Domain.Contracts;
using BlazorPractice.Shared.Constants.Application;
using BlazorPractice.Shared.Wrapper;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId
{
    /// <summary>
    /// TEntityIdによるデータ取得とキャッシュ処理のクエリ
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntityId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TExtendedAttribute"></typeparam>
    public class GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequest<Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        public TEntityId EntityId { get; set; }

        public GetAllExtendedAttributesByEntityIdQuery(TEntityId entityId)
        {
            EntityId = entityId;
        }
    }

    internal class GetAllExtendedAttributesByEntityIdQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequestHandler<GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>, Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        private readonly IUnitOfWork<TId> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllExtendedAttributesByEntityIdQueryHandler(IUnitOfWork<TId> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> Handle(GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute> request, CancellationToken cancellationToken)
        {
            // 検索
            Func<Task<List<TExtendedAttribute>>> getAllExtendedAttributesByEntityId = () => _unitOfWork.Repository<TExtendedAttribute>().Entities.Where(x => x.EntityId.Equals(request.EntityId)).ToListAsync(cancellationToken);

            // キャッシュ
            var extendedAttributeList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllEntityExtendedAttributesByEntityIdCacheKey(typeof(TEntity).Name, request.EntityId), getAllExtendedAttributesByEntityId);

            // データ変換
            var mappedExtendedAttributes = _mapper.Map<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>(extendedAttributeList);
            return await Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>.SuccessAsync(mappedExtendedAttributes);
        }
    }
}