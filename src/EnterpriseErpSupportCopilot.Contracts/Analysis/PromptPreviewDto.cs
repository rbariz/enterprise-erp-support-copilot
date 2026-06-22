namespace EnterpriseErpSupportCopilot.Contracts.Analysis
{
    public sealed record PromptPreviewDto(
        string SystemPrompt,
        string UserPrompt,
        int KnowledgeArticles,
        int SimilarIncidents,
        int EstimatedContextSize
    );
}
