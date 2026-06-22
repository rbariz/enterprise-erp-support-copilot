namespace EnterpriseErpSupportCopilot.Contracts.Analysis
{
    public sealed record AnalysisFeedbackDto(
    Guid Id,
    Guid AnalysisId,
    bool IsUseful,
    string? Comment,
    DateTime CreatedAt
);
}
