using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace EnterpriseErpSupportCopilot.Infrastructure.Repositories
{
    public sealed class ResolvedIncidentRepository : IResolvedIncidentRepository
    {
        private readonly SupportCopilotDbContext _db;

        public ResolvedIncidentRepository(SupportCopilotDbContext db)
        {
            _db = db;
        }

        public async Task<List<ResolvedIncident>> SearchSimilarAsync(
    string query,
    CancellationToken cancellationToken)
        {
            var terms = SearchText.ExtractTerms(query);

            return await _db.ResolvedIncidents
                .Where(x => terms.Any(term =>
                    x.ProblemSummary.ToLower().Contains(term) ||
                    x.RootCause.ToLower().Contains(term) ||
                    x.Resolution.ToLower().Contains(term) ||
                    x.Tags.ToLower().Contains(term)))
                .Take(5)
                .ToListAsync(cancellationToken);
        }
    }
}
