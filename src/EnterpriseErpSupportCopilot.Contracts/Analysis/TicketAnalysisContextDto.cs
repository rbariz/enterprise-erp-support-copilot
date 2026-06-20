using EnterpriseErpSupportCopilot.Contracts.Incidents;
using EnterpriseErpSupportCopilot.Contracts.Knowledge;

namespace EnterpriseErpSupportCopilot.Contracts.Analysis
{
    public sealed record TicketAnalysisContextDto(
    IReadOnlyList<KnowledgeSourceDto> KnowledgeArticles,
    IReadOnlyList<ResolvedIncidentDto> SimilarIncidents
);
}
