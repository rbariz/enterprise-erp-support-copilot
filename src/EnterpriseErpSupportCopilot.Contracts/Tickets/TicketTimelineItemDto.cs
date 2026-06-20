namespace EnterpriseErpSupportCopilot.Contracts.Tickets
{
    public sealed record TicketTimelineItemDto(
        DateTime Timestamp,
        string Type,
        string Title,
        string Description
    );
}
