using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Infrastructure.Rag
{

    public sealed class RagContextBuilder : IRagContextBuilder
    {
        private readonly IKnowledgeSearchService _knowledgeSearch;
        private readonly IResolvedIncidentRepository _incidents;

        public RagContextBuilder(
            IKnowledgeSearchService knowledgeSearch,
            IResolvedIncidentRepository incidents)
        {
            _knowledgeSearch = knowledgeSearch;
            _incidents = incidents;
        }

        public async Task<RagContext> BuildAsync(
    SupportTicket ticket,
    CancellationToken cancellationToken)
        {
            var query = BuildQuery(ticket);

            var articles = await _knowledgeSearch.SearchAsync(
                query,
                topK: 5,
                cancellationToken);

            var incidents = await _incidents.SearchSimilarAsync(
                query,
                cancellationToken);

            return new RagContext(
                articles,
                incidents);
        }

        private static string BuildQuery(SupportTicket ticket)
        {
            return $"""
    ERP support ticket.

    Title:
    {ticket.Title}

    Description:
    {ticket.Description}

    Category:
    {ticket.Category}

    Severity:
    {ticket.Severity}

    Expected support knowledge:
    Oracle EBS troubleshooting, workflow issues, concurrent requests, interface errors, forms errors, OAF errors, payables, purchasing, inventory and general ledger incidents.
    """;
        }
    }
}
