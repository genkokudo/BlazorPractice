using BlazorPractice.Application.Features.Dashboards.Queries.GetData;
using BlazorPractice.Shared.Wrapper;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.Dashboard
{
    public interface IDashboardManager : IManager
    {
        Task<IResult<DashboardDataResponse>> GetDataAsync();
    }
}