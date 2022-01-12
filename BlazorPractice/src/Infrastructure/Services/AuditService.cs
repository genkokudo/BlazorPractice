using AutoMapper;
using BlazorPractice.Application.Extensions;
using BlazorPractice.Application.Interfaces.Services;
using BlazorPractice.Application.Responses.Audit;
using BlazorPractice.Infrastructure.Contexts;
using BlazorPractice.Infrastructure.Models.Audit;
using BlazorPractice.Infrastructure.Specifications;
using BlazorPractice.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPractice.Infrastructure.Services
{
    /// <summary>
    /// 監査（更新テーブル、項目、値の履歴）
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<AuditService> _localizer;

        public AuditService(
            IMapper mapper,
            BlazorHeroContext context,
            IExcelService excelService,
            IStringLocalizer<AuditService> localizer)
        {
            _mapper = mapper;
            _context = context;
            _excelService = excelService;
            _localizer = localizer;
        }

        /// <summary>
        /// 現在のユーザの監査基準（更新テーブル、項目、値の履歴）を取得する
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId)
        {
            var trails = await _context.AuditTrails.Where(a => a.UserId == userId).OrderByDescending(a => a.Id).Take(250).ToListAsync();
            var mappedLogs = _mapper.Map<List<AuditResponse>>(trails);
            return await Result<IEnumerable<AuditResponse>>.SuccessAsync(mappedLogs);
        }

        /// <summary>
        /// Excel出力
        /// searchString,searchInOldValues,searchInNewValuesはOR条件
        /// </summary>
        /// <param name="userId">ユーザID</param>
        /// <param name="searchString"></param>
        /// <param name="searchInOldValues"></param>
        /// <param name="searchInNewValues"></param>
        /// <returns></returns>
        public async Task<IResult<string>> ExportToExcelAsync(string userId, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false)
        {
            // 検索条件が無ければ全検索というフィルタ仕様を作成
            var auditSpec = new AuditFilterSpecification(userId, searchString, searchInOldValues, searchInNewValues);
            var trails = await _context.AuditTrails
                .Specify(auditSpec)     // フィルタ仕様に従って検索（決まったWhere句を適用）する
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
            var data = await _excelService.ExportAsync(trails, sheetName: _localizer["Audit trails"],
                mappers: new Dictionary<string, Func<Audit, object>>
                {
                    { _localizer["Table Name"], item => item.TableName },
                    { _localizer["Type"], item => item.Type },
                    { _localizer["Date Time (Local)"], item => DateTime.SpecifyKind(item.DateTime, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["Date Time (UTC)"], item => item.DateTime.ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["Primary Key"], item => item.PrimaryKey },
                    { _localizer["Old Values"], item => item.OldValues },
                    { _localizer["New Values"], item => item.NewValues },
                });

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}