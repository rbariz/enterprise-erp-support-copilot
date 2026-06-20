using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Domain.Analysis
{
    public class AiTicketAnalysis
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TicketId { get; set; }

        public string Summary { get; set; } = default!;
        public string ProbableRootCause { get; set; } = default!;
        public string RecommendedActions { get; set; } = default!;
        public string RiskAssessment { get; set; } = default!;

        public int ConfidenceScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
