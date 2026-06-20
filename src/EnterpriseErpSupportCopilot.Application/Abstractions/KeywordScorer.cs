using EnterpriseErpSupportCopilot.Domain.Knowledge;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public static class KeywordScorer
    {
        public static double Score(
            string query,
            KnowledgeArticle article)
        {
            var queryTerms = query
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToList();

            var text =
                $"{article.Title} {article.Tags} {article.Content}"
                .ToLowerInvariant();

            var matches = queryTerms.Count(x => text.Contains(x));

            if (queryTerms.Count == 0)
            {
                return 0;
            }

            return (double)matches / queryTerms.Count;
        }
    }
}
