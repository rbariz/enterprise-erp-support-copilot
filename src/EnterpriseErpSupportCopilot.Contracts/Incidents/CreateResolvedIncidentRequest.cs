namespace EnterpriseErpSupportCopilot.Contracts.Incidents
{
    public sealed record CreateResolvedIncidentRequest(
    string ProblemSummary,
    string RootCause,
    string Resolution,
    string Tags
);
}
