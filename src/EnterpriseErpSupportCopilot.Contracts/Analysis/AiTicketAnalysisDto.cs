using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Contracts.Analysis
{
    public sealed record AiTicketAnalysisDto(
    Guid Id,
    Guid TicketId,
    string Summary,
    string ProbableRootCause,
    string RecommendedActions,
    string RiskAssessment,
    int ConfidenceScore,
    DateTime CreatedAt
);
}
