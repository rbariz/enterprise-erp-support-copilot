namespace EnterpriseErpSupportCopilot.Contracts.Knowledge
{
    public sealed record KnowledgeSourceDto(
    Guid Id,
    string Title,
    string Content,
    string Tags,
    double Distance,
    int SimilarityScore,
    IReadOnlyList<string> MatchedTerms,
    double KeywordScore,
    double FinalScore
);

}
