using EnterpriseErpSupportCopilot.Domain.Knowledge;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public sealed record KnowledgeSearchHit(
    KnowledgeArticle Article,
    double Distance,
    double KeywordScore,
    double FinalScore
);
}
