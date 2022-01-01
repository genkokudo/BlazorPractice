using BlazorPractice.Application.Interfaces.Services;
using System;

namespace BlazorPractice.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}