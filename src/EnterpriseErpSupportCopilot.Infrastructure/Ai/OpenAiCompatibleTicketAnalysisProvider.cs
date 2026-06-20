using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Tickets;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnterpriseErpSupportCopilot.Infrastructure.Ai
{
    public sealed class OpenAiCompatibleTicketAnalysisProvider : ITicketAnalysisProvider
    {
        private readonly HttpClient _http;
        private readonly AiProviderOptions _options;

        public OpenAiCompatibleTicketAnalysisProvider(
            HttpClient http,
            IOptions<AiProviderOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public async Task<TicketAnalysisResult> AnalyzeAsync(
                    SupportTicket ticket,
                    IReadOnlyList<KnowledgeArticle> knowledgeArticles,
                    IReadOnlyList<ResolvedIncident> similarIncidents,
                    CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.OpenAiCompatibleApiKey))
            {
                return Fallback(ticket, knowledgeArticles, similarIncidents);
            }

            _http.BaseAddress = new Uri(_options.OpenAiCompatibleBaseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.OpenAiCompatibleApiKey);

            var prompt = BuildPrompt(ticket, knowledgeArticles, similarIncidents);

            var request = new
            {
                model = _options.OpenAiCompatibleModel,
                temperature = _options.Temperature,
                max_tokens = _options.MaxTokens,
                response_format = new { type = "json_object" },
                messages = new object[]
                {
                new
                {
                    role = "system",
                    content = """
                    You are an ERP support copilot specialized in Oracle E-Business Suite-like incidents.
                    Analyze support tickets using only the provided ticket and knowledge context.
                    Return strict JSON only.
                    """
                },
                new
                {
                    role = "user",
                    content = prompt
                }
                }
            };

            var response = await _http.PostAsJsonAsync(
                "chat/completions",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return Fallback(ticket, knowledgeArticles, similarIncidents);
            }

            var payload = await response.Content.ReadFromJsonAsync<OpenAiChatResponse>(
                cancellationToken: cancellationToken);

            var content = payload?.Choices?.FirstOrDefault()?.Message?.Content;

            if (string.IsNullOrWhiteSpace(content))
            {
                return Fallback(ticket, knowledgeArticles, similarIncidents);
            }

            try
            {
                var parsed = JsonSerializer.Deserialize<AiAnalysisJson>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (parsed is null)
                {
                    return Fallback(ticket, knowledgeArticles, similarIncidents);
                }

                return new TicketAnalysisResult(
                    parsed.Summary,
                    parsed.ProbableRootCause,
                    parsed.RecommendedActions,
                    parsed.RiskAssessment,
                    Math.Clamp(parsed.ConfidenceScore, 0, 100));
            }
            catch
            {
                return Fallback(ticket, knowledgeArticles, similarIncidents);
            }
        }

        private static string BuildPrompt(
    SupportTicket ticket,
    IReadOnlyList<KnowledgeArticle> knowledgeArticles,
    IReadOnlyList<ResolvedIncident> similarIncidents)
        {
            var knowledge = knowledgeArticles.Count == 0
                ? "No relevant knowledge articles found."
                : string.Join("\n\n", knowledgeArticles.Select(x =>
                    $"""
            Article: {x.Title}
            Tags: {x.Tags}
            Content: {x.Content}
            """));

            var incidents = similarIncidents.Count == 0
                ? "No similar resolved incidents found."
                : string.Join("\n\n", similarIncidents.Select(x =>
                    $"""
            Incident: {x.ProblemSummary}
            Root Cause: {x.RootCause}
            Resolution: {x.Resolution}
            Tags: {x.Tags}
            """));

            return $$"""
    Analyze this ERP support ticket.

    Ticket:
    Reference: {{ticket.Reference}}
    Title: {{ticket.Title}}
    Category: {{ticket.Category}}
    Severity: {{ticket.Severity}}
    Description: {{ticket.Description}}

    Knowledge Context:
    {{knowledge}}

    Similar Resolved Incidents:
    {{incidents}}

    Return strict JSON with this exact schema:
    {
      "summary": "short ticket summary",
      "probableRootCause": "most likely root cause",
      "recommendedActions": "clear step-by-step actions",
      "riskAssessment": "operational risk assessment",
      "confidenceScore": 0
    }
    """;
        }

        private static TicketAnalysisResult Fallback(
    SupportTicket ticket,
    IReadOnlyList<KnowledgeArticle> knowledgeArticles,
    IReadOnlyList<ResolvedIncident> similarIncidents)
        {
            var knowledgeHint = knowledgeArticles.Count > 0
                ? $"Found {knowledgeArticles.Count} relevant knowledge article(s)."
                : "No matching knowledge article was found.";

            var incidentHint = similarIncidents.Count > 0
                ? $"Found {similarIncidents.Count} similar resolved incident(s)."
                : "No similar resolved incident was found.";

            var confidenceScore = knowledgeArticles.Count switch
            {
                > 0 when similarIncidents.Count > 0 => 80,
                > 0 => 70,
                _ when similarIncidents.Count > 0 => 65,
                _ => 45
            };

            return new TicketAnalysisResult(
                $"Ticket {ticket.Reference} reports an ERP issue related to {ticket.Category}.",
                $"Likely cause requires investigation based on ERP logs, workflow status, interface data and historical incidents. {knowledgeHint} {incidentHint}",
                "Review application logs, validate source data, check workflow/concurrent request status, compare with similar resolved incidents and apply the recommended resolution pattern when applicable.",
                ticket.Severity is TicketSeverity.High or TicketSeverity.Critical
                    ? "High operational risk. Immediate support investigation recommended."
                    : "Moderate operational risk. Standard support investigation recommended.",
                confidenceScore);
        }

        private sealed class OpenAiChatResponse
        {
            public List<OpenAiChoice> Choices { get; set; } = [];
        }

        private sealed class OpenAiChoice
        {
            public OpenAiMessage Message { get; set; } = new();
        }

        private sealed class OpenAiMessage
        {
            public string Content { get; set; } = "";
        }

        private sealed class AiAnalysisJson
        {
            public string Summary { get; set; } = "";
            public string ProbableRootCause { get; set; } = "";
            public string RecommendedActions { get; set; } = "";
            public string RiskAssessment { get; set; } = "";
            public int ConfidenceScore { get; set; }
        }
    }
}
