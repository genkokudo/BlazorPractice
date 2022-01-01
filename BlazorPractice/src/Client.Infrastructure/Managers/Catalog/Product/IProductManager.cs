using BlazorPractice.Application.Features.Products.Commands.AddEdit;
using BlazorPractice.Application.Features.Products.Queries.GetAllPaged;
using BlazorPractice.Application.Requests.Catalog;
using BlazorPractice.Shared.Wrapper;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.Catalog.Product
{
    public interface IProductManager : IManager
    {
        Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request);

        Task<IResult<string>> GetProductImageAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditProductCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}