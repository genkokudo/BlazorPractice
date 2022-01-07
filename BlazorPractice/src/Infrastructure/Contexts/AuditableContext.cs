using BlazorPractice.Application.Enums;
using BlazorPractice.Infrastructure.Models.Audit;
using BlazorPractice.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorPractice.Infrastructure.Contexts
{
    // CreatedByとかを扱うのはここではない

    /// <summary>
    /// 監査関係の機能を持たせたDbContext
    /// 標準ライブラリのユーザ管理DbContextを継承する
    /// </summary>
    public abstract class AuditableContext : IdentityDbContext<BlazorHeroUser, BlazorHeroRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, BlazorHeroRoleClaim, IdentityUserToken<string>>
    {
        protected AuditableContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// 更新テーブル、項目、値の履歴を記録する
        /// </summary>
        public DbSet<Audit> AuditTrails { get; set; }

        public virtual async Task<int> SaveChangesAsync(string userId = null, CancellationToken cancellationToken = new())
        {
            var auditEntries = OnBeforeSaveChanges(userId); // TemporaryPropertiesを持つ変更情報だけを集める
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries, cancellationToken);
            return result;
        }

        /// <summary>
        /// 保存前処理
        /// 変更情報を集める
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>TemporaryPropertiesを持つ変更情報のリスト</returns>
        private List<AuditEntry> OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();  // 変更情報のリスト
            foreach (var entry in ChangeTracker.Entries())
            {
                // 変更履歴テーブルの更新ではこの処理は行わない
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                // 変更された行に対して、そのテーブル・列・値を記録する
                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = userId
                };
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // SaveChangesが呼び出されたときにストアから生成された値で置き換えられる一時的な値だった場合
                        // リストに追加する
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                // DB用のデータに直す
                AuditTrails.Add(auditEntry.ToAudit());
            }
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        /// <summary>
        /// 保存後処理
        /// ストアから生成された値で置き換えられた値を修正し（よくわからない）
        /// 変更追跡情報をテーブルに書き込む
        /// </summary>
        /// <param name="auditEntries">値の変更リスト</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken = new())
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                // おそらく、SaveChangesが呼び出されたときにストアから生成された一時的な値で置き換えられる場合があり、
                // その修正作業を行っている
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        // 主キーの場合
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        // 主キーでない場合
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                AuditTrails.Add(auditEntry.ToAudit());
            }
            return SaveChangesAsync(cancellationToken);
        }
    }
}