namespace EnterpriseErpSupportCopilot.Contracts.Analysis
{
    public sealed record CreateAnalysisFeedbackRequest(
    Guid AnalysisId,
    bool IsUseful,
    string? Comment
);
}
