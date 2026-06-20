namespace EnterpriseErpSupportCopilot.Infrastructure.Repositories
{
    internal static class SearchText
    {
        public static string[] ExtractTerms(string query)
        {
            return query
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => x.Length >= 3)
                .Distinct()
                .Take(10)
                .ToArray();
        }
    }
}
