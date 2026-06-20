using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Contracts.Knowledge;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace EnterpriseErpSupportCopilot.Infrastructure.Knowledge
{

    public sealed class KnowledgeSearchService : IKnowledgeSearchService
    {
        private const double MaxDistance = 0.55;

        private readonly SupportCopilotDbContext _db;
        private readonly IEmbeddingProvider _embeddingProvider;
        private readonly ILogger<KnowledgeSearchService> _logger;

        public KnowledgeSearchService(
            SupportCopilotDbContext db,
            IEmbeddingProvider embeddingProvider,
            ILogger<KnowledgeSearchService> logger)
        {
            _db = db;
            _embeddingProvider = embeddingProvider;
            _logger = logger;
        }

        public async Task<IReadOnlyList<KnowledgeSearchHit>> SearchAsync(
    string query,
    int topK,
    CancellationToken cancellationToken)
        {
            var embedding = await _embeddingProvider.EmbedAsync(
                query,
                cancellationToken);

            var vector = new Vector(embedding);

            var candidates = await _db.KnowledgeArticles
                .Where(x => x.Embedding != null)
                .Select(x => new
                {
                    Article = x,
                    Distance = x.Embedding!.CosineDistance(vector)
                })
                .OrderBy(x => x.Distance)
                .Take(topK * 3)
                .ToListAsync(cancellationToken);

            var ranked = candidates
                .Select(x =>
                {
                    var keywordScore = KeywordScorer.Score(query, x.Article);
                    var vectorScore = 1 - x.Distance;
                    var finalScore = (vectorScore * 0.7) + (keywordScore * 0.3);

                    return new KnowledgeSearchHit(
                        x.Article,
                        x.Distance,
                        keywordScore,
                        finalScore);
                })
                .Where(x => x.FinalScore >= 0.55)
                .OrderByDescending(x => x.FinalScore)
                .Take(topK)
                .ToList();

            foreach (var hit in ranked)
            {
                _logger.LogInformation(
                    "RAG Match: {Title} | Distance={Distance} | Keyword={Keyword} | Final={Final}",
                    hit.Article.Title,
                    hit.Distance,
                    hit.KeywordScore,
                    hit.FinalScore);
            }

            return ranked;
        }
    }
}
