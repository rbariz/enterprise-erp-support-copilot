using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Contracts.Tickets
{
    public sealed record TicketDto(
    Guid Id,
    string Reference,
    string Title,
    string Description,
    string Category,
    string Severity,
    string Status,
    DateTime CreatedAt
);
}
