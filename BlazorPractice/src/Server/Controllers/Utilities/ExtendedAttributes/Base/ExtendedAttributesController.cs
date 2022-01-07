using BlazorPractice.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorPractice.Application.Features.ExtendedAttributes.Commands.Delete;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.Export;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetAll;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using BlazorPractice.Application.Features.ExtendedAttributes.Queries.GetById;
using BlazorPractice.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BlazorPractice.Server.Controllers.Utilities.ExtendedAttributes.Base
{
    /// <summary>
    /// 抽象拡張属性コントローラクラス
    /// 
    /// CRUDはこれでまとめている。継承して型引数にEntityを入れるだけでAPIができる
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ExtendedAttributesController<TId, TEntityId, TEntity, TExtendedAttribute>
        : BaseApiController<ExtendedAttributesController<TId, TEntityId, TEntity, TExtendedAttribute>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        /// <summary>
        /// 全検索
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var extendedAttributes = await _mediator.Send(new GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>());
            return Ok(extendedAttributes);
        }

        /// <summary>
        /// Get All Entity Extended Attributes by entity id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>Status 200 OK</returns>
        [HttpGet("by-entity/{entityId}")]
        public virtual async Task<IActionResult> GetAllByEntityId(TEntityId entityId)
        {
            var extendedAttributes = await _mediator.Send(new GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>(entityId));
            return Ok(extendedAttributes);
        }

        /// <summary>
        /// IDによる検索
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(TId id)
        {
            var extendedAttribute = await _mediator.Send(new GetExtendedAttributeByIdQuery<TId, TEntityId, TEntity, TExtendedAttribute> { Id = id });
            return Ok(extendedAttribute);
        }

        /// <summary>
        /// 新規登録と更新
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost]
        public virtual async Task<IActionResult> Post(AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TId id)
        {
            return Ok(await _mediator.Send(new DeleteExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> { Id = id }));
        }

        /// <summary>
        /// エンティティ拡張属性の検索とExcelへのエクスポート
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="entityId"></param>
        /// <param name="includeEntity"></param>
        /// <param name="onlyCurrentGroup"></param>
        /// <param name="currentGroup"></param>
        /// <returns>DataにExcelファイルデータのByte配列を入れる</returns>
        [HttpGet("export")]
        public virtual async Task<IActionResult> Export(string searchString = "", TEntityId entityId = default, bool includeEntity = false, bool onlyCurrentGroup = false, string currentGroup = "")
        {
            return Ok(await _mediator.Send(new ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>(searchString, entityId, includeEntity, onlyCurrentGroup, currentGroup)));
        }
    }
}