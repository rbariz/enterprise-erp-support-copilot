using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Application.Rag
{
    public static class RagExplainabilityHelper
    {
        public static IReadOnlyList<string> FindMatches(
            string query,
            string tags)
        {
            var queryTerms = query
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToHashSet();

            return tags
                .ToLowerInvariant()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Where(queryTerms.Contains)
                .Distinct()
                .Take(5)
                .ToList();
        }
    }
}
