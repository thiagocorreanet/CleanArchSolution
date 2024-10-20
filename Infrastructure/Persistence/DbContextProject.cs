
using Core.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence
{
    public class DbContextProject : IdentityDbContext<IdentityUser>
    {
        public DbContextProject(DbContextOptions options) : base(options) { }

        public DbSet<AuditProcess> AuditsProcess => Set<AuditProcess>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextProject).Assembly);
        }

        public void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = GetAuditEntries();
            foreach (var auditEntry in auditEntries)
            {
                AuditsProcess.Add(auditEntry);
            }
        }

        private List<AuditProcess> GetAuditEntries()
        {
            var auditEntries = new List<AuditProcess>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (ShouldSkip(entry))
                    continue;

                var auditProcess = AuditProcess.CreateFromEntity(entry);
                auditEntries.Add(auditProcess);
            }

            return auditEntries;
        }

        private bool ShouldSkip(EntityEntry entry)
        {
            return entry.Entity is AuditProcess || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged;
        }
    }
}
