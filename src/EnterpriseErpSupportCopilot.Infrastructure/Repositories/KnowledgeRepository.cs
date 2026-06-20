using EnterpriseErpSupportCopilot.Application.Abstractions;
using EnterpriseErpSupportCopilot.Domain.Knowledge;
using EnterpriseErpSupportCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace EnterpriseErpSupportCopilot.Infrastructure.Repositories
{
    public sealed class KnowledgeRepository : IKnowledgeRepository
    {
        private readonly SupportCopilotDbContext _db;

        public KnowledgeRepository(SupportCopilotDbContext db)
        {
            _db = db;
        }

        public async Task<List<KnowledgeArticle>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            var terms = SearchText.ExtractTerms(query);

            return await _db.KnowledgeArticles
                .Where(x => terms.Any(term =>
                    x.Title.ToLower().Contains(term) ||
                    x.Content.ToLower().Contains(term) ||
                    x.Tags.ToLower().Contains(term)))
                .Take(5)
                .ToListAsync(cancellationToken);
        }
    }
}
