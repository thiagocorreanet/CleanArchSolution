using Core.Entities;
using Core.Enums;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Newtonsoft.Json;

namespace Core.Entities;

public class AuditProcess : BaseEntity
{
    public AuditProcess() { }

    public string Type { get; set; } = null!;
    public string Table { get; set; } = null!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }

    public static AuditProcess CreateFromEntity(EntityEntry entry)
    {
        var audit = new AuditProcess
        {
            Table = entry.Entity.GetType().Name,
            PrimaryKey = JsonConvert.SerializeObject(GetPrimaryKeyValues(entry)),
            Type = GetAuditType(entry),
            OldValues = GetOldValues(entry),
            NewValues = GetNewValues(entry),
            AffectedColumns = GetChangedColumns(entry)
        };

        return audit;
    }

    private static string GetAuditType(EntityEntry entry)
    {
        return entry.State switch
        {
            EntityState.Added => EAuditType.Create.ToString(),
            EntityState.Deleted => EAuditType.Delete.ToString(),
            EntityState.Modified => EAuditType.Update.ToString(),
            _ => throw new NotSupportedException("Estado não suportado para auditoria")
        };
    }

    private static string? GetOldValues(EntityEntry entry)
    {
        if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
        {
            var oldValues = entry.Properties
                .Where(p => p.IsModified || entry.State == EntityState.Deleted)
                .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);

            return oldValues.Count > 0 ? JsonConvert.SerializeObject(oldValues) : null;
        }

        return null;
    }

    private static string? GetNewValues(EntityEntry entry)
    {
        if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
        {
            var newValues = entry.Properties
                .Where(p => entry.State == EntityState.Added || p.IsModified)
                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

            return newValues.Count > 0 ? JsonConvert.SerializeObject(newValues) : null;
        }

        return null;
    }

    private static string? GetChangedColumns(EntityEntry entry)
    {
        if (entry.State == EntityState.Modified)
        {
            var changedColumns = entry.Properties
                .Where(p => p.IsModified)
                .Select(p => p.Metadata.Name)
                .ToList();

            return changedColumns.Count > 0 ? JsonConvert.SerializeObject(changedColumns) : null;
        }

        return null;
    }

    private static Dictionary<string, object> GetPrimaryKeyValues(EntityEntry entry)
    {
        var keyValues = new Dictionary<string, object>();
        var keyNames = entry.Metadata.FindPrimaryKey().Properties;

        foreach (var keyName in keyNames)
        {
            keyValues[keyName.Name] = entry.Property(keyName.Name).CurrentValue!;
        }

        return keyValues;
    }
}
