namespace EnterpriseErpSupportCopilot.Contracts.Analysis
{
    public sealed record RagContextMetricsDto(
    int KnowledgeArticles,
    int ResolvedIncidents,
    int MatchedTerms,
    int ContextSizeBytes
);
}
