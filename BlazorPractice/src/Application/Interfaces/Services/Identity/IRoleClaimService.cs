using BlazorPractice.Application.Interfaces.Common;
using BlazorPractice.Application.Requests.Identity;
using BlazorPractice.Application.Responses.Identity;
using BlazorPractice.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPractice.Application.Interfaces.Services.Identity
{
    public interface IRoleClaimService : IService
    {
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

        Task<Result<string>> SaveAsync(RoleClaimRequest request);

        Task<Result<string>> DeleteAsync(int id);
    }
}