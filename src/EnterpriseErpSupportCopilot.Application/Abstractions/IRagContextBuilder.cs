using EnterpriseErpSupportCopilot.Domain.Tickets;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface IRagContextBuilder
    {
        Task<RagContext> BuildAsync(
            SupportTicket ticket,
            CancellationToken cancellationToken);
    }
}
