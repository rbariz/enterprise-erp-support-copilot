namespace EnterpriseErpSupportCopilot.Contracts.Dashboard
{
    public sealed record AiProviderStatusDto(
    string Provider,
    string Model,
    bool IsConfigured,
    double Temperature,
    int MaxTokens
);
}
