using BlazorPractice.Application.Interfaces.Services;
using System;

namespace BlazorPractice.Infrastructure.Shared.Services
{
    /// <summary>
    /// 使用する時間を統一するためのサービス
    /// </summary>
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}