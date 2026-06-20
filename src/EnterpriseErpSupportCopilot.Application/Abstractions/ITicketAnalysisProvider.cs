using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Tickets;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface ITicketAnalysisProvider
    {
        Task<TicketAnalysisResult> AnalyzeAsync(
            SupportTicket ticket,
            IReadOnlyList<KnowledgeArticle> knowledgeArticles,
        IReadOnlyList<ResolvedIncident> similarIncidents,
            CancellationToken cancellationToken);
    }
}
