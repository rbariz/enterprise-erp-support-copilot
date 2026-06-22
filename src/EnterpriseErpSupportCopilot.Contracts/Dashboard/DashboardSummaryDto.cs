using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Contracts.Dashboard
{
    public sealed record DashboardSummaryDto(
    int TotalTickets,
    int OpenTickets,
    int CriticalTickets,
    int KnowledgeArticles,
    int ResolvedIncidents,
    int AnalysesGenerated,
    int UsefulFeedback,
    int NotUsefulFeedback
);
}
