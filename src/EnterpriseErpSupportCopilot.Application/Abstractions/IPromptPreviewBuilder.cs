using EnterpriseErpSupportCopilot.Contracts.Analysis;
using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Tickets;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface IPromptPreviewBuilder
    {
        PromptPreviewDto Build(
            SupportTicket ticket,
            IReadOnlyList<KnowledgeArticle> knowledgeArticles,
            IReadOnlyList<ResolvedIncident> similarIncidents);
    }
}
