using EnterpriseErpSupportCopilot.Contracts.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Knowledge;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface IKnowledgeSearchService
    {
        Task<IReadOnlyList<KnowledgeSearchHit>> SearchAsync(
            string query,
            int topK,
            CancellationToken cancellationToken);
    }
}
