﻿using BlazorPractice.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.Export;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetAll;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using BlazorPractice.Client.Infrastructure.Extensions;
using BlazorPractice.Domain.Contracts;
using BlazorPractice.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.ExtendedAttribute
{
    /// <summary>
    /// 多分、各エンティティに機能を持たせようとしている
    /// Excel出力とか
    /// </summary>
    /// <typeparam name="TId">AuditableEntityのID</typeparam>
    /// <typeparam name="TEntityId">IEntityのID</typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TExtendedAttribute"></typeparam>
    public class ExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
        : IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        private readonly HttpClient _httpClient;

        public ExtendedAttributeManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<string>> ExportToExcelAsync(ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute> request)
        {
            // 全検索かどうかでAPIのパスを取得して叩く
            // 検索文字列が空ならば、全検索のAPIを叩く
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(request.SearchString) && !request.IncludeEntity && !request.OnlyCurrentGroup
                ? Routes.ExtendedAttributesEndpoints.Export(typeof(TEntity).Name, request.EntityId, request.IncludeEntity, request.OnlyCurrentGroup, request.CurrentGroup)
                : Routes.ExtendedAttributesEndpoints.ExportFiltered(typeof(TEntity).Name, request.SearchString, request.EntityId, request.IncludeEntity, request.OnlyCurrentGroup, request.CurrentGroup));
            return await response.ToResult<string>();
        }

        public async Task<IResult<TId>> DeleteAsync(TId id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.ExtendedAttributesEndpoints.Delete(typeof(TEntity).Name)}/{id}");
            return await response.ToResult<TId>();
        }

        public async Task<IResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.ExtendedAttributesEndpoints.GetAll(typeof(TEntity).Name));
            return await response.ToResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>();
        }

        public async Task<IResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> GetAllByEntityIdAsync(TEntityId entityId)
        {
            var route = Routes.ExtendedAttributesEndpoints.GetAllByEntityId(typeof(TEntity).Name, entityId);
            var response = await _httpClient.GetAsync(route);
            return await response.ToResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>();
        }

        public async Task<IResult<TId>> SaveAsync(AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ExtendedAttributesEndpoints.Save(typeof(TEntity).Name), request);
            return await response.ToResult<TId>();
        }
    }
}