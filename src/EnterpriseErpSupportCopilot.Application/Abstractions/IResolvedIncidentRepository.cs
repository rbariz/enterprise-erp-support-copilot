using EnterpriseErpSupportCopilot.Domain.Incidents;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface IResolvedIncidentRepository
    {
        Task<List<ResolvedIncident>> SearchSimilarAsync(
            string query,
            CancellationToken cancellationToken);
    }
}
