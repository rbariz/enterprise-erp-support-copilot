using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Domain.Incidents;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Infrastructure.Ai
{
    public sealed class StubTicketAnalysisProvider : ITicketAnalysisProvider
    {
        public Task<TicketAnalysisResult> AnalyzeAsync(
    SupportTicket ticket,
    IReadOnlyList<KnowledgeArticle> knowledgeArticles,
    IReadOnlyList<ResolvedIncident> similarIncidents,
    CancellationToken cancellationToken)
        {
            var knowledgeHint = knowledgeArticles.Count == 0
                ? "No matching knowledge article was found."
                : $"Found {knowledgeArticles.Count} potentially relevant knowledge article(s).";

            var result = new TicketAnalysisResult(
                Summary: $"Ticket {ticket.Reference} reports an issue related to {ticket.Category}.",
                ProbableRootCause: $"Likely cause requires investigation based on ERP logs, workflow status and interface data. {knowledgeHint}",
                RecommendedActions: "Check concurrent request logs, review related interface errors, validate workflow status and compare with similar resolved incidents.",
                RiskAssessment: ticket.Severity is TicketSeverity.Critical or TicketSeverity.High
                    ? "High operational risk. Immediate support investigation recommended."
                    : "Moderate operational risk. Standard support investigation recommended.",
                ConfidenceScore: knowledgeArticles.Count > 0 ? 70 : 45);

            return Task.FromResult(result);
        }
    }
}
