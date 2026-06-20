namespace EnterpriseErpSupportCopilot.Domain.Analysis
{
    public class AnalysisFeedback
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AnalysisId { get; set; }

        public bool IsUseful { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
