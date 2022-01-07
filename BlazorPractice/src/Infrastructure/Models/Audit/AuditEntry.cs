using BlazorPractice.Application.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorPractice.Infrastructure.Models.Audit
{
    /// <summary>
    /// 監査対象テーブルの値変更履歴
    /// </summary>
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string UserId { get; set; }
        public string TableName { get; set; }
        /// <summary>主キー変更の項目名と値</summary>
        public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object> OldValues { get; } = new();
        /// <summary>主キーでない項目変更の項目名と値</summary>
        public Dictionary<string, object> NewValues { get; } = new();
        /// <summary>SaveChangesが呼び出されたときにストアから生成された値で置き換えられる一時的な値とみなされたプロパティのリスト</summary>
        public List<PropertyEntry> TemporaryProperties { get; } = new();
        public AuditType AuditType { get; set; }
        /// <summary>1つの更新に対し変更した項目のリスト</summary>
        public List<string> ChangedColumns { get; } = new();
        public bool HasTemporaryProperties => TemporaryProperties.Any();

        /// <summary>
        /// DB用のデータに変換する
        /// </summary>
        /// <returns></returns>
        public Audit ToAudit()
        {
            var audit = new Audit
            {
                UserId = UserId,
                Type = AuditType.ToString(),
                TableName = TableName,
                DateTime = DateTime.UtcNow,
                PrimaryKey = JsonConvert.SerializeObject(KeyValues),
                OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues),
                AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns)
            };
            return audit;
        }
    }
}