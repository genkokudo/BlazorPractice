using AutoMapper;
using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Domain.Entities.Misc;
using BlazorPractice.Shared.Constants.Application;
using BlazorPractice.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorPractice.Application.Features.DocumentTypes.Commands.AddEdit
{
    public class AddEditDocumentTypeCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }

    internal class AddEditDocumentTypeCommandHandler : IRequestHandler<AddEditDocumentTypeCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditDocumentTypeCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditDocumentTypeCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditDocumentTypeCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditDocumentTypeCommand command, CancellationToken cancellationToken)
        {
            // リポジトリがなかったら作成して、そのキャッシュを取得
            if (await _unitOfWork.Repository<DocumentType>().Entities.Where(p => p.Id != command.Id)
                .AnyAsync(p => p.Name == command.Name, cancellationToken))
            {
                return await Result<int>.FailAsync(_localizer["Document type with this name already exists."]);
            }

            if (command.Id == 0)    // 追加の場合
            {
                var documentType = _mapper.Map<DocumentType>(command);                      // AutoMapperで変換
                await _unitOfWork.Repository<DocumentType>().AddAsync(documentType);        // リポジトリに対しデータ追加メソッドを呼ぶ
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);  // コミット
                return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Saved"]);
            }
            else    // 更新の場合、このデータはnull項目のみ更新する
            {
                var documentType = await _unitOfWork.Repository<DocumentType>().GetByIdAsync(command.Id);
                if (documentType != null)
                {
                    documentType.Name = command.Name ?? documentType.Name;                              // nullの場合のみ代入
                    documentType.Description = command.Description ?? documentType.Description;         // nullの場合のみ代入
                    await _unitOfWork.Repository<DocumentType>().UpdateAsync(documentType);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);
                    return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Document Type Not Found!"]);
                }
            }
        }
    }
}