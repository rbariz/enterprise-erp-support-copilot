namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public sealed record TicketAnalysisResult(
        string Summary,
        string ProbableRootCause,
        string RecommendedActions,
        string RiskAssessment,
        int ConfidenceScore
    );
}
