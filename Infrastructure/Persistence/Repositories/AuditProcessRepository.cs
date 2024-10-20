using Core.Entities;
using Core.Repositories;


namespace Infrastructure.Persistence.Repositories;

public class AuditProcessRepository : BaseRepository<AuditProcess>, IAuditProcessRepository
{
    public AuditProcessRepository(DbContextProject context) : base(context)
    {
    }
}
