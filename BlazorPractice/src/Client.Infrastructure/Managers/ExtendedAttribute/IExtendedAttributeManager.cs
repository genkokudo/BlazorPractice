﻿using BlazorPractice.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.Export;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetAll;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using BlazorPractice.Domain.Contracts;
using BlazorPractice.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.ExtendedAttribute
{
    /// <summary>
    /// 各エンティティに共通の機能を持たせようとしている？
    /// WebAssemblyHostBuilderExtensionsで登録させて、DIできるようにしている？
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntityId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TExtendedAttribute"></typeparam>
    public interface IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
        where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
        where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
        where TId : IEquatable<TId>
    {
        Task<IResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>> GetAllAsync();

        Task<IResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> GetAllByEntityIdAsync(TEntityId entityId);

        Task<IResult<TId>> SaveAsync(AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> request);

        Task<IResult<TId>> DeleteAsync(TId id);

        /// <summary>
        /// Excelデータにして出力する
        /// </summary>
        /// <param name="request"></param>
        /// <returns>response.DataがExcelデータのByteArray</returns>
        Task<IResult<string>> ExportToExcelAsync(ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute> request);
    }
}