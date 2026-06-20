using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Knowledge;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public sealed record RagContext(
    IReadOnlyList<KnowledgeSearchHit> KnowledgeArticles,
    IReadOnlyList<ResolvedIncident> SimilarIncidents
);
}
