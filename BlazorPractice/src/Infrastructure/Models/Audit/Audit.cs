using BlazorPractice.Domain.Contracts;
using System;

namespace BlazorPractice.Infrastructure.Models.Audit
{
    /// <summary>
    /// 更新したテーブルと項目を記録する
    /// 1つのテーブルに、複数のカラム名と値をJSONで持つ
    /// </summary>
    public class Audit : IEntity<int>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        /// <summary>JSONまたはnull</summary>
        public string OldValues { get; set; }
        /// <summary>JSONまたはnull</summary>
        public string NewValues { get; set; }
        /// <summary>この更新で変更した項目、JSONまたはnull</summary>
        public string AffectedColumns { get; set; }
        /// <summary>主キー情報はJSONで持つ</summary>
        public string PrimaryKey { get; set; }
    }
}