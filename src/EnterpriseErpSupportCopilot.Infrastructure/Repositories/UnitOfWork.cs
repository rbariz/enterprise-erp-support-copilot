using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
namespace EnterpriseErpSupportCopilot.Infrastructure.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly SupportCopilotDbContext _db;

        public UnitOfWork(SupportCopilotDbContext db)
        {
            _db = db;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
