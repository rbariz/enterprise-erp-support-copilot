using EnterpriseErpSupportCopilot.Domain.Knowledge;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface IKnowledgeRepository
    {
        Task<List<KnowledgeArticle>> SearchAsync(string query, CancellationToken cancellationToken);
    }
}
