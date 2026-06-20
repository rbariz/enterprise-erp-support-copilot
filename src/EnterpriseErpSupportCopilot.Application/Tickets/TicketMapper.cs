using EnterpriseErpSupportCopilot.Contracts.Tickets;
using EnterpriseErpSupportCopilot.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Application.Tickets
{
    public static class TicketMapper
    {
        public static TicketDto ToDto(SupportTicket ticket)
        {
            return new TicketDto(
                ticket.Id,
                ticket.Reference,
                ticket.Title,
                ticket.Description,
                ticket.Category.ToString(),
                ticket.Severity.ToString(),
                ticket.Status.ToString(),
                ticket.CreatedAt);
        }
    }
}
