namespace EnterpriseErpSupportCopilot.Contracts.Tickets
{
    public sealed record CreateTicketRequest(
        string Title,
        string Description,
        string Category,
        string Severity
    );
}
