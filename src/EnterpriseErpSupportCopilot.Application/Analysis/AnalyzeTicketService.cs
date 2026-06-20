using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Application.Rag;
using EnterpriseErpSupportCopilot.Contracts.Analysis;
using EnterpriseErpSupportCopilot.Contracts.Incidents;
using EnterpriseErpSupportCopilot.Contracts.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Application.Analysis
{

    public sealed class AnalyzeTicketService
    {
        private readonly ITicketRepository _tickets;
        private readonly IAnalysisRepository _analysis;
        private readonly ITicketAnalysisProvider _provider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRagContextBuilder _ragContextBuilder;

        public AnalyzeTicketService(
            ITicketRepository tickets,
            IAnalysisRepository analysis,
            ITicketAnalysisProvider provider,
            IUnitOfWork unitOfWork,
            IRagContextBuilder ragContextBuilder)
        {
            _tickets = tickets;
            _analysis = analysis;
            _provider = provider;
            _unitOfWork = unitOfWork;
            _ragContextBuilder = ragContextBuilder;
        }

        public async Task<TicketAnalysisResultDto?> AnalyzeAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var ticket = await _tickets.GetByIdAsync(ticketId, cancellationToken);

            if (ticket is null)
            {
                return null;
            }

            var ragContext = await _ragContextBuilder.BuildAsync(
    ticket,
    cancellationToken);

            var knowledge = ragContext.KnowledgeArticles
                .Select(x => x.Article)
                .ToList();
            var similarIncidents = ragContext.SimilarIncidents;

            var result = await _provider.AnalyzeAsync(
                    ticket,
                    knowledge,
                    similarIncidents,
                    cancellationToken);

            var analysis = new AiTicketAnalysis
            {
                TicketId = ticket.Id,
                Summary = result.Summary,
                ProbableRootCause = result.ProbableRootCause,
                RecommendedActions = result.RecommendedActions,
                RiskAssessment = result.RiskAssessment,
                ConfidenceScore = result.ConfidenceScore
            };

            await _analysis.AddAsync(analysis, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var analysisDto = new AiTicketAnalysisDto(
                    analysis.Id,
                    analysis.TicketId,
                    analysis.Summary,
                    analysis.ProbableRootCause,
                    analysis.RecommendedActions,
                    analysis.RiskAssessment,
                    analysis.ConfidenceScore,
                    analysis.CreatedAt);

            var query = $"""
                    {ticket.Title}
                    {ticket.Description}
                    {ticket.Category}
                    """;

            var knowledgeDtos = ragContext.KnowledgeArticles.Select(x =>
                new KnowledgeSourceDto(
                    x.Article.Id,
                    x.Article.Title,
                    x.Article.Content,
                    x.Article.Tags,
                    x.Distance,
                    (int)Math.Round((1 - x.Distance) * 100),
                    RagExplainabilityHelper.FindMatches(
                        query,
                        x.Article.Tags),
                    KeywordScore: x.KeywordScore,
                    FinalScore: x.FinalScore
                ))
                .ToList();

            var incidentDtos = similarIncidents.Select(x => new ResolvedIncidentDto(
                    x.Id,
                    x.ProblemSummary,
                    x.RootCause,
                    x.Resolution,
                    x.Tags,
                    x.ResolvedAt)).ToList();

            var matchedTermsCount = knowledgeDtos
    .SelectMany(x => x.MatchedTerms)
    .Distinct()
    .Count();

            var contextSize = knowledgeDtos.Sum(x =>
                x.Content.Length +
                x.Title.Length +
                x.Tags.Length);

            contextSize += incidentDtos.Sum(x =>
                x.ProblemSummary.Length +
                x.RootCause.Length +
                x.Resolution.Length);

            var metrics = new RagContextMetricsDto(
                KnowledgeArticles: knowledgeDtos.Count,
                ResolvedIncidents: incidentDtos.Count,
                MatchedTerms: matchedTermsCount,
                ContextSizeBytes: contextSize);

            return new TicketAnalysisResultDto(analysisDto, knowledgeDtos, incidentDtos, metrics);
        }
    }
}
