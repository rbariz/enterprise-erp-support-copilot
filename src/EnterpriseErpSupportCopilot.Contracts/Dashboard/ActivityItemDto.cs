namespace EnterpriseErpSupportCopilot.Contracts.Dashboard
{
    public sealed record ActivityItemDto(
        DateTime Timestamp,
        string Type,
        string Description
    );
}
