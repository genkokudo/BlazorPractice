using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorPractice.Server.Controllers
{
    /// <summary>
    /// APIコントローラにMediatorとLoggerの実装を指示するために定義
    /// APIバージョン指定は機能しているのか謎
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController<T> : ControllerBase
    {
        private IMediator _mediatorInstance;
        private ILogger<T> _loggerInstance;
        protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
    }
}