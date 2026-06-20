using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Infrastructure.Knowledge
{

    public sealed class KnowledgeEmbeddingService
    {
        private readonly SupportCopilotDbContext _db;
        private readonly IEmbeddingProvider _embeddingProvider;

        public KnowledgeEmbeddingService(
            SupportCopilotDbContext db,
            IEmbeddingProvider embeddingProvider)
        {
            _db = db;
            _embeddingProvider = embeddingProvider;
        }

        public async Task<int> GenerateMissingEmbeddingsAsync(CancellationToken cancellationToken)
        {
            var articles = await _db.KnowledgeArticles
                .Where(x => x.Embedding == null)
                .ToListAsync(cancellationToken);

            foreach (var article in articles)
            {
                var text = $"{article.Title}\n{article.Tags}\n{article.Content}";
                var embedding = await _embeddingProvider.EmbedAsync(text, cancellationToken);

                article.Embedding = new Vector(embedding);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return articles.Count;
        }
    }
}
