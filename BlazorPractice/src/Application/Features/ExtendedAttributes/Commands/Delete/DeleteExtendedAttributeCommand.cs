using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Domain.Contracts;
using BlazorPractice.Shared.Constants.Application;
using BlazorPractice.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// IDによるEntity削除
// QueryではなくCommandという名前だけど、これもMediator。

// UnitOfWorkの考え方でリポジトリに削除指示を送る形なので、普通よりややこしい
namespace BlazorPractice.Application.Features.ExtendedAttributes.Commands.Delete
{
    internal class DeleteExtendedAttributeCommandLocalization
    {
        // for localization
    }

    public class DeleteExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequest<Result<TId>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        public TId Id { get; set; }
    }

    internal class DeleteExtendedAttributeCommandHandler<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequestHandler<DeleteExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute>, Result<TId>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        private readonly IStringLocalizer<DeleteExtendedAttributeCommandLocalization> _localizer;
        private readonly IExtendedAttributeUnitOfWork<TId, TEntityId, TEntity> _unitOfWork;

        public DeleteExtendedAttributeCommandHandler(
            IExtendedAttributeUnitOfWork<TId, TEntityId, TEntity> unitOfWork,
            IStringLocalizer<DeleteExtendedAttributeCommandLocalization> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        /// <summary>
        /// 対象レコードの削除指示
        /// キャッシュのクリア（ApplicationConstants.Cacheでキャッシュのキーを発行）
        /// コミット
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result<TId>> Handle(DeleteExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> command, CancellationToken cancellationToken)
        {
            // DBからIDで対象を取得
            var extendedAttribute = await _unitOfWork.Repository<TExtendedAttribute>().GetByIdAsync(command.Id);
            if (extendedAttribute != null)
            {
                // DbContextに対象の削除指示する
                await _unitOfWork.Repository<TExtendedAttribute>().DeleteAsync(extendedAttribute);

                // 削除されたエンティティ拡張属性に関連するすべてのキャッシュを削除する。
                // エンティティ名とIDでキャッシュキーを取得して、そのキーで処理対象の一覧を作成
                var cacheKeys = await _unitOfWork.Repository<TExtendedAttribute>().Entities.Select(x =>
                    ApplicationConstants.Cache.GetAllEntityExtendedAttributesByEntityIdCacheKey(
                        typeof(TEntity).Name, x.Entity.Id)).Distinct().ToListAsync(cancellationToken);          // この方法のキャッシュは、EntityIdで検索した時に行われている

                // エンティティのキャッシュキーを取得して、キャッシュ更新対象に加える
                // ※キャッシュ更新は、ここで対象キャッシュをクリアし、次に全検索などで読み出す時にキャッシュが無ければ読み込まれるという仕組み
                cacheKeys.Add(ApplicationConstants.Cache.GetAllEntityExtendedAttributesCacheKey(typeof(TEntity).Name));

                // 処理対象のキー一覧を送って、キャッシュの更新を指示。コミットも行う
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, cacheKeys.ToArray());

                return await Result<TId>.SuccessAsync(extendedAttribute.Id, _localizer["Extended Attribute Deleted"]);
            }
            else
            {
                // 無かったらエラー
                return await Result<TId>.FailAsync(_localizer["Extended Attribute Not Found!"]);
            }
        }
    }
}