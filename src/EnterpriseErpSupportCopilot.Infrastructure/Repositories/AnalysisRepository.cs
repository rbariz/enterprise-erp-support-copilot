using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Domain.Analysis;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace EnterpriseErpSupportCopilot.Infrastructure.Repositories
{
    public sealed class AnalysisRepository : IAnalysisRepository
    {
        private readonly SupportCopilotDbContext _db;

        public AnalysisRepository(SupportCopilotDbContext db)
        {
            _db = db;
        }

        public Task<AiTicketAnalysis?> GetLatestByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            return _db.Analyses
                .Where(x => x.TicketId == ticketId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(AiTicketAnalysis analysis, CancellationToken cancellationToken)
        {
            await _db.Analyses.AddAsync(analysis, cancellationToken);
        }
    }
}
