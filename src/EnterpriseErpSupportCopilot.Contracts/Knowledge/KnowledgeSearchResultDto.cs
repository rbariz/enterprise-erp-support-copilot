namespace EnterpriseErpSupportCopilot.Contracts.Knowledge
{
    public sealed record KnowledgeSearchResultDto(
        Guid Id,
        string Title,
        string Content,
        string Tags,
        double Distance
    );
}
