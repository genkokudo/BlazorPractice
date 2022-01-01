using BlazorPractice.Application.Features.Documents.Commands.AddEdit;
using BlazorPractice.Application.Features.Documents.Queries.GetAll;
using BlazorPractice.Application.Features.Documents.Queries.GetById;
using BlazorPractice.Application.Requests.Documents;
using BlazorPractice.Shared.Wrapper;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.Misc.Document
{
    public interface IDocumentManager : IManager
    {
        Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllPagedDocumentsRequest request);

        Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request);

        Task<IResult<int>> SaveAsync(AddEditDocumentCommand request);

        Task<IResult<int>> DeleteAsync(int id);
    }
}