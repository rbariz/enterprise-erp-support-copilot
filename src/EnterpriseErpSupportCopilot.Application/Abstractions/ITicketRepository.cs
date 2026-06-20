using EnterpriseErpSupportCopilot.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseErpSupportCopilot.Application.Abstractions
{
    public interface ITicketRepository
    {
        Task<List<SupportTicket>> GetAllAsync(CancellationToken cancellationToken);
        Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken);
        Task UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken);
    }
}
