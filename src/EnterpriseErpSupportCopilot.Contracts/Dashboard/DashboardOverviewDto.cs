using EnterpriseErpSupportCopilot.Contracts.Analysis;
using EnterpriseErpSupportCopilot.Contracts.Tickets;

namespace EnterpriseErpSupportCopilot.Contracts.Dashboard
{
    public sealed record DashboardOverviewDto(
    DashboardSummaryDto Summary,
    IReadOnlyList<TicketDto> RecentTickets,
    IReadOnlyList<AiTicketAnalysisDto> RecentAnalyses,
    IReadOnlyList<ActivityItemDto> RecentActivities
);
}
