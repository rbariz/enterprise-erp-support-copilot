using EnterpriseErpSupportCopilot.Domain.Analysis;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface IAnalysisRepository
    {
        Task<AiTicketAnalysis?> GetLatestByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken);
        Task AddAsync(AiTicketAnalysis analysis, CancellationToken cancellationToken);
    }
}
