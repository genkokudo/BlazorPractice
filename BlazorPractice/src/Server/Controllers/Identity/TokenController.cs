using BlazorPractice.Application.Interfaces.Services;
using BlazorPractice.Application.Interfaces.Services.Identity;
using BlazorPractice.Application.Requests.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlazorPractice.Server.Controllers.Identity
{
    [Route("api/identity/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _identityService;

        public TokenController(ITokenService identityService, ICurrentUserService currentUserService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Get Token (Email, Password)
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost]
        public async Task<ActionResult> Get(TokenRequest model)
        {
            var response = await _identityService.LoginAsync(model);
            return Ok(response);
        }

        /// <summary>
        /// トークンの更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
        {
            // 現在のトークンから新しいトークンを取得する
            var response = await _identityService.GetRefreshTokenAsync(model);
            return Ok(response);
        }
    }
}