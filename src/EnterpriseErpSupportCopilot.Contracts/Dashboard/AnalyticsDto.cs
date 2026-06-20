namespace EnterpriseErpSupportCopilot.Contracts.Dashboard
{
    public sealed record AnalyticsDto(
    IReadOnlyList<GroupCountDto> TicketsBySeverity,
    IReadOnlyList<GroupCountDto> TicketsByCategory,
    IReadOnlyList<GroupCountDto> TicketsByStatus
);
}
