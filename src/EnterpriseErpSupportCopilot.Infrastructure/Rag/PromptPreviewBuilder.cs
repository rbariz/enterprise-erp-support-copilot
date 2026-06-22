using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Contracts.Analysis;
using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Tickets;

namespace EnterpriseErpSupportCopilot.Infrastructure.Rag
{
    public sealed class PromptPreviewBuilder : IPromptPreviewBuilder
    {
        public PromptPreviewDto Build(
            SupportTicket ticket,
            IReadOnlyList<KnowledgeArticle> knowledgeArticles,
            IReadOnlyList<ResolvedIncident> similarIncidents)
        {
            var systemPrompt = """
        You are an ERP support copilot specialized in Oracle E-Business Suite-like incidents.
        Analyze support tickets using only the provided ticket, knowledge articles and resolved incidents.
        Return structured support analysis with summary, probable root cause, recommended actions, risk assessment and confidence score.
        """;

            var knowledge = knowledgeArticles.Count == 0
                ? "No relevant knowledge articles found."
                : string.Join("\n\n", knowledgeArticles.Select(x =>
                    $"""
                Knowledge Article:
                Title: {x.Title}
                Tags: {x.Tags}
                Content: {x.Content}
                """));

            var incidents = similarIncidents.Count == 0
                ? "No similar resolved incidents found."
                : string.Join("\n\n", similarIncidents.Select(x =>
                    $"""
                Similar Resolved Incident:
                Problem: {x.ProblemSummary}
                Root Cause: {x.RootCause}
                Resolution: {x.Resolution}
                Tags: {x.Tags}
                """));

            var userPrompt = $"""
        Analyze this ERP support ticket.

        Ticket:
        Reference: {ticket.Reference}
        Title: {ticket.Title}
        Category: {ticket.Category}
        Severity: {ticket.Severity}
        Description: {ticket.Description}

        Knowledge Context:
        {knowledge}

        Similar Resolved Incidents:
        {incidents}

        Expected Output:
        - Summary
        - Probable Root Cause
        - Recommended Actions
        - Risk Assessment
        - Confidence Score
        """;

            var size = systemPrompt.Length + userPrompt.Length;

            return new PromptPreviewDto(
                systemPrompt,
                userPrompt,
                knowledgeArticles.Count,
                similarIncidents.Count,
                size);
        }
    }
}
