using AutoMapper;
using BlazorPractice.Application.Interfaces.Services;
using BlazorPractice.Application.Interfaces.Services.Identity;
using BlazorPractice.Application.Requests.Identity;
using BlazorPractice.Application.Responses.Identity;
using BlazorPractice.Infrastructure.Contexts;
using BlazorPractice.Infrastructure.Models.Identity;
using BlazorPractice.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPractice.Infrastructure.Services.Identity
{
    /// <summary>
    /// RoleClaimsテーブルに問い合わせる
    /// </summary>
    public class RoleClaimService : IRoleClaimService
    {
        private readonly IStringLocalizer<RoleClaimService> _localizer;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly BlazorHeroContext _db;

        public RoleClaimService(
            IStringLocalizer<RoleClaimService> localizer,
            IMapper mapper,
            ICurrentUserService currentUserService,
            BlazorHeroContext db)
        {
            _localizer = localizer;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _db = db;
        }

        /// <summary>
        /// RoleClaim全レコード
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<RoleClaimResponse>>> GetAllAsync()
        {
            var roleClaims = await _db.RoleClaims.ToListAsync();
            var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        /// <summary>
        /// RoleClaim件数
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCountAsync()
        {
            var count = await _db.RoleClaims.CountAsync();
            return count;
        }

        /// <summary>
        /// IDによるRoleClaim取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<RoleClaimResponse>> GetByIdAsync(int id)
        {
            var roleClaim = await _db.RoleClaims
                .SingleOrDefaultAsync(x => x.Id == id);
            var roleClaimResponse = _mapper.Map<RoleClaimResponse>(roleClaim);
            return await Result<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }

        /// <summary>
        /// roleIdによる全ロール取得
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId)
        {
            var roleClaims = await _db.RoleClaims
                .Include(x => x.Role)
                .Where(x => x.RoleId == roleId)
                .ToListAsync();
            var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        /// <summary>
        /// ロール更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<string>> SaveAsync(RoleClaimRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RoleId))
            {
                return await Result<string>.FailAsync(_localizer["Role is required."]);
            }

            if (request.Id == 0)
            {
                var existingRoleClaim =
                    await _db.RoleClaims
                        .SingleOrDefaultAsync(x =>
                            x.RoleId == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value);
                if (existingRoleClaim != null)
                {
                    return await Result<string>.FailAsync(_localizer["Similar Role Claim already exists."]);
                }
                var roleClaim = _mapper.Map<BlazorHeroRoleClaim>(request);
                await _db.RoleClaims.AddAsync(roleClaim);
                await _db.SaveChangesAsync(_currentUserService.UserId);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} created."], request.Value));
            }
            else
            {
                var existingRoleClaim =
                    await _db.RoleClaims
                        .Include(x => x.Role)
                        .SingleOrDefaultAsync(x => x.Id == request.Id);
                if (existingRoleClaim == null)
                {
                    return await Result<string>.SuccessAsync(_localizer["Role Claim does not exist."]);
                }
                else
                {
                    existingRoleClaim.ClaimType = request.Type;
                    existingRoleClaim.ClaimValue = request.Value;
                    existingRoleClaim.Group = request.Group;
                    existingRoleClaim.Description = request.Description;
                    existingRoleClaim.RoleId = request.RoleId;
                    _db.RoleClaims.Update(existingRoleClaim);
                    await _db.SaveChangesAsync(_currentUserService.UserId);
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for Role {1} updated."], request.Value, existingRoleClaim.Role.Name));
                }
            }
        }

        /// <summary>
        /// ロール削除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<string>> DeleteAsync(int id)
        {
            var existingRoleClaim = await _db.RoleClaims
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingRoleClaim != null)
            {
                _db.RoleClaims.Remove(existingRoleClaim);
                await _db.SaveChangesAsync(_currentUserService.UserId);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for {1} Role deleted."], existingRoleClaim.ClaimValue, existingRoleClaim.Role.Name));
            }
            else
            {
                return await Result<string>.FailAsync(_localizer["Role Claim does not exist."]);
            }
        }
    }
}