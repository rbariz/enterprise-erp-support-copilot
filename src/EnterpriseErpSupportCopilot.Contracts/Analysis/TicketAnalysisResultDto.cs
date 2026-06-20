using EnterpriseErpSupportCopilot.Contracts.Incidents;
using EnterpriseErpSupportCopilot.Contracts.Knowledge;

namespace EnterpriseErpSupportCopilot.Contracts.Analysis
{
    public sealed record TicketAnalysisResultDto(
        AiTicketAnalysisDto Analysis,
        IReadOnlyList<KnowledgeSourceDto> KnowledgeArticles,
    IReadOnlyList<ResolvedIncidentDto> SimilarIncidents,
    RagContextMetricsDto ContextMetrics
    );
}
