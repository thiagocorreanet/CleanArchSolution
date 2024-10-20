using Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class ConfigurationAudit : IEntityTypeConfiguration<AuditProcess>
{
    public void Configure(EntityTypeBuilder<AuditProcess> builder)
    {
        builder.ToTable("AuditsProcess");

        builder.HasKey(c => c.Id)
            .HasName("PK_AUDITSPROCESS");

        builder.Property(c => c.Id)
            .HasColumnName("AuditProcessId")
            .HasColumnOrder(1)
            .ValueGeneratedOnAdd()
            .HasComment("Chave primária da auditoria.");

        builder.Property(c => c.Type)
           .HasColumnName("Type")
           .HasColumnOrder(2)
           .IsRequired()
           .HasColumnType("VARCHAR(20)")
           .HasComment("Tipo da auditoria");

        builder.Property(c => c.Table)
           .HasColumnName("Table")
           .HasColumnOrder(3)
           .IsRequired()
           .HasColumnType("VARCHAR(100)")
           .HasComment("Tabela na qual vai ser exibida na auditoria");

        builder.Property(c => c.OldValues)
           .HasColumnName("OldValues")
           .HasColumnOrder(4)
           .HasColumnType("VARCHAR(MAX)")
           .HasComment("Valor antigo");

        builder.Property(c => c.NewValues)
           .HasColumnName("NewValues")
           .HasColumnOrder(5)
           .HasColumnType("VARCHAR(MAX)")
           .HasComment("Novo valor");

        builder.Property(c => c.AffectedColumns)
           .HasColumnName("AffectedColumns")
           .HasColumnOrder(6)
           .HasColumnType("VARCHAR(MAX)")
           .HasComment("Colunas que foram afetadas");

        builder.Property(c => c.PrimaryKey)
           .HasColumnName("PrimaryKey")
           .HasColumnOrder(7)
           .HasColumnType("VARCHAR(50)")
           .HasComment("Primary key da tabela");

        builder.HasIndex(c => c.Id)
            .HasDatabaseName("IX_AUDITSPROCESS_USERID")
            .IsUnique();
    }
}
